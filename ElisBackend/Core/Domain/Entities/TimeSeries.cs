﻿using ElisBackend.Core.Domain.Abstractions;

namespace ElisBackend.Core.Domain.Entities;

public class TimeSeries(string name, string isin, List<ITimeSeriesFact> timeSerieData) : ITimeSeries
{
    public string Name { get; private set; } = name;
    public string Isin { get; private set; } = isin;
    public List<ITimeSeriesFact> TimeSeriesData { get; private set; } = timeSerieData;
}