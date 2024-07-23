using ElisBackend.Core.Application.UseCases;
using ElisBackend.Core.Domain.Abstractions;
using MediatR;

namespace ElisBackend.Core.Application.Command {
    public class AddCurrency(ICurrency currency) : IRequest<ICurrency> {
        public ICurrency Currency { get; set; } = currency;
    }

    public class AddCurrencyDataHandler(ICurrencyHandling currencyHandling) : IRequestHandler<AddCurrency, ICurrency> {

        public async Task<ICurrency> Handle(AddCurrency request, CancellationToken cancellationToken) {
            return await currencyHandling.Add(request.Currency);
        }
    }

}
