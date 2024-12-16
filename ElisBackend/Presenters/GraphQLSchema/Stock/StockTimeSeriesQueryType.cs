using ElisBackend.Core.Application.Queries;
using ElisBackend.Core.Domain.Entities.Filters;
using GraphQL;
using GraphQL.Types;
using MediatR;

namespace ElisBackend.Presenters.GraphQLSchema.Stock
{
    public class StockTimeSeriesQueryType : ObjectGraphType
    {
        public StockTimeSeriesQueryType()
        {
            Field<TimeSeriesType>("StockTimeSerieFacts")
                .Argument<StringGraphType>("isin")
                .Argument<StringGraphType>("timeseriesname")
                .Argument<NonNullGraphType<StringGraphType>>("from")
                    .Description("Date in UTC ISO 8601 format: '2024-07-24T00:00:00.00000Z'")
                .Argument<NonNullGraphType<StringGraphType>>("to")
                    .Description("Date in UTC ISO 8601 format: '2024-07-30T00:00:00.00000Z'")
                .ResolveAsync(async context => {
                    var filter = new FilterTimeSerieFacts() {
                        Isin = context.GetArgument(Name = "isin", defaultValue: ""),
                        TimeSeriesName = context.GetArgument(Name = "timeseriesname", defaultValue: ""),
                        From = context.GetArgument(Name = "from", defaultValue: ""),
                        To = context.GetArgument(Name = "to", defaultValue: ""),
                        Take = 1, // Only a single time series at the time for now ...
                        Skip = 0,
                    };
                    var mediator = context.RequestServices.GetService<IMediator>();

                    return await mediator.Send(new GetTimeSerieFacts(filter));
                });
         }
    }
}

