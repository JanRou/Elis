using ElisBackend.Core.Application.UseCases;
using ElisBackend.Core.Domain.Abstractions;
using MediatR;

namespace ElisBackend.Core.Application.Queries
{
    public class GetStocks(StockFilter filter) : IRequest<IEnumerable<IStock>>
    {
        public StockFilter Filter { get; set; } = filter;
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
