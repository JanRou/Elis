using ElisBackend.Core.Application.UseCases;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities.Filters;
using MediatR;

namespace ElisBackend.Core.Application.Queries {
    public class GetCurrencies(FilterCurrency filter) : IRequest<IEnumerable<ICurrency>> {
        public FilterCurrency Filter { get; set; } = filter;
    }
    public class GetCurrenciesHandler(ICurencyHandling currencyHandling) : IRequestHandler<GetCurrencies, IEnumerable<ICurrency>> {

        public async Task<IEnumerable<ICurrency>> Handle(GetCurrencies request, CancellationToken cancellationToken) {
            return await currencyHandling.Get(request.Filter);
        }
    }
}
