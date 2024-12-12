using ElisBackend.Core.Domain.Abstractions;

namespace ElisBackend.Core.Domain.Entities;

public class TimeSeries(string name, string isin, List<ITimeSeriesData> timeSerieData) : ITimeSeries
{
    public string Name { get; private set; } = name;
    public string Isin { get; private set; } = isin;
    public List<ITimeSeriesData> TimeSeriesData { get; private set; } = timeSerieData;
}