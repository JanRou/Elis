using ElisBackend.Application.UseCases;
using ElisBackend.Domain.Abstractions;
using ElisBackend.Gateways.Dal;
using ElisBackend.Gateways.Repositories.Daos;
using System.Linq;

namespace ElisBackend.Gateways.Repositories.Stock {

    public interface IStockRepository {
        Task<IEnumerable<StockDao>> Get(StockFilter filter);
        Task<StockDao> Add(StockDao stock);
        Task<bool> Delete(int id);
    }

    public class StockRepository : IStockRepository {

        public StockRepository(ElisContext elisContext) {
            db = elisContext;
        }

        public ElisContext db { get; }

        public async Task<IEnumerable<StockDao>> Get(StockFilter filter) {

            return db.Stocks.Where<StockDao>(
                    s => (string.IsNullOrEmpty(filter.Name)
                                || (!string.IsNullOrEmpty(filter.Name) && s.Name.Contains(filter.Name)))
                        && (string.IsNullOrEmpty(filter.Isin)
                                || (!string.IsNullOrEmpty(filter.Isin) && s.Isin.Contains(filter.Isin)))
                        && (string.IsNullOrEmpty(filter.ExchangeUrl)
                                || (string.IsNullOrEmpty(filter.ExchangeUrl) && s.ExchangeUrl.Contains(filter.ExchangeUrl)))
                );
        }

        public async Task<StockDao> Add(StockDao stock) {
            db.Add(stock);
            await db.SaveChangesAsync();

            return stock;
        }

        public async Task<bool> Delete(int id) {
            var stock = db.Stocks.Where<StockDao>(s => s.Id == id).FirstOrDefault();

            bool result = stock != null;
            if (result) {
                db.Remove(stock);
                await db.SaveChangesAsync();
            }

            return result;
        }

    }
}
