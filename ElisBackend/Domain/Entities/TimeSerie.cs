namespace ElisBackend.Domain.Entities;

// Tidsserie, der kan indeholde aktiens handels priser, volume osv.

public interface ITimeSerie {
    string Name { get; }
    int StockId { get; }
    List<ITimeSerieData> TimeSerieData { get; }
}

public class TimeSerie(string name, int stockId, List<ITimeSerieData> timeSerieData) : ITimeSerie {
    public string Name { get; private set; } = name;
    public int StockId { get; private set; } = stockId;
    public List<ITimeSerieData> TimeSerieData { get; private set; } = timeSerieData;
}
