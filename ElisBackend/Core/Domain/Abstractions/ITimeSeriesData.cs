namespace ElisBackend.Core.Domain.Abstractions;

public interface ITimeSeriesData
{
    DateTime Date { get; } // Date is in UTC
    decimal Price { get; }
    decimal Volume { get; } // Not required
}
