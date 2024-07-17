using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Gateways.Dal;
using ElisBackend.Gateways.Repositories.Daos;
using Microsoft.EntityFrameworkCore;

namespace ElisBackend.Gateways.Repositories.Currency {
    public interface ICurrencyRepository {
        Task<IEnumerable<CurrencyDao>> Get(FilterCurrency filter);
    }

    public class CurrencyRepository(ElisContext db) : ICurrencyRepository {
        // TODO DbContext concurrency

        public async Task<IEnumerable<CurrencyDao>> Get(FilterCurrency filter) {
            // TODO What is GraphQL doing eager or lazy loading?
            var query = db.Currencies
                .Where(e =>
                       (string.IsNullOrEmpty(filter.Name) ||
                        (!string.IsNullOrEmpty(filter.Name) && EF.Functions.ILike(e.Name, filter.Name)))
                    && (string.IsNullOrEmpty(filter.Code) ||
                        (!string.IsNullOrEmpty(filter.Code) && EF.Functions.ILike(e.Code, filter.Code)))
                );
            var sql = query.ToQueryString();
            return query;
        }
    }
}
