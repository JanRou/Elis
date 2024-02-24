using ElisBackend.Application.UseCases;
using ElisBackend.Domain.Abstractions;
using ElisBackend.Gateways.Dal;
using ElisBackend.Gateways.Repositories.Daos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            // TODO introducer Take og Skip for sideinddeling (pagination)
            List<NpgsqlParameter> parms = QueuryParametersFromFilter(filter);
            string sql = "select * FROM public.SearchStocks(" + CreateParameterNames(parms) + ")";
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

        public string CreateParameterNames(List<NpgsqlParameter> parms) {
            var sb = new StringBuilder();
            bool first = true;
            foreach (NpgsqlParameter param in parms) { 
                if (first) {
                    sb.Append("@");
                    first = false;
                }
                else {
                    sb.Append(",@");
                }
                sb.Append(param.ParameterName);
            }
            return sb.ToString();
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

        /// <summary>
        /// Creates a list of SQL parameters for EF Core SqlQueryRaw call of SQL stored procedure or function.
        /// The names are lower letters and by default not prepended anything and ending with "in".
        /// Set prepend and ending according to the named arguments in the stored procedure og function.
        /// A parameter's value is always set, so string is empty string "".
        /// Take and skip is set to 0, when not present in the filter. The SQL search procedure or function
        /// has to take all for Take set to 0
        /// </summary>
        /// <param name="filter">The filter with properties to use as parameters.</param>
        /// <param name="prepend">Text string to prepend each parameter name.</param>
        /// <param name="ending">Text string to end each parameter name.</param>
        /// <returns>The list of SQL parameters representing the filter.</returns>
        public List<NpgsqlParameter> QueuryParametersFromFilter(StockFilter filter, string prepend="", string ending="in") {
            

            var props = filter.GetType().GetProperties();

            var result = new List<NpgsqlParameter>();

            foreach (PropertyInfo p in props) {

                result.Add(new NpgsqlParameter(
                        prepend + p.Name.ToLower() + ending
                    , SetNullValue( p.PropertyType, p.GetValue(filter)))
                );
            }

            return result;
        }

        // TODO lav om til pattern - switch
        public object SetNullValue(Type t, object? obj) {
            if (t == typeof(int)) {
                return obj;
            }
            if (t == typeof(string)) {
                if (obj==null) {
                    return "";
                }
            }
            return obj;
        }

    }
}
