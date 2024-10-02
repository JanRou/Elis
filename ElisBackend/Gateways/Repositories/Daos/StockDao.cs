using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElisBackend.Gateways.Repositories.Daos {
    public class StockDao {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Isin { get; set; }
        public string InstrumentCode { get; set; }

        [ForeignKey("ExchangeId")]
        public virtual ExchangeDao Exchange { get; set; }
        public int ExchangeId { get; set; } = 0;

        [ForeignKey("CurrencyId")]
        public virtual CurrencyDao Currency { get; set; }
        public int CurrencyId { get; set; } = 0;

        // TimeSeries for this stock for navigation
        public IEnumerable<TimeSerieDao> TimeSeries { get; set; } = new List<TimeSerieDao>();

    }
}
