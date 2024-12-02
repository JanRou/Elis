using ElisBackend.Core.Domain.Abstractions;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace ElisBackend.Core.Domain.Entities;

// Aktie beskrivelse
public class Stock(string name, string isin, string instrumentCode, Exchange exchange, Currency currency) : IStock
{
    public string Name { get; private set; } = name;
    public string Isin { get; private set; } = isin;
    public string InstrumentCode { get; private set; } = instrumentCode;
    public IExchange Exchange { get; private set; } = exchange;
    public ICurrency Currency { get; private set; } = currency;
}
