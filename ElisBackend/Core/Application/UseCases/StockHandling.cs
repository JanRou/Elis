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

    public class StockHandling : IStockHandling
    {
        private readonly IStockRepository repository;

        public StockHandling(IStockRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<IStock>> Get(FilterStock filter)
        {
            var result = await repository.Get(filter);
            return Map(result);
        }

        public async Task<bool> AddStock(IStock stock)
        {
            var stockDao = new StockDao(stock.Name, stock.Isin, 0, 0);
            stockDao.Exchange = new ExchangeDao() { // TODO vil blive erstattet af AutoMapper
                Name = stock.Exchange.Name
              , Country = stock.Exchange.Country
              , Url = stock.Exchange.Url
            };
            stockDao.Currency = new CurrencyDao()
            { // TODO vil blive erstattet af AutoMapper
                Name = stock.Currency.Name
              ,
                Code = stock.Currency.Code
            };
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

        private IEnumerable<IStock> Map(IEnumerable<StockDao> stockDaos)
        {
            List<IStock> result = new List<IStock>();
            foreach (var dao in stockDaos)
            {
                result.Add(Map(dao));
            }
            return result;
        }

        //// TODO installer og brug Automapper
        private IStock Map(StockDao stockDao) {
            return new Stock(stockDao.Name, stockDao.Isin, null, null); // Map(stockDao.Exchange), Map(stockDao.Currency));
        }

        //private IExchange Map(ExchangeDao exchangeDao)
        //{
        //    return new Exchange(exchangeDao.Name, exchangeDao.Country, exchangeDao.Url);
        //}

        //private ICurrency Map(CurrencyDao currencyDao)
        //{
        //    return new Currency(currencyDao.Name, currencyDao.Code);
        //}

    }

}
