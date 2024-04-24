using ElisBackend.Application.UseCases;
using ElisBackend.Domain.Abstractions;
using MediatR;

namespace ElisBackend.Application.Queries
{
    public class GetStocks(StockFilter filter) : IRequest<IEnumerable<IStock>> {
        public StockFilter Filter { get; set; } = filter;
    }

    public class GetStocksHandler(IStockHandling stockhandling) : IRequestHandler<GetStocks, IEnumerable<IStock>> {

        public IStockHandling StockHandling { get; } = stockhandling;

        public async Task<IEnumerable<IStock>> Handle( GetStocks request, CancellationToken cancellationToken) {
            return await StockHandling.Get(request.Filter);
        }
    }

}
