namespace ElisBackend.Core.Domain.Abstractions;

public interface ITimeSeriesFact
{
    DateTime Date { get; } // Date is in UTC
    decimal Price { get; }
    decimal Volume { get; } // Not required
}
