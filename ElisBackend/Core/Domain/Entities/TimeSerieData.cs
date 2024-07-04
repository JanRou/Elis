namespace ElisBackend.Core.Domain.Entities;

public interface ITimeSerieData
{
    DateTime Date { get; }
    decimal Price { get; }
    decimal Volume { get; }
}

public class TimeSerieData(DateTime date, decimal price, decimal volume) : ITimeSerieData
{
    public DateTime Date { get; private set; } = date;
    public decimal Price { get; private set; } = price;
    public decimal Volume { get; private set; } = volume;
}
