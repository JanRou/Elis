using ElisBackend.Core.Application.UseCases;
using ElisBackend.Core.Domain.Abstractions;
using MediatR;

namespace ElisBackend.Core.Application.Command {
    public class AddExchange(IExchange exchange) : IRequest<IExchange> {
        public IExchange Exchange { get; set; } = exchange;
    }
    public class AddExchangeDataHandler(IExchangeHandling exchangeHandling) : IRequestHandler<AddExchange, IExchange> {
        public IExchangeHandling ExchangeHandling { get; } = exchangeHandling;

        public async Task<IExchange> Handle(AddExchange request, CancellationToken cancellationToken) {
            return await ExchangeHandling.Add(request.Exchange);
        }
    }

}
