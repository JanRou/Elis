namespace ElisBackend.Gateways.Repositories.Daos {

    public class ExchangeDao {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Url { get; set; }

        // Stocks on this exchange for navigation
        public IEnumerable<StockDao> Stocks { get; set; } = new List<StockDao>();
    }
}
