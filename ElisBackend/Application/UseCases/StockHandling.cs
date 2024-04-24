using ElisBackend.Domain.Abstractions;
using ElisBackend.Domain.Entities;
using ElisBackend.Gateways.Repositories.Daos;
using ElisBackend.Gateways.Repositories.Stock;

namespace ElisBackend.Application.UseCases {

    public interface IStockHandling {
        Task<IEnumerable<IStock>> Get(StockFilter filter);
        Task<bool> UpdateStocksData();
    }



    public class StockHandling : IStockHandling {
        private readonly IStockRepository repository;

        public StockHandling(IStockRepository repository) {
            this.repository = repository;
        }
        public async Task<IEnumerable<IStock>> Get(StockFilter filter) {
            var result =  await repository.Get(filter);
            return Map(result);
        }

        public async Task<bool> UpdateStocksData() {
            // 1. Hent aktier fra DB, som man henter nye data for
            var stocks = await repository.Get(new StockFilter());

            // 2. Hent data for hver aktie på listen
            foreach(var stock in stocks) {

            }
            return true;
        }

        private IEnumerable<IStock> Map( IEnumerable<StockDao> stockDaos) {
            List<IStock> result = new List<IStock>();
            foreach (var dao in stockDaos) {
                result.Add(Map(dao));
            }
            return result;
        }

        // TODO installer og brug Automapper
        private IStock Map(StockDao stockDao) {
            return new Stock(stockDao.Name, stockDao.Isin, stockDao.Exchange.ExchangeUrl, stockDao.Currency.Short);
        }
    }

}
