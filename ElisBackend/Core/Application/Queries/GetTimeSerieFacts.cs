using ElisBackend.Core.Application.UseCases;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities.Filters;
using MediatR;

namespace ElisBackend.Core.Application.Queries
{
    public class GetTimeSerieFacts(FilterTimeSerieFacts filter) : IRequest<ITimeSeries>
    {
        public FilterTimeSerieFacts Filter { get; set; } = filter;
    }

    public class GetStockDataHandler(ITimeSeriesHandling timeSeriesHandling) : IRequestHandler<GetTimeSerieFacts, ITimeSeries>
    {
        public ITimeSeriesHandling TimeSeriesHandling { get; } = timeSeriesHandling;

        public async Task<ITimeSeries> Handle(GetTimeSerieFacts request, CancellationToken cancellationToken)
        {
            return await TimeSeriesHandling.GetTimeSeries(request.Filter);
        }
    }

}
