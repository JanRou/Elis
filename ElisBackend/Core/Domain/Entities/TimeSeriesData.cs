using ElisBackend.Core.Domain.Abstractions;

namespace ElisBackend.Core.Domain.Entities;

public class TimeSeriesData(DateTime date, decimal price, decimal volume) : ITimeSeriesData
{
    public DateTime Date { get; set; } = date;
    public decimal Price { get; set; } = price;
    public decimal Volume { get; set; } = volume;
}
