﻿using ElisBackend.Core.Domain.Abstractions;

namespace ElisBackend.Core.Domain.Entities;

public class StockTimeSeries(string name, string isin, TimeSeries timeSeries) : IStockTimeSeries
{
    public string Name { get; private set; } = name;
    public string Isin { get; private set; } = isin;
    public ITimeSeries TimeSeries { get; private set; } = timeSeries;
}