namespace ElisBackend.Core.Domain.Abstractions;

public interface IExchange
{
    public string Name { get; }
    public string Country { get; }
    public string Url { get; }
}