using ElisBackend.Core.Application.Dtos;
using GraphQL.Types;

namespace ElisBackend.Presenters.GraphQLSchema.Stock {
    public class TimeSerieDataInput : InputObjectGraphType<TimeSerieDataIn> {
        public TimeSerieDataInput() {
            Name = "TimeSerieDataInput";
            Field<NonNullGraphType<StringGraphType>>("date")
                .Description("Date in UTC ISO 8601 format: '2024-07-24T00:00:00.000Z'");
            Field<NonNullGraphType<DecimalGraphType>>("price").Description("Price as decimal");
            Field<NonNullGraphType<DecimalGraphType>>("volume").Description("Volume as decimal");
        }
    }
}


