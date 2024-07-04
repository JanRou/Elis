namespace ElisBackend.Core.Domain.Abstractions;

public interface ICurrency
{
    public string Name { get; }
    public string Code { get; }
}