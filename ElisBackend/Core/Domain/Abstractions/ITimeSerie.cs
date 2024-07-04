using ElisBackend.Core.Domain.Entities;

namespace ElisBackend.Core.Domain.Abstractions;

// Tidsserie, der kan indeholde aktiens handels priser, volume osv.

public interface ITimeSerie
{
    string Name { get; }
    int StockId { get; }
    List<ITimeSerieData> TimeSerieData { get; }
}
