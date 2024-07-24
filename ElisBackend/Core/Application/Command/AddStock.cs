using ElisBackend.Core.Application.Dtos;
using ElisBackend.Core.Application.UseCases;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities;
using MediatR;

namespace ElisBackend.Core.Application.Command
{
    public class AddStock(StockIn stock) : IRequest<IStock> {
        public StockIn StockIn { get; set; } = stock;
    }

    public class AddStockDataHandler(IStockHandling stockHandling) : IRequestHandler<AddStock, IStock>
    {
        public async Task<IStock> Handle(AddStock request, CancellationToken cancellationToken) {
            var exchange = new Exchange(request.StockIn.ExchangeName, "", ""); // Refer to exchange by name
            var currency = new Currency("", request.StockIn.CurrencyCode);     // Refer to currency by code
            var stock = new Stock(request.StockIn.Name, request.StockIn.Isin, exchange, currency);
            return await stockHandling.Add(stock);
        }
    }
}
