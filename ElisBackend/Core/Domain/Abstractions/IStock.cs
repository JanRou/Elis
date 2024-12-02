namespace ElisBackend.Core.Domain.Abstractions
{
    public interface IStock
    {
        string Name { get; }
        string Isin { get; }
        string InstrumentCode { get; }
        IExchange Exchange { get; }
        ICurrency Currency { get; }
    }

}
