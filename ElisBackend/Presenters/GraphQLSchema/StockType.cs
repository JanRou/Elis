using ElisBackend.Core.Domain.Abstractions;
using GraphQL.Types;

namespace ElisBackend.Presenters.GraphQLSchema
{
    public class StockType : ObjectGraphType<IStock> {
        public StockType() {
            Description = "Basic stock information";
            Field(f => f.Name).Description("The name of the stock");
            Field(f => f.Isin).Description("The ISIN code for the stock");
            Field("exchange", f => f.Exchange.Name).Description("The exchange name for the stock");
            Field("currency", f => f.Currency.Code).Description("The currency code for the stock");
            // TODO Tilføj nested typer for børs (exchange) og valuta (currency)
            //Field<ExchangeType>("exchange", f => f.Exchange.Name).Description("The exchange name for the stock");
            //Field<CurrencyType>("currency", f => f.Currency).Description("The currency code for the stock");

            // TODO time series of prices
            //Field<ListGraphType<StockPriceSeriesType>>("priceseries")
            //    .Argument<NonNullGraphType<StringGraphType>>("periodeStart")
            //    .Argument<NonNullGraphType<StringGraphType>>("periodeEnd")
            //    .ResolveAsync( async ctx => {
            //    });
        }
    }
}


