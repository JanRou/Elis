namespace ElisBackend.Core.Domain.Abstractions
{
    public interface IStockTimeSeries
    {
        string Name { get; }
        string Isin { get; }
        ITimeSeries TimeSeries { get; }
    }
}
