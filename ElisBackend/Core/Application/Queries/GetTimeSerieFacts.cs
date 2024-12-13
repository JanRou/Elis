using ElisBackend.Core.Application.UseCases;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities.Filters;
using MediatR;

namespace ElisBackend.Core.Application.Queries
{
    public class GetTimeSerieFacts(FilterTimeSerieFacts filter) : IRequest<IStockTimeSeries>
    {
        public FilterTimeSerieFacts Filter { get; set; } = filter;
    }

    public class GetStockDataHandler(IStockHandling stockhandling) : IRequestHandler<GetTimeSerieFacts, IStockTimeSeries>
    {
        public IStockHandling StockHandling { get; } = stockhandling;

        public async Task<IStockTimeSeries> Handle(GetTimeSerieFacts request, CancellationToken cancellationToken)
        {
            return await StockHandling.GetTimeSerieFacts(request.Filter);
        }
    }

}
