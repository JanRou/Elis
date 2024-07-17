using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Core.Domain.Entities;
using ElisBackend.Gateways.Repositories.Currency;
using AutoMapper;

namespace ElisBackend.Core.Application.UseCases {
    public interface ICurencyHandling {
        Task<IEnumerable<ICurrency>> Get(FilterCurrency filter);
    }

    public class CurrencyHandling(ICurrencyRepository repository, IMapper mapper) : ICurencyHandling {

        public async Task<IEnumerable<ICurrency>> Get(FilterCurrency filter) {
            var result = await repository.Get(filter);
            return mapper.Map<IEnumerable<Currency>>(result);
        }
    }
}
