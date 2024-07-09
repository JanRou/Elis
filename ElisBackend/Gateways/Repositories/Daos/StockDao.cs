using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElisBackend.Gateways.Repositories.Daos {
    public class StockDao {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Isin { get; set; }

        [ForeignKey("Exchange")]
        public int ExchangeId { get; set; } = 0;
        public virtual ExchangeDao Exchange { get; set; }

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; } = 0;
        public virtual CurrencyDao Currency { get; set; }

        // TimeSeriesFacts for this stock for navigation
        public IEnumerable<TimeSerieFactDao> TimeSeriesFacts { get; set; } = new List<TimeSerieFactDao>();

    }
}
