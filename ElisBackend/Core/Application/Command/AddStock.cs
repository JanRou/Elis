using ElisBackend.Core.Application.UseCases;
using ElisBackend.Core.Domain.Abstractions;
using MediatR;

namespace ElisBackend.Core.Application.Command
{
    public class AddStock() : IRequest<bool> {
    }

    public class AddStockDataHandler(IStockHandling stockHandling) : IRequestHandler<AddStock, bool>
    {
        public IStockHandling StockHandling { get; } = stockHandling;

        public async Task<bool> Handle(AddStock request, CancellationToken cancellationToken)
        {
            IStock stock = null; 
            return await StockHandling.AddStock(stock);
        }
    }
}
