﻿using ElisBackend.Core.Domain.Abstractions;
using GraphQL.Types;

namespace ElisBackend.Presenters.GraphQLSchema.Stock
{
    public class TimeSeriesType : ObjectGraphType<ITimeSeries>
    {
        public TimeSeriesType()
        {
            Description = "Time series with facts";
            Field(s => s.Isin).Description("The ISIN code for the stock");
            Field(s => s.Name).Description("The name of the time series");
            Field<ListGraphType<TimeSeriesFactsType>>("timeSeriesData").Description("List of facts");
        }
    }
}


