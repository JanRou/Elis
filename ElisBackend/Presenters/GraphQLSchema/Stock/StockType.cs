using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities;
using GraphQL.Types;

namespace ElisBackend.Presenters.GraphQLSchema.Stock {
    public class StockType : ObjectGraphType<IStock> {
        public StockType() {
            Description = "Basic stock information";
            Field(s => s.Name).Description("The name of the stock");
            Field(s => s.Isin).Description("The ISIN code for the stock");
            Field(s => s.InstrumentCode).Description("The instrument code for the stock");
            Field("exchange", s => s.Exchange.Name).Description("The exchange name for the stock");
            Field("currency", s => s.Currency.Code).Description("The currency code for the stock");
        }
    }
}


