using AutoMapper;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities;
using ElisBackend.Gateways.Repositories.Daos;
using ElisBackend.Gateways.Repositories.Stock;

namespace ElisBackend.Core.Application.UseCases
{

    public interface IStockHandling
    {
        Task<IEnumerable<IStock>> Get(FilterStock filter);
        Task<bool> UpdateStocksData();
        Task<bool> AddStock(IStock stock);
    }

    public class StockHandling(IStockRepository repository, IMapper mapper) : IStockHandling
    {
        public async Task<IEnumerable<IStock>> Get(FilterStock filter)
        {
            var result = await repository.Get(filter);
            return mapper.Map<IEnumerable<Stock>>(result);
        }

        public async Task<bool> AddStock(IStock stock)
        {
            var stockDao = mapper.Map<StockDao>(stock);
            var addedStockDao = await repository.Add(stockDao);
            return addedStockDao.Id > 1;
        }


        public async Task<bool> UpdateStocksData()
        {
            // 1. Hent aktier fra DB, som man henter nye data for
            //var stocks = await repository.Get(new FilterStock());

            // 2. Hent data for hver aktie på listen
            //foreach (var stock in stocks)
            //{
            //    // TODO 
            //}
            return true;
        }

    }

}
