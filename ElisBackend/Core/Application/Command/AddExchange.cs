using ElisBackend.Core.Application.Dtos;
using ElisBackend.Core.Application.UseCases;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities;
using MediatR;

namespace ElisBackend.Core.Application.Command {
    public class AddExchange(ExchangeIn exchangeIn) : IRequest<IExchange> {
        public ExchangeIn ExchangeIn { get; set; } = exchangeIn;
    }

    public class AddExchangeDataHandler(IExchangeHandling exchangeHandling) : IRequestHandler<AddExchange, IExchange> {

        public async Task<IExchange> Handle(AddExchange request, CancellationToken cancellationToken) {
            var exchange = new Exchange(request.ExchangeIn.Name, request.ExchangeIn.Country, request.ExchangeIn.Url);
            return await exchangeHandling.Add(exchange);
        }
    }
}
