using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Gateways.Dtos;

namespace ElisBackend.Gateways.Exchanges
{

    // TODO
    // API til at hente aktiedata fra børserne. Der kan være forskellige underliggende
    // api'er til hver børs, som bliver valgt på grundlag af aktiens ExchangeURL.
    public interface IExchangeApi {
        Task<TimeSerieDto> GetStockData(string stockName, string isin);
    }

    public interface IExchangeService {
        void Register(string url, IExchangeApi exchangeApi);
        void Unregister(string url);
        Task<TimeSerieDto> GetStockData(IStock stock);
    }

    public class ExchangeService : IExchangeService {

        private Dictionary<string, IExchangeApi> _exchanges = new Dictionary<string, IExchangeApi>();

        // TODO concurrency?
        public void Register(string name, IExchangeApi exchangeApi) {
            _exchanges.Add(name, exchangeApi);
        }
        public void Unregister(string name) {
            _exchanges.Remove(name);
        }

        public async Task<TimeSerieDto> GetStockData(IStock stock) {           
             return await _exchanges[stock.Exchange.Name].GetStockData(stock.Name, stock.Isin);
        }
    }


}
