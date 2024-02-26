using ElisBackend.Application.UseCases;
using ElisBackend.Domain.Abstractions;
using ElisBackend.Extensions;
using ElisBackend.Gateways.Dal;
using ElisBackend.Gateways.Repositories.Daos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;

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
           
            List<int> stockIds = null;
            var parms = new List<NpgsqlParameter>().QueryParametersFromClass<StockFilter>(filter);
            string sql = "select * FROM public.SearchStocks(" + parms.CreateParameterNames() + ")";
            try {
                stockIds = db.Database.SqlQueryRaw<int>(sql, parms.ToArray()).ToList();
            }
            catch (Exception ex) {
                // TODO LOG exception
                throw ex;
             }

            return db.Stocks
                .Where<StockDao>( s => stockIds.Contains(s.Id) )
                .Include( c => c.Currency)
                .Include( e => e.Exchange);

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
