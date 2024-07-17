using ElisBackend.Core.Application.Command;
using GraphQL.Types;
using MediatR;

namespace ElisBackend.Presenters.GraphQLSchema {
    //public class StockMutation : ObjectGraphType {
    //    public StockMutation() {
    //        Description = "Stock information";
    //        Field<StockType>("createStock")
    //            .Argument<NonNullGraphType<StringGraphType>>("isin")
    //            .Argument<NonNullGraphType<StringGraphType>>("name")
    //            .Argument<NonNullGraphType<StringGraphType>>("exchange")
    //            .Argument<NonNullGraphType<StringGraphType>>("currency")
    //            .ResolveAsync(async context => {
    //                // TODO puha det er slemt det her, der må ikke være en reference ind mod domain :(
    //                //var stock = new Stock(  
    //                //      context.GetArgument(Name = "name", defaultValue: "")
    //                //    , context.GetArgument(Name = "isin", defaultValue: "")
    //                //    , context.GetArgument(Name = "exchange", defaultValue: null)
    //                //    , context.GetArgument(Name = "currency", defaultValue: null)
    //                //    );
    //                var mediator = context.RequestServices.GetService<IMediator>();
    //                return await mediator.Send( new AddStock(/* TODO stock */));
    //            }
    //        );

    //    }
    //}
}
