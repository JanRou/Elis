using ElisBackend.Core.Domain.Abstractions;

namespace ElisBackend.Core.Domain.Entities;

public class TimeSeriesFact(DateTime date, decimal price, decimal volume) : ITimeSeriesFact
{
    public DateTime Date { get; set; } = date;
    public decimal Price { get; set; } = price;
    public decimal Volume { get; set; } = volume;
}
