using ElisBackend.Domain.Abstractions;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace ElisBackend.Domain.Entities;

// Aktie beskrivelse
public class Stock(string name, string isin, string exchangeUrl, string currency) : IStock
{
    public string Name { get; private set; } = name;
    public string Isin { get; private set; } = isin;
    public string ExchangeUrl { get; private set; } = exchangeUrl;
    public string Currency { get; private set; } = currency;
}
