using ElisBackend.Core.Application.Queries;
using ElisBackend.Core.Domain.Entities;
using ElisBackend.Core.Domain.Entities.Filters;
using GraphQL;
using GraphQL.Types;
using MediatR;

namespace ElisBackend.Presenters.GraphQLSchema {
    public class StocksQuery : ObjectGraphType {
        public StocksQuery() {
            Description = "Stock, exchange and currency information";

            Field<ListGraphType<StockType>>("stocks")
                .Argument<StringGraphType>("isin")
                .Argument<StringGraphType>("name")
                .Argument<StringGraphType>("exchange")
                .Argument<StringGraphType>("currency")
                .Argument<IntGraphType>("take")
                .Argument<IntGraphType>("skip")
                .ResolveAsync(async context => {
                    // TODO FilterX er ikke godt at bruge her
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
                }
            );

            //Field<ListGraphType<ExchangeType>>("exchanges")
            //    .Argument<StringGraphType>("name")
            //    .Argument<StringGraphType>("exchange")
            //    .Argument<StringGraphType>("url")
            //    .ResolveAsync(async context => {
            //        // TODO FilterX er ikke godt at bruge her
            //        var filter = new FilterExchange() {
            //            Name = context.GetArgument(Name = "name", defaultValue: ""),
            //            Country = context.GetArgument(Name = "country", defaultValue: ""),
            //            Url = context.GetArgument(Name = "url", defaultValue: ""),
            //        };
            //        var mediator = context.RequestServices.GetService<IMediator>();
            //        return await mediator.Send(new GetExchanges(filter));
            //    }
            //);

            //Field<ListGraphType<CurrencyType>>("currencies")
            //    .Argument<StringGraphType>("name")
            //    .Argument<StringGraphType>("code")
            //    .ResolveAsync(async context => {
            //        // TODO FilterX er ikke godt at bruge her
            //        var filter = new FilterCurrency() {
            //            Name = context.GetArgument(Name = "name", defaultValue: ""),
            //            Code = context.GetArgument(Name = "code", defaultValue: ""),
            //        };
            //        var mediator = context.RequestServices.GetService<IMediator>();
            //        return await mediator.Send(new GetCurrencies(filter));
            //    }
            //);

        }
    }
}
