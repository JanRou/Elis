using ElisBackend.Core.Application.Command;
using GraphQL.Types;
using MediatR;

namespace ElisBackend.Presenters.GraphQLSchema {
    public class ElisMutation : ObjectGraphType {
        public ElisMutation() {
            Description = "Create new stock, exchange or currency information";
            Field<StockMutationType>("addstock").Resolve(ctx => new { });
            Field<ExchangeMutationType>("addexchange").Resolve(ctx => new { });
            //Field<CurrencyMutationType>("addcurrency").Resolve(ctx => new { });

        }
    }
}
