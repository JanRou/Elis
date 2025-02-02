﻿using ElisBackend.Core.Domain.Entities;

namespace ElisBackend.Core.Domain.Abstractions;

// TimeSeries that can hold a stock's price, volume.

public interface ITimeSeries
{
    string Name { get; } // Name of the time series like PricesAndVolumes, MACD, ...
    string Isin {  get; } // Id of the stock
    List<ITimeSeriesFact> TimeSeriesData { get; }
}
