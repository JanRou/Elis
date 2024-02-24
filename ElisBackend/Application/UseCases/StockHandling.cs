using ElisBackend.Domain.Abstractions;
using ElisBackend.Domain.Entities;
using ElisBackend.Gateways.Repositories.Daos;
using ElisBackend.Gateways.Repositories.Stock;

namespace ElisBackend.Application.UseCases {

    // TODO der mangler skip og take til sideinddeling
    // TODO der mangler sortering
    public class StockFilter {
        public string Name { get; set; }
        public string Isin { get; set; }
        public string Currency { get; set; }
        public string ExchangeUrl { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
    }

    public interface IStockHandling {
        Task<IEnumerable<IStock>> Get(StockFilter filter);
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

        private IEnumerable<IStock> Map( IEnumerable<StockDao> stockDaos) {
            List<IStock> result = new List<IStock>();
            foreach (var dao in stockDaos) {
                result.Add(Map(dao));
            }
            return result;
        }

        private IStock Map(StockDao stockDao) {
            return new Stock(stockDao.Name, stockDao.Isin, stockDao.Exchange.ExchangeUrl);
        }

    }

}
