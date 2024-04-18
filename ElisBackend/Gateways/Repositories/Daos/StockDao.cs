using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElisBackend.Gateways.Repositories.Daos {
    public class StockDao(string name, string isin, int exchangeId, int currencyId) {

        public int Id { get; set; }
        public string Name { get; set; } = name;
        public string Isin { get; set; } = isin;

        [ForeignKey("Exchange")]
        public int ExchangeId { get; set; } = exchangeId;
        public virtual ExchangeDao Exchange { get; set; }

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; } = currencyId;
        public virtual CurrencyDao Currency { get; set; }
    }

    public class StockSearchResultDao {
        public int Id { get; set; }
    } 
}
