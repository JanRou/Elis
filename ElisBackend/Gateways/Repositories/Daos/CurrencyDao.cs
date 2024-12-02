namespace ElisBackend.Gateways.Repositories.Daos {
    public class CurrencyDao {        
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Code { get; set; }

        // Stocks traded in this currency for navigation
        public IEnumerable<StockDao> Stocks { get; set; } = new List<StockDao>();

    }
}
