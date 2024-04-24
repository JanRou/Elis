using ElisBackend.Application.UseCases;
using MediatR;

namespace ElisBackend.Application.Command {
    
    public class UpdateStocksData : IRequest<bool> {}

    public class UpdateStocksDataHandler(IStockHandling stockHandling) : IRequestHandler<UpdateStocksData, bool> {
        public IStockHandling StockHandling { get; } = stockHandling;

        public async Task<bool> Handle(UpdateStocksData request, CancellationToken cancellationToken) {
            return await StockHandling.UpdateStocksData();
        }
    }
}
