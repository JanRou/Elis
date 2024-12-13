﻿using ElisBackend.Core.Domain.Abstractions;
using GraphQL.Types;

namespace ElisBackend.Presenters.GraphQLSchema.Stock
{
    public class StockTimeSeriesType : ObjectGraphType<IStockTimeSeries>
    {
        public StockTimeSeriesType()
        {
            Description = "Basic stock information";
            Field(s => s.Name).Description("The name of the stock");
            Field(s => s.Isin).Description("The ISIN code for the stock");
            Field<ListGraphType<TimeSerieFactsType>>("TimeSerieFacts");
        }
    }
}

