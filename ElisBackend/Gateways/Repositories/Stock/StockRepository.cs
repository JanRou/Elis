using ElisBackend.Core.Application.Queries;
using ElisBackend.Core.Domain.Entities;
using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Extensions;
using ElisBackend.Gateways.Dal;
using ElisBackend.Gateways.Repositories.Daos;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Linq;

namespace ElisBackend.Gateways.Repositories.Stock
{

    public interface IStockRepository {
        /// <summary>
        /// Gets stocks from the repostitory using a filter. 
        /// </summary>
        /// <param name="filter">Filter to select stocks.</param>
        /// <returns>A list of stocks</returns>
        Task<IEnumerable<StockDao>> Get(FilterStock filter);

        /// <summary>
        /// Adds a new stock referring to the exchange by name or id and currency by by code or id.
        /// Exchange or currency can't be created at the same time as a stock.
        /// </summary>
        /// <param name="stock">Stock to add with name and isin code.</param>
        /// <returns>Stock added with ExchangeId and CurrencyId</returns>
        Task<StockDao> Add(StockDao stock);

        /// <summary>
        /// Delete a stock identified by it's isin code
        /// </summary>
        /// <param name="isin">Code identifying the stock</param>
        /// <returns>True when deleted otherwise false</returns>
        Task<bool> DeleteStock(string isin);
        Task<bool> DeleteStock(int id);

        /// <summary>
        /// Add a timeserie data to the stock identified by isin code.
        /// </summary>
        /// <param name="isin">Isin code for the stock</param>
        /// <param name="timeSerie">TimeSerie to add</param>
        /// <returns>Count of facts added to the time serie</returns>
        Task<int> AddOrUpdateTimeSerieAddFacts(string isin, TimeSerieDao timeSerie);
    }

    public class StockRepository(ElisContext db) : IStockRepository {
        // TODO DbContext concurrency

        public async Task<IEnumerable<StockDao>> Get(FilterStock filter) {

            List<int> stockIds = null;
            var parms = new List<NpgsqlParameter>().QueryParametersFromClass<FilterStock>(filter);
            string sql = "select * FROM public.SearchStocks(" + parms.CreateParameterNames() + ")";
            // SearchStocks returns ids in sorted order
            try {
                stockIds = db.Database.SqlQueryRaw<int>(sql, parms.ToArray()).ToList();
            }
            catch (Exception ex) {
                // TODO LOG exception
                throw ex;
            }

            var unsorted = db.Stocks
                .Where<StockDao>(s => stockIds.Contains(s.Id))                
                .Include(s => s.Currency)
                .Include(s => s.Exchange)
                .ToList();

            // Sort by searchstocks result before returning
            return unsorted.OrderBy(u => stockIds.FindIndex( i => i == u.Id));            
        }

        public async Task<StockDao> Add(StockDao stock) {
            // TODO Dette bør gøres i en procedure, fordi man kan bruge SQL-merge konstruktionen.
            // I SQL-merge kan man indsætte manglende exchange og valuta. Eller man
            // kan opdatere aktie oplysninger.
            stock.ExchangeId = GetExchangeId(stock);
            stock.CurrencyId = GetCurrencyId(stock);
            // Reset exchange and currency in stock, so EF doesn't create new exchange and currency entries
            stock.Exchange = null;
            stock.Currency = null;
            db.Add(stock);
            await db.SaveChangesAsync();
            return stock;
        }

        private int GetExchangeId(StockDao stock) {
            return db.Exchanges.Where(e=>
                ((!(stock.Exchange==null || string.IsNullOrEmpty(stock.Exchange.Name))) && e.Name == stock.Exchange.Name)
                ||
                ( (stock.Exchange == null || string.IsNullOrEmpty(stock.Exchange.Name)) && e.Id == stock.ExchangeId)
                ).First().Id;
        }

        private int GetCurrencyId(StockDao stock) {
            return db.Currencies.Where(c =>
                 ((!(stock.Currency == null || string.IsNullOrEmpty(stock.Currency.Code))) && c.Code == stock.Currency.Code)
                 ||
                 ((stock.Currency == null || string.IsNullOrEmpty(stock.Currency.Code)) && c.Id == stock.CurrencyId)
                 ).First().Id;
        }

        public async Task<bool> DeleteStock(int id) {
            var stock = db.Stocks.Where<StockDao>(s => s.Id == id).First();

            bool result = stock != null;
            if (result) {
                db.Remove(stock);
                await db.SaveChangesAsync();
            }

            return result;
        }

        public async Task<bool> DeleteStock(string isin) {
            var stock = db.Stocks.Where<StockDao>(s => s.Isin == isin).First();

            bool result = stock != null;
            if (result) {
                db.Remove(stock);
                await db.SaveChangesAsync();
            }

            return result;
        }

        public async Task<int> AddOrUpdateTimeSerieAddFacts(string isin, TimeSerieDao timeSerie) {
            // TODO Dette bør gøres i en procedure, fordi man kan bruge SQL-merge konstruktionen.
            // I SQL-merge kan man indsætte manglende datoer, manglende tidsserie og fakta. Eller man
            // kan opdatere pris og volume på eksisterende tidsserie-fakta.
            timeSerie.StockId = db.Stocks.Where<StockDao>(s => s.Isin == isin).First().Id;
            int timeSerieId = GetOrAddTimeSerieId(timeSerie);
            foreach (var fact in timeSerie.Facts) {
                fact.TimeSerieId = timeSerieId;
                fact.DateId = GetOrAddDateId(fact);
                db.Add(fact);
            }

            return await db.SaveChangesAsync();
        }

        // public for unit test
        public int GetOrAddDateId(TimeSerieFactDao fact) {
            if (db.Dates.Any(d => d.DateTimeUtc == fact.Date.DateTimeUtc)) {
                fact.DateId = db.Dates.Where(d => d.DateTimeUtc == fact.Date.DateTimeUtc).First().Id;
                // Reset Date to prevent EF to add existing dates
                fact.Date = null;
            }
            // TODO det gør EF helt af sig selv
            //else {
            //    // Date is missing
            //    db.Add(fact.Date);                
            //}
            return fact.Date.Id;
        }

        // public for unit test
        public int GetOrAddTimeSerieId(TimeSerieDao timeSerie) {
            if (db.TimeSeries.Any(t => t.StockId == timeSerie.StockId && t.Name == timeSerie.Name)) {
                // TimeSerie exist, get id
                timeSerie.Id = db.TimeSeries.Where<TimeSerieDao>(t =>
                            t.StockId == timeSerie.StockId && t.Name == timeSerie.Name).First().Id;
            }
            else {
                // TimeSerie doesn't exist, add it
                db.Add(timeSerie);
            }
            return timeSerie.Id;
        }

        public DateDao GetDate(int id) {
            return db.Dates.Where<DateDao>(d => d.Id == id).First();
        }
    }
}
