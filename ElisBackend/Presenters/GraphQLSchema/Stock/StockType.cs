using ElisBackend.Core.Application.Command;
using ElisBackend.Core.Application.Dtos;
using ElisBackend.Core.Application.Queries;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities;
using ElisBackend.Core.Domain.Entities.Filters;
using GraphQL;
using GraphQL.Types;
using MediatR;

namespace ElisBackend.Presenters.GraphQLSchema.Stock
{
    public class StockType : ObjectGraphType<IStock>
    {
        public StockType()
        {
            Description = "Basic stock information";
            Field(s => s.Name).Description("The name of the stock");
            Field(s => s.Isin).Description("The ISIN code for the stock");
            Field("exchange", s => s.Exchange.Name).Description("The exchange name for the stock");
            Field("currency", s => s.Currency.Code).Description("The currency code for the stock");
            // TODO time series of price and volume
            //Field<ListGraphType<StockPriceSeriesType>>("priceseries")
            //    .Argument<NonNullGraphType<StringGraphType>>("periodeStart")
            //    .Argument<NonNullGraphType<StringGraphType>>("periodeEnd")
            //    .ResolveAsync( async ctx => {
            //    });
        }
    }

    public class StocksType : ObjectGraphType
    {
        public StocksType()
        {
            Field<ListGraphType<StockType>>("stocks")
                .Argument<StringGraphType>("isin")
                .Argument<StringGraphType>("name")
                .Argument<StringGraphType>("exchange")
                .Argument<StringGraphType>("currency")
                .Argument<IntGraphType>("take")  // DRY - don't repeat yorself
                .Argument<IntGraphType>("skip")
                .ResolveAsync(async context =>
                {
                    var filter = new FilterStock()
                    {
                        Isin = context.GetArgument(Name = "isin", defaultValue: ""),
                        Name = context.GetArgument(Name = "name", defaultValue: ""),
                        ExchangeName = context.GetArgument(Name = "exchange", defaultValue: ""),
                        CurrencyCode = context.GetArgument(Name = "currency", defaultValue: ""),
                        Take = context.GetArgument(Name = "take", defaultValue: 0),  // DRY - don't repeat yorself
                        Skip = context.GetArgument(Name = "skip", defaultValue: 0),
                    };
                    var mediator = context.RequestServices.GetService<IMediator>();
                    return await mediator.Send(new GetStocks(filter));
                });
        }
    }

    public class StockInputType : InputObjectGraphType<StockIn>
    {
        public StockInputType()
        {
            Name = "StockInput";
            Field<NonNullGraphType<StringGraphType>>("name");
            Field<NonNullGraphType<StringGraphType>>("isin");
            Field<NonNullGraphType<StringGraphType>>("exchangename");
            Field<NonNullGraphType<StringGraphType>>("currencycode");
        }
    }

    public class TimeSerieDataInput : InputObjectGraphType<TimeSerieDataIn>
    {
        public TimeSerieDataInput()
        {
            Name = "TimeSerieDataInput";
            Field<NonNullGraphType<StringGraphType>>("date")
                .Description("Date in UTC and ISO 8601 format: '2024-07-24T14:48:00.000Z'");
            Field<NonNullGraphType<DecimalGraphType>>("price").Description("Price as decimal");
            Field<NonNullGraphType<DecimalGraphType>>("volume").Description("Volume as decimal");
        }
    }
    public class StockDataInputType : InputObjectGraphType<StockDataIn>
    {
        public StockDataInputType()
        {
            Name = "StockDataInput";
            Field<NonNullGraphType<StringGraphType>>("isin").Description("Stock identification");
            Field<NonNullGraphType<StringGraphType>>("timeseriename").Description("Name of the timeseries usually 'PricesAndVolumes'");
        }
    }

    public class StockMutationType : ObjectGraphType
    {
        public StockMutationType()
        {
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
            Field<StockType>("adddata")
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


