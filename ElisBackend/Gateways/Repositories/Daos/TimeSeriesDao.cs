using System.ComponentModel.DataAnnotations.Schema;

namespace ElisBackend.Gateways.Repositories.Daos {
    public class TimeSeriesDao {

        // Primary key
        public int Id { get; set; } = 0;

        // Name is part of index with stockId
        public string Name { get; set; }

        [ForeignKey("StockId")]
        public virtual StockDao Stock { get; set; }
        public int StockId { get; set; } = 0;

        // Facts for this timeseries for navigation
        public IEnumerable<TimeSeriesFactDao> Facts { get; set; } = new List<TimeSeriesFactDao>();
    }
}
