using ElisBackend.Domain.Abstractions;

namespace ElisBackend.Domain.Entities;

public class Stock : IStock
{
    public Stock(string name, string isin, string exchangeUrl)
    {
        Name = name;
        Isin = isin;
        ExchangeUrl = exchangeUrl;
    }

    public string Name { get; private set; }
    public string Isin { get; private set; }
    public string ExchangeUrl { get; private set; }
}
