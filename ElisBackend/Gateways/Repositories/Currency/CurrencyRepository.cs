using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Gateways.Dal;
using ElisBackend.Gateways.Repositories.Daos;
using Microsoft.EntityFrameworkCore;

namespace ElisBackend.Gateways.Repositories.Currency {
    public interface ICurrencyRepository {
        Task<IEnumerable<CurrencyDao>> Get(FilterCurrency filter);
        Task<CurrencyDao> Add(CurrencyDao currency);
        Task<bool> Delete(int id);
        Task<bool> Delete(string code);
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

        public async Task<CurrencyDao> Add(CurrencyDao currency) {
            db.Add(currency);
            await db.SaveChangesAsync();
            return currency;
        }
        // TODO DRY
        public async Task<bool> Delete(int id) {
            var currency = db.Currencies.Where<CurrencyDao>(s => s.Id == id).FirstOrDefault();

            bool result = currency != null;
            if (result) {
                db.Remove(currency);
                await db.SaveChangesAsync();
            }

            return result;
        }
        // TODO DRY
        public async Task<bool> Delete(string code) {
            var currency = db.Currencies.Where<CurrencyDao>(s => s.Code == code).FirstOrDefault();

            bool result = currency != null;
            if (result) {
                db.Remove(currency);
                await db.SaveChangesAsync();
            }

            return result;
        }
    }
}
