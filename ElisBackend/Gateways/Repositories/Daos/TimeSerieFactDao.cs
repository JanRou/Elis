using System.ComponentModel.DataAnnotations.Schema;

namespace ElisBackend.Gateways.Repositories.Daos {
    public class TimeSerieFactDao {
        [ForeignKey("TimeSerie")]
        public int TimeSerieId { get; set; }
        public virtual TimeSerieDao TimeSerie { get; set; }

        // Date is the day for the closing price and volume
        [ForeignKey("Date")]
        public int DateId { get; set; }
        public virtual DateDao Date { get; set; }

        // Fact
        public decimal Price { get; set; }
        public decimal Volume { get; set; }
    }
}
