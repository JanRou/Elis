using ElisBackend.Core.Domain.Entities;

namespace ElisBackend.Core.Domain.Abstractions;

// TimeSeries that can hold a stock's proces, volumes etc.

public interface ITimeSeries
{
    string Name { get; }
    string Isin { get; }
    List<ITimeSeriesData> TimeSeriesData { get; }
}
