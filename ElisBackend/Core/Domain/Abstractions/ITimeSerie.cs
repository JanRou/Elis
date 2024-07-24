using ElisBackend.Core.Domain.Entities;

namespace ElisBackend.Core.Domain.Abstractions;

// Tidsserie, der kan indeholde aktiens handels priser, volume osv.

public interface ITimeSerie
{
    string Name { get; }
    string Isin { get; }
    List<TimeSerieData> TimeSerieData { get; }
}
