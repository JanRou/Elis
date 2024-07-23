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
            Field<StocksType>("stocks").Resolve( ctx => new {});
            Field<ExchangesType>("exchanges").Resolve(ctx => new { });
            Field<CurrenciesType>("currencies").Resolve(ctx => new { });
        }
    }
}
