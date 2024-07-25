namespace ElisBackend.Core.Domain.Entities;

public interface ITimeSerieData
{
    DateTime Date { get; } // Date is in UTC
    decimal Price { get; }
    decimal Volume { get; }
}

public class TimeSerieData(DateTime date, decimal price, decimal volume) : ITimeSerieData
{
    public DateTime Date { get; set; } = date;
    public decimal Price { get; set; } = price;
    public decimal Volume { get; set; } = volume;
}
