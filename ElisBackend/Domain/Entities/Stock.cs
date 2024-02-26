using ElisBackend.Domain.Abstractions;

namespace ElisBackend.Domain.Entities;

public class Stock : IStock
{
    public Stock(string name, string isin, string exchangeUrl, string currency)
    {
        Name = name;
        Isin = isin;
        ExchangeUrl = exchangeUrl;
        Currency = currency;
    }

    public string Name { get; private set; }
    public string Isin { get; private set; }
    public string ExchangeUrl { get; private set; }
    public string Currency { get; private set; }
}
