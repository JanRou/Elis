namespace ElisBackend.Gateways.Repositories.Daos {
    public class StockDao {
        public StockDao(string name, string isin, string exchangeUrl)
        {
            Name = name;
            Isin = isin;
            ExchangeUrl = exchangeUrl;
        }
        public string Name { get; set; }
        public string Isin { get; set; }
        public string ExchangeUrl { get; set; }
    }
}
