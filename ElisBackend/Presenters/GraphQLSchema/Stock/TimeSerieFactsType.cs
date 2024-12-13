using ElisBackend.Core.Domain.Abstractions;
using GraphQL.Types;

namespace ElisBackend.Presenters.GraphQLSchema.Stock
{
    public class TimeSerieFactsType : ObjectGraphType<ITimeSeriesFact>
    {
        public TimeSerieFactsType()
        {
            Description = "The time serie facts for the stock";
            Field<NonNullGraphType<StringGraphType>>("date")
                .Description("Date in UTC ISO 8601 format: '2024-07-24T00:00:00.00000Z'");
            Field<NonNullGraphType<DecimalGraphType>>("price").Description("Price as decimal");
            Field<NonNullGraphType<DecimalGraphType>>("volume").Description("Volume as decimal");
        }
    }
}


