using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElisBackend.Gateways.Repositories.Daos {
    public class StockDao {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Isin { get; set; }
        public string InstrumentCode { get; set; }

        [ForeignKey("Exchange")]
        public int ExchangeId { get; set; } = 0;
        public virtual ExchangeDao Exchange { get; set; }

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; } = 0;
        public virtual CurrencyDao Currency { get; set; }

        // TimeSeries for this stock for navigation
        public IEnumerable<TimeSerieDao> TimeSeries { get; set; } = new List<TimeSerieDao>();

    }
}
