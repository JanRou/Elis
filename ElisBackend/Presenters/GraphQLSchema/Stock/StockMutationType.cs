using ElisBackend.Core.Application.Command;
using ElisBackend.Core.Application.Dtos;
using GraphQL;
using GraphQL.Types;
using MediatR;

namespace ElisBackend.Presenters.GraphQLSchema.Stock {
    public class StockMutationType : ObjectGraphType {
        public StockMutationType() {
            Field<StockType>("create")
                .Argument<NonNullGraphType<StockInputType>>("stock")
                .ResolveAsync(async ctx =>
                {
                    var stockIn = ctx.GetArgument<StockIn>("stock");
                    var mediator = ctx.RequestServices.GetService<IMediator>();
                    return await mediator.Send(new AddStock(stockIn));
                });
            Field<BooleanGraphType>("delete")
                .Argument<NonNullGraphType<StringGraphType>>("isin")
                .ResolveAsync(async ctx =>
                {
                    var isin = ctx.GetArgument<string>("isin");
                    var mediator = ctx.RequestServices.GetService<IMediator>();
                    return await mediator.Send(new DeleteStock(isin));
                });
            Field<StockDataAddResultType>("adddata")
                .Argument<NonNullGraphType<StockDataInputType>>("StockDataInput")
                .Argument<NonNullGraphType<ListGraphType<TimeSerieDataInput>>>("TimeSerieDataInput")
                .ResolveAsync(async ctx => {
                    var stockDataIn = ctx.GetArgument<StockDataIn>("StockDataInput", defaultValue: null);
                    var timeSeriesDataIn = ctx.GetArgument<List<TimeSerieDataIn>>("TimeSerieDataInput", defaultValue: null);
                    var mediator = ctx.RequestServices.GetService<IMediator>();                    
                    return await mediator.Send(new AddStockData(stockDataIn, timeSeriesDataIn));
                });
        }
    }
}


