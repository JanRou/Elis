namespace ElisBackend.Gateways.Repositories.Daos {

    public class DateDao {
        public int Id { get; set; }
        // The DateTimeUtc is the day for the closing price and volume
        public DateTime DateTimeUtc { get; set; }

        // Facts for this date for navigation
        public IEnumerable<TimeSerieFactDao> Facts { get; set; } = new List<TimeSerieFactDao>();

    }
}
