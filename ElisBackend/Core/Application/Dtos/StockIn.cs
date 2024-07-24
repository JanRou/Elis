namespace ElisBackend.Core.Application.Dtos {
    public class StockIn(string name, string isin, string exchangeName, string currencyCode)
    {
        public string Name { get; private set; } = name;
        public string Isin { get; private set; } = isin;
        public string ExchangeName { get; private set; } = exchangeName;
        public string CurrencyCode { get; private set; } = currencyCode;
    }
}
