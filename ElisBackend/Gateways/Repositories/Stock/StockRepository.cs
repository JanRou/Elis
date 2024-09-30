using ElisBackend.Core.Application.Queries;
using ElisBackend.Core.Domain.Abstractions;
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
        /// <returns>List of fact ids added</returns>
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
            // TODO Dette bør gøres i en procedure, fordi man kan bruge SQL-merge og
            // en tabel til at overføre hele tidsserien i et kald til databasen.
            // I SQL-merge kan man indsætte manglende datoer, manglende tidsserie og fakta. Eller man
            // kan opdatere pris og volume på eksisterende tidsserie og datoer.
            timeSerie.StockId = db.Stocks.Where<StockDao>(s => s.Isin == isin).First().Id;
            int timeSerieId = GetOrAddTimeSerieId(timeSerie);
            int result = 0;
            // TODO Linq?
            foreach (var fact in timeSerie.Facts) {
                fact.TimeSerieId = timeSerieId;
                SetDateId(fact);
                db.Add(fact);
                result++;
            }
            await db.SaveChangesAsync();

            return result;
        }

        private int GetDateId(DateTime date) {
            int result = 0;
            if (db.Dates.Any(d => d.DateTimeUtc == date)) {
                 result = db.Dates.Where(d => d.DateTimeUtc == date).First().Id;
            }

            return result;
        }

        private void SetDateId(TimeSerieFactDao fact) {
            int dateId = GetDateId(fact.Date.DateTimeUtc);
            if (dateId != 0) {
                fact.DateId = dateId;
                // Reset Date to prevent EF to add duplets of dates
                fact.Date = null;
            }
        }

        private int GetTimeSerieId(TimeSerieDao timeSerie) {
            int result = 0;
            if (db.TimeSeries.Any(t => t.StockId == timeSerie.StockId && t.Name == timeSerie.Name)) {
                // TimeSerie exist, get id
                result = db.TimeSeries.Where<TimeSerieDao>(t => 
                        t.StockId == timeSerie.StockId && t.Name == timeSerie.Name).First().Id;                
            }
            return result;
        }

        private int GetOrAddTimeSerieId(TimeSerieDao timeSerie) {
            int result = GetTimeSerieId(timeSerie);
            if (result == 0) { 
                // TimeSerie doesn't exist, add it
                db.Add(timeSerie);
                result =timeSerie.Id;  
            }
            return result;
        }

        // For unit test
        public DateDao GetDate(int id) {
            return db.Dates.Where<DateDao>(d => d.Id == id).First();
        }

        // To clean up after unit tests
        public async Task<bool> DeleteFacts(string isin, TimeSerieDao timeSerie) {
            bool result = false;
            timeSerie.StockId = db.Stocks.Where<StockDao>(s => s.Isin == isin).First().Id;
            int timeSerieId = GetTimeSerieId(timeSerie);
            result = timeSerieId != 0;
            if (result) {
                foreach (var fact in timeSerie.Facts) {
                    int dateId = GetDateId(fact.Date.DateTimeUtc);
                    result = dateId != 0;
                    if (!result) {
                        break;
                    }
                    var factToRemove = db.TimeSerieFacts.Where<TimeSerieFactDao>( f => 
                                                    f.TimeSerieId==timeSerieId && f.DateId==dateId).First();
                    db.Remove(factToRemove);
                }
            }

            if (result) {
                await db.SaveChangesAsync();
            }

            return result;
        }
    }
}
