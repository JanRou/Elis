﻿using ElisBackend.Core.Application.Queries;
using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities;
using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Extensions;
using ElisBackend.Gateways.Dal;
using ElisBackend.Gateways.Repositories.Daos;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ElisBackend.Gateways.Repositories.Stock
{

    public interface IStockRepository {
        /// <summary>
        /// Gets stocks from the repostitory using a filter. 
        /// </summary>
        /// <param name="filter">Filter to select stocks.</param>
        /// <returns>A list of stocks</returns>
        Task<IEnumerable<StockDao>> Get(FilterStock filter);

        /// <summary>
        /// Adds a new stock referring to the exchange by name or id and currency by by code or id.
        /// Exchange or currency can't be created at the same time as a stock.
        /// </summary>
        /// <param name="stock">Stock to add with name and isin code.</param>
        /// <returns>Stock added with ExchangeId and CurrencyId</returns>
        Task<StockDao> Add(StockDao stock);

        /// <summary>
        /// Delete a stock identified by it's isin code
        /// </summary>
        /// <param name="isin">Code identifying the stock</param>
        /// <returns>True when deleted otherwise false</returns>
        Task<bool> DeleteStock(string isin);
        Task<bool> DeleteStock(int id);

    }

    public class StockRepository(ElisContext db) : IStockRepository {
        // TODO DbContext concurrency

        public async Task<IEnumerable<StockDao>> Get(FilterStock filter) {

            List<int> stockIds = null;
            var parms = new List<NpgsqlParameter>().QueryParametersFromClass<FilterStock>(filter);
            string sql = "select * FROM public.SearchStocks(" + parms.CreateParameterNames() + ")";
            // SearchStocks returns ids in sorted order
            try {
                stockIds = db.Database.SqlQueryRaw<int>(sql, parms.ToArray()).ToList();
            }
            catch (Exception ex) {
                // TODO LOG exception
                throw ex;
            }

            var unsorted = db.Stocks
                .Where<StockDao>(s => stockIds.Contains(s.Id))                
                .Include(s => s.Currency)
                .Include(s => s.Exchange)
                .ToList();

            // Sort by Searchstocks result before returning
            // TODO get rid of the linear search FindIndex
            return unsorted.OrderBy(u => stockIds.FindIndex( i => i == u.Id));            
        }

        public async Task<StockDao> Add(StockDao stock) {
            stock.ExchangeId = GetExchangeId(stock);
            stock.CurrencyId = GetCurrencyId(stock);
            // Reset exchange and currency in stock, so EF doesn't create new exchange and currency entries
            stock.Exchange = null;
            stock.Currency = null;
            db.Add(stock);
            await db.SaveChangesAsync();
            return stock;
        }

        private int GetExchangeId(StockDao stock) {
            return db.Exchanges.Where(e=>
                ((!(stock.Exchange==null || string.IsNullOrEmpty(stock.Exchange.Name))) && e.Name == stock.Exchange.Name)
                ||
                ( (stock.Exchange == null || string.IsNullOrEmpty(stock.Exchange.Name)) && e.Id == stock.ExchangeId)
                ).First().Id;
        }

        private int GetCurrencyId(StockDao stock) {
            return db.Currencies.Where(c =>
                 ((!(stock.Currency == null || string.IsNullOrEmpty(stock.Currency.Code))) && c.Code == stock.Currency.Code)
                 ||
                 ((stock.Currency == null || string.IsNullOrEmpty(stock.Currency.Code)) && c.Id == stock.CurrencyId)
                 ).First().Id;
        }


        // TODO DRY
        public async Task<bool> DeleteStock(int id) {
            var stock = db.Stocks.Where<StockDao>(s => s.Id == id).First();

            bool result = stock != null;
            if (result) {
                db.Remove(stock);
                await db.SaveChangesAsync();
            }

            return result;
        }

        // TODO DRY
        public async Task<bool> DeleteStock(string isin) {
            var stock = db.Stocks.Where<StockDao>(s => s.Isin == isin).First();

            bool result = stock != null;
            if (result) {
                db.Remove(stock);
                await db.SaveChangesAsync();
            }

            return result;
        }
    }
}