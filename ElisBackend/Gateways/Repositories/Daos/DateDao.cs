namespace ElisBackend.Gateways.Repositories.Daos {

    public class DateDao(DateTime dateTimeUtc) {
        public int Id { get; set; }
        public DateTime DateTimeUtc { get; set; } = dateTimeUtc;

        // Facts for this date for navigation
        public IEnumerable<TimeSerieFactDao> Facts { get; set; }

    }
}
