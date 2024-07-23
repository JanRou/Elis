using ElisBackend.Core.Domain.Abstractions;

namespace ElisBackend.Core.Domain.Entities;

public class TimeSerie(string name, int stockId, List<ITimeSerieData> timeSerieData) : ITimeSerie
{
    public string Name { get; private set; } = name;
    public int StockId { get; private set; } = stockId;
    public List<ITimeSerieData> TimeSerieData { get; private set; } = timeSerieData;
}