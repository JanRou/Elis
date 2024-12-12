using ElisBackend.Core.Application.Command;
using ElisBackend.Presenters.GraphQLSchema.Currency;
using ElisBackend.Presenters.GraphQLSchema.Exchange;
using ElisBackend.Presenters.GraphQLSchema.Stock;
using GraphQL.Types;
using MediatR;

namespace ElisBackend.Presenters.GraphQLSchema {
    public class ElisMutation : ObjectGraphType {
        public ElisMutation() {
            Description = "Create new stock, exchange or currency information";
            Field<StockMutationType>("stock").Resolve(ctx => new { });
            Field<ExchangeMutationType>("exchange").Resolve(ctx => new { });
            Field<CurrencyMutationType>("currency").Resolve(ctx => new { });

        }
    }
}
