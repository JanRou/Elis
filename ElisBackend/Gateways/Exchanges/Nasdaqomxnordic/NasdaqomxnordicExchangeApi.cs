using ElisBackend.Gateways.Dtos;

namespace ElisBackend.Gateways.Exchanges.Nasdaqomxnordic {
    public class NasdaqomxnordicExchangeApi : IExchangeApi {

        public async Task<TimeSerieDto> GetStockData(string stockName, string isin) {
            // TODO
            return null;
        }
    }
}
