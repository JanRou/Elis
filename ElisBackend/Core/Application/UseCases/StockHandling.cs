using AutoMapper;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities;
using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Gateways.Repositories.Daos;
using ElisBackend.Gateways.Repositories.Stock;

namespace ElisBackend.Core.Application.UseCases {

    public interface IStockHandling
    {
        Task<IEnumerable<IStock>> Get(FilterStock filter);
        Task<IStock> AddData(IStock stock, TimeSerie timeSerie);
        Task<IStock> Add(IStock stock);
        Task<bool> Delete(int id);
        Task<bool> Delete(string isin);
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


        public async Task<IStock> AddData(IStock stock, TimeSerie timeSerie) {
            // TODO brug repository til at tilføje tidsserie aktien
            return stock;
        }

        public async Task<bool> Delete(int id) {
            return await repository.DeleteStock(id);
        }

        public async Task<bool> Delete(string isin) {
            return await repository.DeleteStock(isin);
        }

    }
}
