using ElisBackend.Application.UseCases;
using ElisBackend.Domain.Abstractions;
using ElisBackend.Gateways.Dal;
using ElisBackend.Gateways.Repositories.Daos;
using System;
using System.Linq;

namespace ElisBackend.Gateways.Repositories.Stock {

    public interface IStockRepository {
        Task<IEnumerable<StockDao>> Get(StockFilter filter);
        Task<StockDao> Add(StockDao stock);
        Task<bool> Delete(int id);
    }

    public class StockRepository : IStockRepository {

        public StockRepository(ElisContext elisContext) {
            db = elisContext;
        }

        public ElisContext db { get; }

        public async Task<IEnumerable<StockDao>> Get(StockFilter filter) {

            // TODO kald en stored procedure til at udføre filteret, der bliver alt for 
            // kompliceret i linq med fare for at ef henter hele tabellen
            //var query = from photo in context.Set<PersonPhoto>()
            //           join person in context.Set<Person>()
            //               on photo.PersonPhotoId equals person.PhotoId
            //           select new { person, photo };

            return db.Stocks.Where<StockDao>(
                    s => (string.IsNullOrEmpty(filter.Name)
                                || (!string.IsNullOrEmpty(filter.Name) && s.Name.Contains(filter.Name)))
                        && (string.IsNullOrEmpty(filter.Isin)
                                || (!string.IsNullOrEmpty(filter.Isin) && s.Isin.Contains(filter.Isin)))
                        //&& (string.IsNullOrEmpty(filter.ExchangeUrl)
                        //        || (string.IsNullOrEmpty(filter.ExchangeUrl) 
                        //                && s.Exchange.ExchangeUrl.Contains(filter.ExchangeUrl)))
                );
        }

        public async Task<StockDao> Add(StockDao stock) {
            db.Add(stock);
            await db.SaveChangesAsync();

            return stock;
        }

        public async Task<bool> Delete(int id) {
            var stock = db.Stocks.Where<StockDao>(s => s.Id == id).FirstOrDefault();

            bool result = stock != null;
            if (result) {
                db.Remove(stock);
                await db.SaveChangesAsync();
            }

            return result;
        }

    }
}
