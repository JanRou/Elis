﻿using ElisBackend.Application.UseCases;
using ElisBackend.Domain.Abstractions;
using ElisBackend.Gateways.Dal;
using ElisBackend.Gateways.Repositories.Daos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Npgsql;
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

            // TODO dan en list af typen object som er typen NpgsqlParameter
            // fra filter properties og deres værdier
            // så er det dynamisk
            var namein = CreateParameter( "namein", filter.Name);
            var isinin = CreateParameter("isinin", filter.Isin);
            var currencyin = CreateParameter("currencyin", filter.Currency);
            var exchangeurlin = CreateParameter("exchangeurlin", filter.ExchangeUrl);

            string sql = "select * FROM public.SearchStocks( @namein, @isinin, @currencyin, @exchangeurlin)";
            List<int> stockIds = null;
            // TODO introducer Take og Skip for sideinddeling (pagination)
            try {
                stockIds = db.Database.SqlQueryRaw<int>(sql, namein, isinin, currencyin, exchangeurlin).ToList();
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

        private NpgsqlParameter CreateParameter( string parmin, string source) {
            return new NpgsqlParameter( parmin, !string.IsNullOrEmpty(source) ? source : "");
        }

        private NpgsqlParameter CreateParameter(StockFilter filter) {

            

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
		
				public string ConstructSqlQueuryfromFilter(InvoiceFilterFrontend filter)
{
	decimal eps = 0.0000000001m;
	Dictionary<System.Type, Func<object, string>> typeFormats = new Dictionary<Type, Func<object, string>>()
	{
		{ typeof(int), i => ((int)i)!=0 ? i.ToString() : "" },
		{ typeof(int?), i => ((int?)i).HasValue && ((int?)i) != 0 ? i?.ToString() : ""},
		{ typeof(uint), u => ((uint)u)!=0 ? ((uint)u).ToString() : "" },
		{ typeof(uint?), u => ((uint?)u).HasValue && ((uint?)u)!=0 ? u?.ToString() : "" },
		{ typeof(decimal), d =>  d.ToString() },
		{ typeof(decimal?), d => d.ToString() ??  "" },
		{ typeof(double), d => Double.IsNormal((double) d) ? d.ToString() :  "" },
		{ typeof(double?), d => ((double?) d).HasValue && Double.IsNormal((double) d) ? d?.ToString() :  "" },
		{ typeof(string), s => !string.IsNullOrEmpty((string)s) ? "'"+s+"'" : "" },
	};

	var sb = new StringBuilder("exec GetInvoices ");
	var props = filter.GetType().GetProperties();

	bool first = true;
	foreach (PropertyInfo p in props)
	{
		string parmstring = typeFormats[p.PropertyType](p.GetValue(filter));
		if (!string.IsNullOrEmpty(parmstring))
		{
			if (!first)
			{
				sb.Append(",");
			}
			sb.Append("@").Append(p.Name).Append("=");
			sb.Append(parmstring);
			first = false;
		}
	}

	return sb.ToString();
}


    }
}
