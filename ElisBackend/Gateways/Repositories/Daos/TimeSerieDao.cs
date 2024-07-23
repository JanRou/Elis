using System.ComponentModel.DataAnnotations.Schema;

namespace ElisBackend.Gateways.Repositories.Daos {
    public class TimeSerieDao {
        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("Stock")]
        public int StockId { get; set; } = 0;
        public virtual StockDao Stock { get; set; }


        // Facts for this timeseries for navigation
        public IEnumerable<TimeSerieFactDao> Facts { get; set; } = new List<TimeSerieFactDao>();
    }
}
