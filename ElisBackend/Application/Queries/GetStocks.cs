using ElisBackend.Application.UseCases;
using ElisBackend.Domain.Abstractions;
using MediatR;

namespace ElisBackend.Application.Queries
{
    public class GetStocks : IRequest<IEnumerable<IStock>> {
        public GetStocks(StockFilter filter) {
            Filter = filter;
        }
        public StockFilter Filter { get; set; }
    }

    public class GetStocksHandler : IRequestHandler<GetStocks, IEnumerable<IStock>> {

        public GetStocksHandler(IStockHandling stockhandling) {
            StockHandling = stockhandling;
        }

        public IStockHandling StockHandling { get; }

        public async Task<IEnumerable<IStock>> Handle( GetStocks request, CancellationToken cancellationToken) {
            return await StockHandling.Get(request.Filter);
        }
    }

}
