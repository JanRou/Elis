using ElisBackend.Core.Application.Dtos;
using GraphQL.Types;

namespace ElisBackend.Presenters.GraphQLSchema.Stock {
    public class StockDataInputType : InputObjectGraphType<StockDataIn> {
        public StockDataInputType() {
            Name = "StockDataInput";
            Field<NonNullGraphType<StringGraphType>>("isin").Description("Stock identification");
            Field<NonNullGraphType<StringGraphType>>("timeseriename").Description("Name of the timeseries usually 'PricesAndVolumes'");
        }
    }
}


