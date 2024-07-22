﻿using ElisBackend.Core.Application.Queries;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities.Filters;
using GraphQL;
using GraphQL.Types;
using MediatR;

namespace ElisBackend.Presenters.GraphQLSchema
{
    public class StockType : ObjectGraphType<IStock> {
        public StockType() {
            Description = "Basic stock information";
            Field(f => f.Name).Description("The name of the stock");
            Field(f => f.Isin).Description("The ISIN code for the stock");
            Field("exchange", f => f.Exchange.Name).Description("The exchange name for the stock");
            Field("currency", f => f.Currency.Code).Description("The currency code for the stock");
            // TODO time series of prices
            //Field<ListGraphType<StockPriceSeriesType>>("priceseries")
            //    .Argument<NonNullGraphType<StringGraphType>>("periodeStart")
            //    .Argument<NonNullGraphType<StringGraphType>>("periodeEnd")
            //    .ResolveAsync( async ctx => {
            //    });
        }
    }

    public class StocksType : ObjectGraphType {
        public StocksType() {
            Field<ListGraphType<StockType>>("stocks")
                .Argument<StringGraphType>("isin")
                .Argument<StringGraphType>("name")
                .Argument<StringGraphType>("exchange")
                .Argument<StringGraphType>("currency")
                .Argument<IntGraphType>("take")  // DRY - don't repeat yorself
                .Argument<IntGraphType>("skip")
                .ResolveAsync(async context => {
                    // TODO FilterX er ikke godt at bruge her
                    var filter = new FilterStock() {
                        Isin = context.GetArgument(Name = "isin", defaultValue: ""),
                        Name = context.GetArgument(Name = "name", defaultValue: ""),
                        ExchangeName = context.GetArgument(Name = "exchange", defaultValue: ""),
                        CurrencyCode = context.GetArgument(Name = "currency", defaultValue: ""),
                        Take = context.GetArgument(Name = "take", defaultValue: 0),  // DRY - don't repeat yorself
                        Skip = context.GetArgument(Name = "skip", defaultValue: 0),
                    };
                    var mediator = context.RequestServices.GetService<IMediator>();
                    return await mediator.Send(new GetStocks(filter));
                }
            );
        }
    }

}


