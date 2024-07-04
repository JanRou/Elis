using ElisBackend.Core.Domain.Abstractions;
using GraphQL.Types;

namespace ElisBackend.Presenters.GraphQLSchema
{
    public class StockType : ObjectGraphType<IStock> {
        public StockType() {
            Description = "Basic stock information";
            Field(f => f.Name).Description("The name of the stock");
            Field(f => f.Isin).Description("The ISIN code for the stock");
            Field(f => f.Exchange).Description("The exchange for the stock");
            Field(f => f.Currency).Description("The currency for the stock");
            // TODO time series of prices
            //Field<ListGraphType<StockPriceSeriesType>>("priceseries")
            //    .Argument<NonNullGraphType<StringGraphType>>("periodeStart")
            //    .Argument<NonNullGraphType<StringGraphType>>("periodeEnd")
            //    .ResolveAsync( async ctx => {
            //    });
        }
    }
}


