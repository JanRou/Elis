using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Npgsql;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using DataAccessLayer.Tables;
using DataAccessLayer.Dto;
using System.IO;

namespace DataAccessLayer {

    public class Dal : IDal {

        private readonly string _connectionString;
        private NpgsqlConnection _connection = null;
        private TextWriter _writer = null;

        public Dal(string connectionString) {
            _connectionString = connectionString;
        }

        public void Open() {
            if (_connection == null) {
                _connection = new NpgsqlConnection(_connectionString);
                _connection.Open();
            }
        }

        public void Close() {
            if (_connection != null) {
                if (_writer != null) {
                    CloseWriter();
                }
                _connection.Close();
                _connection = null;
            }
        }

        public void OpenTimeserieWriter() {
            var sb = new StringBuilder();
            sb.Append("COPY public.Timeserie");
            sb.Append(" (\"time\", serieid, value) FROM STDIN");
            _writer = GetConnection().BeginTextImport(sb.ToString());
        }

        public void CloseWriter() {
            _writer.Close();
            _writer.Dispose();
            _writer = null;
        }

        public void WriteTimeserieWriter(ISerieFact timeserie) {
            _writer.Write($"{timeserie.Time}\t{timeserie.SerieId}\t{timeserie.Value}\n");
        }

        private NpgsqlConnection GetConnection() {
            if (_connection == null) {
                Open();
            }
            return _connection;
        }

        private int ExecuteCommand(string sqlCommand) {
            NpgsqlCommand cmd = new NpgsqlCommand(sqlCommand, GetConnection());
            int result = cmd.ExecuteNonQuery();
            return result;
        }

        // Data-mart
        public IStock GetStock(string name, string marketName) {
            var query = new StringBuilder();
            query.Append($"SELECT st.* FROM stock AS st");
            query.Append($" JOIN public.market m ON st.marketid = m.id and m.name like '{marketName}'");
            query.Append($" WHERE st.name LIKE '{name}'");
            return GetConnection().QueryFirstOrDefault<Stock>(query.ToString());
        }

        public IMarket GetMarket(string name) {
            String query = $"SELECT * FROM market WHERE name LIKE '{name}'";
            return GetConnection().QueryFirstOrDefault<Market>(query);
        }

        public int InsertStock( string stockName, string ticker, string market ) {
            // INSERT INTO public.stock(
            //    name, shortname, marketid)
            //   SELECT 'Per Aarsleff A/S', 'PAAL-B', id
            //   FROM public.market where name like 'Nasdaq OMX Copenhagen';
            StringBuilder sql = new StringBuilder();
            sql.Append("INSERT INTO public.stock(");
            sql.Append(" name, shortname, marketid)");
            sql.Append($" SELECT '{stockName}', '{ticker}', id");
            sql.Append($" FROM public.market WHERE name LIKE '{market}';");
            ExecuteCommand(sql.ToString());
            return GetStock( stockName, market).Id;
        }

        public ISerieDim GetSerieForStock(int stockId, string serieAttribute) {
            //SELECT se.* FROM public.serie AS se
            //    JOIN public.stockserie AS ss ON se.id = ss.serieid
            //    JOIN public.serieattribute AS sa ON se.serieattributeid = sa.id
            //WHERE ss.stockid = 1 and sa.name like 'volume'
            var query = new StringBuilder();
            query.Append("SELECT se.* FROM public.serie AS se");
            query.Append(" JOIN public.stockserie AS ss ON se.id = ss.serieid");
            query.Append(" JOIN public.serieattribute AS sa ON se.serieattributeid = sa.id");
            query.Append($" WHERE ss.stockid = {stockId} and sa.name like '{serieAttribute}'");
            return GetConnection().QueryFirstOrDefault<SerieDim>(query.ToString());
        }

        public ISerieDim GetSerie(string name) {
            //SELECT s.id, s.name, s.datasourceid, s.valuetypeid, s.currencyid, s.serieattributeid
            //FROM public.serie AS s
            //WHERE s.Name like 'serie name'
            var query = new StringBuilder();
            query.Append("SELECT s.id FROM public.serie AS s");
            query.Append($" WHERE s.Name like '{name}'");
            return GetConnection().QueryFirstOrDefault<SerieDim>(query.ToString());
        }

        public int InsertSerie(SerieDto serie) {
            //INSERT INTO public.serie
            //        (name, datasourceid, valuetypeid, currencyid, serieattributeid)
            //SELECT  'serie name', d.id, v.id, c.id, s.id
            //FROM public.datasource AS d
            //  JOIN public.valuetype AS v ON v.name like '2 decimals'
            //  JOIN public.currency AS c ON c.shortname like 'DKK'
            //  JOIN public.serieattribute AS s ON s."name" like 'close'
            //WHERE d.ticker like 'NOVO-B'
            var sql = new StringBuilder();
            sql.Append("INSERT INTO public.serie");
            sql.Append("    (name, datasourceid, valuetypeid, currencyid, serieattributeid)");
            sql.Append($" SELECT  '{serie.Name}', d.id, v.id, c.id, s.id");
            sql.Append(" FROM public.datasource AS d");
            sql.Append($" JOIN public.valuetype AS v ON v.name like '{serie.ValueType}'");
            sql.Append($" JOIN public.currency AS c ON c.shortname like '{serie.Currency}'");
            sql.Append($" JOIN public.serieattribute AS s ON s.name like '{serie.Attribute}'");
            sql.Append($" WHERE d.ticker like '{serie.Ticker}';");
            ExecuteCommand(sql.ToString());
            return GetSerie(serie.Name).Id;
        }


        public void AssociateStockAndSerie(int stockId, int serieId) {
            //insert into public.stockserie(stockid, serieid)
            //    values(2,2)
            var sql = new StringBuilder();
            sql.Append("insert into public.stockserie(stockid, serieid)");
            sql.Append($" values ({stockId},{serieId})");
            ExecuteCommand(sql.ToString());
        }

        // Obsolete brug dem som skabelon til andre type
        //public Datasource GetDatasource(string ticker, string url) {
        //    //SELECT * FROM public.datasource AS d
        //    //  WHERE d.ticker like 'ticker' and d.url like 'url';
        //    var query = new StringBuilder();
        //    query.Append("SELECT * FROM public.datasource AS d");
        //    query.Append($" WHERE d.ticker like '{ticker}' and d.url like '{url}'");
        //    return GetConnection().QueryFirstOrDefault<Datasource>(query.ToString());
        //}

        //public void InsertDatasource(DatasourceDto dto) {
        //    //INSERT INTO public.datasource
        //    //        (ticker, url, config, marketid)
        //    // SELECT  'ticker', 'url', 'config', m.id
        //    // FROM public.market AS m
        //    // WHERE m.name like 'market name'
        //    var sql = new StringBuilder();
        //    sql.Append("INSERT INTO public.datasource (ticker, url, config, marketid)");
        //    sql.Append($" SELECT '{dto.Ticker}', '{dto.Url}', '{dto.Config}', m.id");
        //    sql.Append("  FROM public.market As m");
        //    sql.Append($" WHERE m.name like '{dto.Market}'");
        //    ExecuteCommand(sql.ToString());
        //}

        public SerieFact GetTimeserieId(DateTime time, int serieId) {
            //SELECT * FROM public.timeserie AS t
            //  WHERE t.time = time and t.serieid = serieId;
            var query = new StringBuilder();
            query.Append("SELECT * FROM public.timeserie AS t");
            query.Append($" WHERE t.time = '{time}' and t.serieid = {serieId};");
            return GetConnection().QueryFirstOrDefault<SerieFact>(query.ToString());
        }
    }
}
