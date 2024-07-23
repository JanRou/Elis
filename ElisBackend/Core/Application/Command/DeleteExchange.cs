using ElisBackend.Core.Application.UseCases;
using MediatR;

namespace ElisBackend.Core.Application.Command {
    public class DeleteExchange(string name) : IRequest<bool> {
        public string Name { get; set; } = name;
    }

    public class DeleteExchangeDataHandler(IExchangeHandling exchangeHandling) : IRequestHandler<DeleteExchange, bool> {
        public async Task<bool> Handle(DeleteExchange request, CancellationToken cancellationToken) {
            return await exchangeHandling.Delete(request.Name);
        }
    }

}
