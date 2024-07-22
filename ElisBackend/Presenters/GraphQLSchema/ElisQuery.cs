using ElisBackend.Core.Application.Queries;
using ElisBackend.Core.Domain.Entities;
using ElisBackend.Core.Domain.Entities.Filters;
using GraphQL;
using GraphQL.Types;
using MediatR;

namespace ElisBackend.Presenters.GraphQLSchema {
    public class ElisQuery : ObjectGraphType {
        public ElisQuery() {
            Description = "Get stock, exchange and currency information";
            Field<StocksType>("getstocks").Resolve( ctx => new {});
            Field<ExchangesType>("getexchanges").Resolve(ctx => new { });
            Field<CurrenciesType>("getcurrencies").Resolve(ctx => new { });
        }
    }
}
