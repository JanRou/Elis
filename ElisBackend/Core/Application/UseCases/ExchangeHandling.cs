using AutoMapper;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities;
using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Gateways.Repositories.Exchange;

namespace ElisBackend.Core.Application.UseCases
{
    public interface IExchangeHandling {
        Task<IEnumerable<IExchange>> Get(FilterExchange filter);
    }

    public class ExchangeHandling(IExchangeRepository repository, IMapper mapper) : IExchangeHandling {

        public async Task<IEnumerable<IExchange>> Get(FilterExchange filter) {
            var result = await repository.Get(filter);
            return mapper.Map<IEnumerable<Exchange>>(result);
        }
    }
}
