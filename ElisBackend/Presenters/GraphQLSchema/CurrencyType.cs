﻿using ElisBackend.Core.Application.Queries;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities.Filters;
using GraphQL;
using GraphQL.Types;
using MediatR;

namespace ElisBackend.Presenters.GraphQLSchema
{
    public class CurrencyType : ObjectGraphType<ICurrency> {
        public CurrencyType() {
            Field(f => f.Name).Description("The name of the currency");
            Field(f => f.Code).Description("The short name of the currency");
        }
    }
    public class CurrenciesType : ObjectGraphType {
        public CurrenciesType() {
            Field<ListGraphType<CurrencyType>>("currencies")
                .Argument<StringGraphType>("name")
                .Argument<StringGraphType>("code")
                .Argument<IntGraphType>("take") // DRY - don't repeat yorself
                .Argument<IntGraphType>("skip")
                .ResolveAsync(async context => {
                    // TODO FilterX er ikke godt at bruge her
                    var filter = new FilterCurrency() {
                        Name = context.GetArgument(Name = "name", defaultValue: ""),
                        Code = context.GetArgument(Name = "code", defaultValue: ""),
                        Take = context.GetArgument(Name = "take", defaultValue: 0),  // DRY - don't repeat yorself
                        Skip = context.GetArgument(Name = "skip", defaultValue: 0),
                    };
                    var mediator = context.RequestServices.GetService<IMediator>();
                    return await mediator.Send(new GetCurrencies(filter));
                }
            );
        }
    }
}