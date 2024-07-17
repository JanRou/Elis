using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Gateways.Dal;
using ElisBackend.Gateways.Repositories.Daos;
using Microsoft.EntityFrameworkCore;

namespace ElisBackend.Gateways.Repositories.Exchange
{
    public interface IExchangeRepository
    {
        Task<IEnumerable<ExchangeDao>> Get(FilterExchange filter);
    }

    public class ExchangeRepository(ElisContext db) : IExchangeRepository
    {
        // TODO DbContext concurrency

        public async Task<IEnumerable<ExchangeDao>> Get(FilterExchange filter)
        {
            // TODO What is GraphQL doing eager or lazy loading?
            return db.Exchanges
                .Where(e =>
                       (string.IsNullOrEmpty(filter.Name) || 
                        (!string.IsNullOrEmpty(filter.Name) && EF.Functions.ILike( e.Name, filter.Name)))
                    && (string.IsNullOrEmpty(filter.Country) || 
                        (!string.IsNullOrEmpty(filter.Country) && EF.Functions.ILike(e.Country,filter.Country)))
                    && (string.IsNullOrEmpty(filter.Url) || 
                        (!string.IsNullOrEmpty(filter.Url) && EF.Functions.ILike(e.Url, filter.Url)))
                );
        }
    }
}
