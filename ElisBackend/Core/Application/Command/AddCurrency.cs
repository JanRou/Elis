using ElisBackend.Core.Application.Dtos;
using ElisBackend.Core.Application.UseCases;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities;
using MediatR;

namespace ElisBackend.Core.Application.Command {
    public class AddCurrency(CurrencyIn currency) : IRequest<ICurrency> {
        public CurrencyIn CurrencyIn { get; set; } = currency;
    }

    public class AddCurrencyDataHandler(ICurrencyHandling currencyHandling) : IRequestHandler<AddCurrency, ICurrency> {

        public async Task<ICurrency> Handle(AddCurrency request, CancellationToken cancellationToken) {
            var currency = new Currency(request.CurrencyIn.Name, request.CurrencyIn.Code);
            return await currencyHandling.Add(currency);
        }
    }

}
