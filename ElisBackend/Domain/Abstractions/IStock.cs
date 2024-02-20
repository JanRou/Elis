namespace ElisBackend.Domain.Abstractions
{
    public interface IStock
    {
        string Name { get; }
        string Isin { get; }
        string ExchangeUrl { get; }
    }

}
