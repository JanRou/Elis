using System.Diagnostics.Metrics;
using System.Xml.Linq;
using System;
using ElisBackend.Core.Domain.Abstractions;

namespace ElisBackend.Core.Domain.Entities;

public class Exchange(string name, string country, string url) : IExchange
{
    public string Name { get; private set; } = name;
    public string Country { get; private set; } = country;
    public string Url { get; private set; } = url;
}
