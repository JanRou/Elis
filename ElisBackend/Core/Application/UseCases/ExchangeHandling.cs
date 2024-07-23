using AutoMapper;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities;
using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Gateways.Repositories.Daos;
using ElisBackend.Gateways.Repositories.Exchange;

namespace ElisBackend.Core.Application.UseCases
{
    public interface IExchangeHandling {
        Task<IEnumerable<IExchange>> Get(FilterExchange filter);
        Task<IExchange> Add(IExchange exchange);
        Task<bool> Delete(int id);
    }

    public class ExchangeHandling(IExchangeRepository repository, IMapper mapper) : IExchangeHandling {

        public async Task<IEnumerable<IExchange>> Get(FilterExchange filter) {
            var result = await repository.Get(filter);
            return mapper.Map<IEnumerable<Exchange>>(result);
        }

        public async Task<IExchange> Add(IExchange exchange) {
            var dao = mapper.Map<ExchangeDao>(exchange);
            var result = await repository.Add(dao);
            return mapper.Map<Exchange>(result);
        }

        public async Task<bool> Delete(int id) {
            // TODO Håndter fejl: er der nogle aktier db, som handles på børsen, som man vil slette
            return await repository.Delete(id);
        }
    }
}
