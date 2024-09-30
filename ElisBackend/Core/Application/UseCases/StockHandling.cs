using AutoMapper;
using ElisBackend.Core.Application.Dtos;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities;
using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Gateways.Repositories.Daos;
using ElisBackend.Gateways.Repositories.Stock;

namespace ElisBackend.Core.Application.UseCases {

    public interface IStockHandling
    {
        Task<IEnumerable<IStock>> Get(FilterStock filter);
        Task<IStock> Add(IStock stock);
        Task<bool> Delete(int id);
        Task<bool> Delete(string isin);
        Task<StockDataOut> AddData(TimeSerie timeSerie);
    }

    public class StockHandling(IStockRepository repository, IMapper mapper) : IStockHandling {
        public async Task<IEnumerable<IStock>> Get(FilterStock filter) {
            var result = await repository.Get(filter);
            return mapper.Map<IEnumerable<Stock>>(result);
        }

        public async Task<IStock> Add(IStock stock) {
            var stockDao = mapper.Map<StockDao>(stock);
            var addedStockDao = await repository.Add(stockDao);
            return mapper.Map<Stock>(addedStockDao);
        }

        // TODO overvej brugen af StockDataOut. Er navnet ok? Det kunne være TimeSerieOut.
        // Burde StockDataOut være en core entity?
        public async Task<StockDataOut> AddData(TimeSerie timeSerie) {
            var timeSerieDao = mapper.Map<TimeSerieDao>(timeSerie);
            int result = await repository.AddOrUpdateTimeSerieAddFacts( timeSerie.Isin, timeSerieDao);
            return new StockDataOut( timeSerie.Isin, timeSerie.Name, result);
        }

        public async Task<bool> Delete(int id) {
            return await repository.DeleteStock(id);
        }

        public async Task<bool> Delete(string isin) {
            return await repository.DeleteStock(isin);
        }
    }
}
