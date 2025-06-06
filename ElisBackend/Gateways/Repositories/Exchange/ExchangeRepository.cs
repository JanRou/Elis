﻿using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Gateways.Dal;
using ElisBackend.Gateways.Repositories.Daos;
using Microsoft.EntityFrameworkCore;

namespace ElisBackend.Gateways.Repositories.Exchange
{
    public interface IExchangeRepository
    {
        Task<IEnumerable<ExchangeDao>> Get(FilterExchange filter);
        Task<ExchangeDao> Add(ExchangeDao exchange);
        Task<bool> Delete(int id);
        Task<bool> Delete(string name);
    }

    public class ExchangeRepository(ElisContext db) : IExchangeRepository
    {
        // TODO DbContext concurrency

        public async Task<IEnumerable<ExchangeDao>> Get(FilterExchange filter)
        {
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

        public async Task<ExchangeDao> Add(ExchangeDao exchange) {
            db.Add(exchange);
            await db.SaveChangesAsync();
            return exchange;
        }
        // TODO DRY
        public async Task<bool> Delete(int id) {
            var exchange = db.Exchanges.Where<ExchangeDao>(s => s.Id == id).FirstOrDefault();

            bool result = exchange != null;
            if (result) {
                db.Remove(exchange);
                await db.SaveChangesAsync();
            }

            return result;
        }
        // TODO DRY
        public async Task<bool> Delete(string name) {
            var exchange = db.Exchanges.Where<ExchangeDao>(s => s.Name == name).FirstOrDefault();

            bool result = exchange != null;
            if (result) {
                db.Remove(exchange);
                await db.SaveChangesAsync();
            }

            return result;
        }
    }
}
