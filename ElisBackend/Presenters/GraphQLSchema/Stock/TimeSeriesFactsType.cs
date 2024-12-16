using ElisBackend.Core.Domain.Abstractions;
using GraphQL.Types;
using System.Globalization;

namespace ElisBackend.Presenters.GraphQLSchema.Stock
{
    public class TimeSeriesFactsType : ObjectGraphType<ITimeSeriesFact>
    {
        public TimeSeriesFactsType()
        {
            Description = "The time serie facts for the stock";
            Field(f => f.Date).Description("Date in UTC ISO 8601 format: '2024-07-24T00:00:00.00000Z'");
            Field(f => f.Price).Description("Price as decimal");
            Field(f => f.Volume).Description("Volume as decimal");
        }
    }
}


