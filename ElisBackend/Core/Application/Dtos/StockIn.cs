namespace ElisBackend.Core.Application.Dtos {
    public class StockIn(string name, string isin, string exchangeName, string currencyCode)
    {
        public string Name { get; set; } = name;
        public string Isin { get; set; } = isin;
        public string ExchangeName { get; set; } = exchangeName;
        public string CurrencyCode { get; set; } = currencyCode;
    }
}
