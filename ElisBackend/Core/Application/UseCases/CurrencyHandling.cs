using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Core.Domain.Entities;
using ElisBackend.Gateways.Repositories.Currency;
using AutoMapper;
using ElisBackend.Gateways.Repositories.Daos;

namespace ElisBackend.Core.Application.UseCases {
    public interface ICurrencyHandling {
        Task<IEnumerable<ICurrency>> Get(FilterCurrency filter);
        Task<ICurrency> Add(ICurrency currency);
        Task<bool> Delete(int id);
        Task<bool> Delete(string code);
    }

    public class CurrencyHandling(ICurrencyRepository repository, IMapper mapper) : ICurrencyHandling {

        public async Task<IEnumerable<ICurrency>> Get(FilterCurrency filter) {
            var result = await repository.Get(filter);
            return mapper.Map<IEnumerable<Currency>>(result);
        }

        public async Task<ICurrency> Add(ICurrency currency) {
            var dao = mapper.Map<CurrencyDao>(currency);
            var result = await repository.Add(dao);
            return mapper.Map<Currency>(result);
        }

        public async Task<bool> Delete(int id) {
            // TODO Håndter fejl: er der nogle aktier db, som handles i valutaen, som man vil slette
            return await repository.Delete(id);
        }

        public async Task<bool> Delete(string code) {
            // TODO Håndter fejl: er der nogle aktier db, som handles i valutaen, som man vil slette
            return await repository.Delete(code);
        }

    }
}
