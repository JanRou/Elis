using ElisBackend.Core.Application.UseCases;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities;
using MediatR;

namespace ElisBackend.Core.Application.Command {
    public class AddStock(IStock stock) : IRequest<IStock> {
        public IStock Stock { get; set; } = stock;
    }

    public class AddStockDataHandler(IStockHandling stockHandling) : IRequestHandler<AddStock, IStock>
    {
        public IStockHandling StockHandling { get; } = stockHandling;

        public async Task<IStock> Handle(AddStock request, CancellationToken cancellationToken)
        {
            return await StockHandling.Add(request.Stock);
        }
    }

}
