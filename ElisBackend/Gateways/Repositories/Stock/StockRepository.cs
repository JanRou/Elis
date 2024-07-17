using ElisBackend.Core.Domain.Entities;
using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Extensions;
using ElisBackend.Gateways.Dal;
using ElisBackend.Gateways.Repositories.Daos;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ElisBackend.Gateways.Repositories.Stock
{

    public interface IStockRepository {
        Task<IEnumerable<StockDao>> Get(FilterStock filter);
        Task<StockDao> Add(StockDao stock);
        Task<bool> DeleteStock(int id);
        
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
                .Include(c => c.Currency)
                .Include(e => e.Exchange)
                .ToList();

            // Sort by searchstocks result before returning
            return unsorted.OrderBy(u => stockIds.FindIndex( i => i == u.Id));
            
        }

        public async Task<StockDao> Add(StockDao stock) {
            db.Add(stock);
            await db.SaveChangesAsync();

            return stock;
        }

        public async Task<bool> DeleteStock(int id) {
            var stock = db.Stocks.Where<StockDao>(s => s.Id == id).FirstOrDefault();

            bool result = stock != null;
            if (result) {
                db.Remove(stock);
                await db.SaveChangesAsync();
            }

            return result;
        }

        public async Task<bool> DeleteExchange(int id) {
            var exchange = db.Exchanges.Where<ExchangeDao>(s => s.Id == id).FirstOrDefault();

            bool result = exchange != null;
            if (result) {
                db.Remove(exchange);
                await db.SaveChangesAsync();
            }

            return result;
        }

        public async Task<bool> DeleteCurrency(int id) {
            var currency = db.Currencies.Where<CurrencyDao>(s => s.Id == id).FirstOrDefault();

            bool result = currency != null;
            if (result) {
                db.Remove(currency);
                await db.SaveChangesAsync();
            }

            return result;
        }

        //public async Task<IEnumerable<TimeSerieFactDao>> AddStockData(int stockId, ) {


        //}
    }
}
