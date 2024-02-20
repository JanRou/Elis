using ElisBackend.Application.UseCases;
using ElisBackend.Domain.Abstractions;
using ElisBackend.Gateways.Repositories.Daos;
using System.Linq;

namespace ElisBackend.Gateways.Repositories.Stock {

    public interface IStockRepository {
        Task<IEnumerable<StockDao>> Get(StockFilter filter);

    }

    public class StockRepository : IStockRepository {

        // TODO public StockRepository( ConnectionString ...) {        }

        public async Task<IEnumerable<StockDao>> Get(StockFilter filter) {
            var stocks = CreateStocks();
            // TODO with Entity Framework for PostgreSQL database
            return stocks.Where<StockDao>(
                    s =>   (string.IsNullOrEmpty(filter.Name) 
                                || ( !string.IsNullOrEmpty(filter.Name) && s.Name.Contains(filter.Name) ) )
                        && (string.IsNullOrEmpty(filter.Isin) 
                                || (!string.IsNullOrEmpty(filter.Isin) && s.Isin.Contains(filter.Isin) ))
                        && (string.IsNullOrEmpty(filter.ExchangeUrl) 
                                || (string.IsNullOrEmpty(filter.ExchangeUrl) && s.ExchangeUrl.Contains(filter.ExchangeUrl) ))
                );
        }

        private IEnumerable<StockDao> CreateStocks() {
            return new List<StockDao> {
                new StockDao( "Novo Nordisk B", "DK0062498333", "https://www.nasdaqomxnordic.com/"),
                new StockDao( "Rockwool A/S ser. B", "DK0010219153", "https://www.nasdaqomxnordic.com/"),
                new StockDao( "DSV A/S", "DK0060079531", "https://www.nasdaqomxnordic.com/"),
                new StockDao( "ALK-Abelló B A/S", "DK0061802139", "https://www.nasdaqomxnordic.com/"),
                new StockDao( "Schouw & Co. A/S", "DK0010253921", "https://www.nasdaqomxnordic.com/"),
                new StockDao( "GN Store Nord A/S", "DK0010272632", "https://www.nasdaqomxnordic.com/"),
                new StockDao( "Novozymes B A/S", "DK0060336014", "https://www.nasdaqomxnordic.com/"),
                new StockDao( "iShares Core S&P 500 UCITS ETF USD (Acc)", "IE00B5BMR087", "https://www.xetra.com/"),
                new StockDao( "iShares STOXX Europe 600 Technology UCITS ETF (DE)", "DE000A0H08Q4", "https://www.xetra.com/")
            };
        }

    }
}
