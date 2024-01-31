using ElisBackend.Application.Queries;
using ElisBackend.Application.UseCases;
using ElisBackend.Domain.Abstractions;
using ElisBackend.Presenters.Dtos;
using GraphQL;
using GraphQL.Types;
using MediatR;

namespace ElisBackend.Presenters.GraphQLSchema {

    // Check schema with playground http://localhost:<port>/
    public class ElisSchema : Schema {
        public ElisSchema() : base() {
            Query = new StockQuery();
            //Query = resolver.GetRequiredService<StockQuery>();
            //Mutation = resolver.GetRequiredService<StockMutation>();
            //Subscription = resolver.GetRequiredService<StockSubscription>();
        }
    }

    public class StockQuery : ObjectGraphType {
        public StockQuery() {
            Description = "Stock information";
            Field<ListGraphType<StockType>>("stock")
                .Argument<StringGraphType>( "exchangeurl" )
                .Argument<StringGraphType>("isin" )
                .Argument<StringGraphType>( "name" )
                .ResolveAsync( async context => {
                    var filter = new StockFilter() {
                        ExchangeUrl = context.GetArgument(Name = "exchangeurl", defaultValue: ""),
                        Isin = context.GetArgument(Name = "isin", defaultValue: ""),
                        Name = context.GetArgument(Name = "name", defaultValue: ""),
                    };
                    var mediator = context.RequestServices.GetService<IMediator>();
                    return await mediator.Send(new GetStocks(filter));
                }
            );

        }
    }

    public class StockType : ObjectGraphType<IStock> {
        public StockType() {
            Description = "Basic stock information";
            Field( f => f.Name).Description("The name of the stock");
            Field(f => f.Isin).Description("The ISIN code for the stock");
            Field(f => f.ExchangeUrl).Description("The exchange for the stock");
            // TODO 
            //Field<ListGraphType<StockPriceSeriesType>>("priceseries")
            //    .Argument<NonNullGraphType<StringGraphType>>("periodeStart")
            //    .Argument<NonNullGraphType<StringGraphType>>("periodeEnd")
            //    .ResolveAsync( async ctx => {

            //    });
        }
    }
}
