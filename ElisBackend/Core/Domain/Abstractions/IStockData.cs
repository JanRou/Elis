namespace ElisBackend.Core.Domain.Abstractions
{
    public interface IStockData
    {
        string Name { get; }
        string Isin { get; }
        ITimeSeries TimeSeries { get; }
    }
}
