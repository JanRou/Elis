using ElisBackend.Core.Application.UseCases;
using MediatR;

namespace ElisBackend.Core.Application.Command {
    public class DeleteStock(string isin) : IRequest<bool> {
        public string Isin { get; set; } = isin;
    }
    
    public class DeleteStockDataHandler(IStockHandling exchangeHandling) : IRequestHandler<DeleteStock, bool> {
        public async Task<bool> Handle(DeleteStock request, CancellationToken cancellationToken) {
            return await exchangeHandling.Delete(request.Isin);
        }
    }
}
