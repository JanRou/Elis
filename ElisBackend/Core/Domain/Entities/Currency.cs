using ElisBackend.Core.Domain.Abstractions;

namespace ElisBackend.Core.Domain.Entities;

public class Currency(string name, string code) : ICurrency
{
    public string Name { get; private set; } = name;
    public string Code { get; private set; } = code;
}
