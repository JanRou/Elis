using ElisBackend.Core.Domain.Abstractions;

namespace ElisBackend.Core.Domain.Entities;

public class TimeSeries(string name, string isin, List<TimeSerieData> timeSerieData) : ITimeSerie
{
    public string Name { get; private set; } = name;
    public string Isin { get; private set; } = isin;
    public List<TimeSerieData> TimeSerieData { get; private set; } = timeSerieData;
}