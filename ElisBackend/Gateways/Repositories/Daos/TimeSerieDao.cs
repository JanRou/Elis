namespace ElisBackend.Gateways.Repositories.Daos {
    public class TimeSerieDao(string name) {
        public int Id { get; set; }
        public string Name { get; set; } = name;
        
        // Facts for this timeseries for navigation
        public IEnumerable<TimeSerieFactDao> Facts { get; set; }
    }
}
