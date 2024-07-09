using ElisBackend.Core.Application.Command;
using ElisBackend.Core.Application.Queries;
using ElisBackend.Core.Application.UseCases;
using ElisBackend.Core.Domain.Entities;
using GraphQL;
using GraphQL.Types;
using MediatR;

namespace ElisBackend.Presenters.GraphQLSchema {
    public class StocksQuery : ObjectGraphType {
        public StocksQuery() {
            Description = "Stock information";
            Field<ListGraphType<StockType>>("stocks")
                .Argument<StringGraphType>("isin")
                .Argument<StringGraphType>("name")
                .Argument<StringGraphType>("exchange")
                .Argument<StringGraphType>("currency")
                .ResolveAsync( async context => {
                    // TODO puha det er slemt det her, der må ikke være en reference ind mod domain :(
                    var filter = new FilterStock() { 
                        Isin = context.GetArgument(Name = "isin", defaultValue: ""),
                        Name = context.GetArgument(Name = "name", defaultValue: ""),
                        ExchangeName = context.GetArgument(Name = "exchange", defaultValue: ""),
                        CurrencyCode = context.GetArgument(Name = "currency", defaultValue: ""),
                    };
                    var mediator = context.RequestServices.GetService<IMediator>();
                    return await mediator.Send(new GetStocks(filter));
                }
            );
        }
    }

    public class StockMutation : ObjectGraphType {
        public StockMutation() {
            Description = "Stock information";
            Field<StockType>("createStock")
                .Argument<NonNullGraphType<StringGraphType>>("isin")
                .Argument<NonNullGraphType<StringGraphType>>("name")
                .Argument<NonNullGraphType<StringGraphType>>("exchange")
                .Argument<NonNullGraphType<StringGraphType>>("currency")
                .ResolveAsync(async context => {
                    // TODO puha det er slemt det her, der må ikke være en reference ind mod domain :(
                    //var stock = new Stock(  
                    //      context.GetArgument(Name = "name", defaultValue: "")
                    //    , context.GetArgument(Name = "isin", defaultValue: "")
                    //    , context.GetArgument(Name = "exchange", defaultValue: null)
                    //    , context.GetArgument(Name = "currency", defaultValue: null)
                    //    );
                    var mediator = context.RequestServices.GetService<IMediator>();
                    return await mediator.Send( new AddStock(/* TODO stock */));
                }
            );

        }
    }
}
