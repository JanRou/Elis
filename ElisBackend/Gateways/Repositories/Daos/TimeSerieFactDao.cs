using System.ComponentModel.DataAnnotations.Schema;

namespace ElisBackend.Gateways.Repositories.Daos {
    public class TimeSerieFactDao(int timeSerieId, int dateId, decimal price, decimal volume) {

        [ForeignKey("TimeSerie")]
        public int TimeSerieId { get; set; } = timeSerieId;
        public virtual TimeSerieDao TimeSerie { get; set; }

        // Date is the day for the closing price and volume
        [ForeignKey("Date")]
        public int DateId { get; set; } = dateId;
        public virtual DateDao Date { get; set; }

        public decimal Price { get; set; } = price;
        public decimal Volume { get; set; } = volume;

    }
}
