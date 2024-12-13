using ElisBackend.Core.Application.UseCases;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities.Filters;
using MediatR;

namespace ElisBackend.Core.Application.Queries
{
    public class GetStocks(FilterStock filter) : IRequest<IEnumerable<IStock>>
    {
        public FilterStock Filter { get; set; } = filter;
    }

    public class GetStocksHandler(IStockHandling stockhandling) : IRequestHandler<GetStocks, IEnumerable<IStock>>
    {
        public IStockHandling StockHandling { get; } = stockhandling;

        public async Task<IEnumerable<IStock>> Handle(GetStocks request, CancellationToken cancellationToken)
        {
            return await StockHandling.Get(request.Filter);
        }
    }
}
