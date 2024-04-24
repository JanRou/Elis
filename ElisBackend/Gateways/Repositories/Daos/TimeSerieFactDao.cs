using System.ComponentModel.DataAnnotations.Schema;

namespace ElisBackend.Gateways.Repositories.Daos {
    public class TimeSerieFactDao(int timeSerieId, int stockId, int dateId, decimal price, decimal volume) {

        [ForeignKey("TimeSerie")]
        public int TimeSerieId { get; set; } = timeSerieId;
        public virtual TimeSerieDao TimeSerie { get; set; }

        [ForeignKey("Stock")]
        public int StockId { get; set; } = stockId;
        public virtual StockDao Stock { get; set; }

        [ForeignKey("Date")]
        public int DateId { get; set; } = dateId;
        public virtual DateDao Date { get; set; }

        public decimal Price { get; set; } = price;
        public decimal Volume { get; set; } = volume;

    }
}
