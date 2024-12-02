using ElisBackend.Core.Application.Command;
using ElisBackend.Core.Application.Dtos;
using ElisBackend.Core.Application.Queries;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities;
using ElisBackend.Core.Domain.Entities.Filters;
using GraphQL;
using GraphQL.Types;
using MediatR;

namespace ElisBackend.Presenters.GraphQLSchema.Exchange
{

    public class ExchangeType : ObjectGraphType<IExchange>
    {
        public ExchangeType()
        {
            Field(f => f.Name).Description("The name of the exchange");
            Field(f => f.Country).Description("The country of the exchange");
            Field(f => f.Url).Description("The url to connect to the exchange");
        }
    }
    public class ExchangesType : ObjectGraphType
    {
        public ExchangesType()
        {
            Field<ListGraphType<ExchangeType>>("exchanges")
                .Argument<StringGraphType>("name")
                .Argument<StringGraphType>("exchange")
                .Argument<StringGraphType>("url")
                .Argument<IntGraphType>("take")
                .Argument<IntGraphType>("skip")
                .ResolveAsync(async context =>
                {
                    var filter = new FilterExchange()
                    {
                        Name = context.GetArgument(Name = "name", defaultValue: ""),
                        Country = context.GetArgument(Name = "country", defaultValue: ""),
                        Url = context.GetArgument(Name = "url", defaultValue: ""),
                        Take = context.GetArgument(Name = "take", defaultValue: 0),
                        Skip = context.GetArgument(Name = "skip", defaultValue: 0),
                    };
                    var mediator = context.RequestServices.GetService<IMediator>();
                    return await mediator.Send(new GetExchanges(filter));
                });
        }
    }

    public class ExchangeInputType : InputObjectGraphType
    {
        public ExchangeInputType()
        {
            Name = "ExchangeInput";
            Field<NonNullGraphType<StringGraphType>>("name");
            Field<NonNullGraphType<StringGraphType>>("country");
            Field<NonNullGraphType<StringGraphType>>("url");
        }
    }

    public class ExchangeMutationType : ObjectGraphType
    {
        public ExchangeMutationType()
        {
            Field<ExchangeType>("create")
                .Argument<NonNullGraphType<ExchangeInputType>>("exchange")
                .ResolveAsync(async ctx =>
                {
                    var exchangeIn = ctx.GetArgument<ExchangeIn>("exchange");
                    var mediator = ctx.RequestServices.GetService<IMediator>();
                    return await mediator.Send(new AddExchange(exchangeIn));
                });
            Field<BooleanGraphType>("delete")
                .Argument<NonNullGraphType<StringGraphType>>("name")
                .ResolveAsync(async ctx =>
                {
                    var exchangeName = ctx.GetArgument<string>("name");
                    var mediator = ctx.RequestServices.GetService<IMediator>();
                    return await mediator.Send(new DeleteExchange(exchangeName));
                });
        }
    }
}


