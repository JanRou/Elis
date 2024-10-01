using ElisBackend.Core.Application.Queries;
using ElisBackend.Core.Domain.Entities.Filters;
using GraphQL;
using GraphQL.Types;
using MediatR;

namespace ElisBackend.Presenters.GraphQLSchema.Stock {
    public class StocksType : ObjectGraphType {
        public StocksType() {
            Field<ListGraphType<StockType>>("stocks")
                .Argument<StringGraphType>("isin")
                .Argument<StringGraphType>("name")
                .Argument<StringGraphType>("exchange")
                .Argument<StringGraphType>("currency")
                .Argument<IntGraphType>("take")
                .Argument<IntGraphType>("skip")
                .ResolveAsync(async context => {
                    var filter = new FilterStock() {
                        Isin = context.GetArgument(Name = "isin", defaultValue: ""),
                        Name = context.GetArgument(Name = "name", defaultValue: ""),
                        ExchangeName = context.GetArgument(Name = "exchange", defaultValue: ""),
                        CurrencyCode = context.GetArgument(Name = "currency", defaultValue: ""),
                        Take = context.GetArgument(Name = "take", defaultValue: 0),
                        Skip = context.GetArgument(Name = "skip", defaultValue: 0),
                    };
                    var mediator = context.RequestServices.GetService<IMediator>();

                    return await mediator.Send(new GetStocks(filter));
                });
        }
    }
}


