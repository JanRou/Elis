using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElisBackend.Gateways.Repositories.Daos {
    public class StockDao {
        public StockDao(string name, string isin, int exchangeId, int currencyId)
        {
            Name = name;
            Isin = isin;
            ExchangeId = exchangeId;
            CurrencyId = currencyId;
        }

        public int Id { get; set; } 
        public string Name { get; set; }
        public string Isin { get; set; }

        [ForeignKey("Exchange")]
        public int ExchangeId { get; set; } // => Required foreign key prop
        public virtual ExchangeDao Exchange { get; set; }

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; } // => Required foreign key prop
        public virtual CurrencyDao Currency { get; set; }
    }
}
