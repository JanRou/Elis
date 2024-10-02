using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElisBackend.Gateways.Repositories.Daos {
    public class TimeSerieFactDao {
        [ForeignKey("TimeSerieId")]
        public virtual TimeSerieDao TimeSerie { get; set; }
        [Required]
        public int TimeSerieId { get; set; } = 0;

        // Date is the day for the closing price and volume
        [ForeignKey("DateId")]
        public virtual DateDao Date { get; set; }
        [Required]
        public int DateId { get; set; } = 0;

        // Facts
        public decimal Price { get; set; }
        public decimal Volume { get; set; }
    }
}
