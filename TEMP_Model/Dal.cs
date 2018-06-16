using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Dapper;
using ED.Atlas.Svc.TC.Tray.FE.Trades;
using ED.Atlas.Svc.TC.Tray.FE.TradeStore;

namespace ED.Atlas.Svc.TC.Tray.FE.TradeStore
{
    public class Dal : IDal, IDisposable
    {
        private readonly string _connectionString;
        private SqlConnection _dbConnection;

        public Dal(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Open()
        {
            _dbConnection = new SqlConnection(_connectionString);
            _dbConnection.Open();
        }

        public void Close()
        {
            _dbConnection.Close();
        }

        public void Dispose()
        {
            _dbConnection.Close();
        }

        public int Insert(Trade trade)
        {
            trade.Id = _dbConnection.Query<int>(
                @"
                    insert into Trade(Guid, TradeTimeUtc, ContractId, XmlId)
                    values (@Guid, @TradeTimeUtc, @ContractId, @XmlId);
                    select cast(scope_identity() as int)
                "
                , trade).First();
            return trade.Id;
        }

        public void Delete(Trade trade)
        {
            _dbConnection.Execute(@"delete from Trade where id = @id", new {id = trade.Id});
        }

        public Trade FindTrade(int id)
        {
            return _dbConnection.Query<Trade>("select * from Trade where id = @Id", new {Id = id}).FirstOrDefault();
        }
        public Trade FindTrade(string contractId)
        {
            return _dbConnection.Query<Trade>("select * from Trade where contractId = @ContractId"
                , new { ContractId = contractId }).FirstOrDefault();
        }

        public int Insert(XmlEvent xmlEvent)
        {
            // Prepare call with dynamic parameters for the 
            SqlXml sqlXml = new SqlXml(new XmlTextReader(new StringReader(xmlEvent.XmlData.InnerXml)));
            var dynparms = new DynamicParameters();
            dynparms.Add("@AcquiredTimeUtc", xmlEvent.AcquiredTimeUtc, DbType.DateTime2);
            dynparms.Add("@Md5", xmlEvent.Md5, DbType.String);
            dynparms.Add("@Type", xmlEvent.Type, DbType.Int32);
            dynparms.Add("@XmlData", sqlXml, DbType.Xml, ParameterDirection.Input, -1);
            dynparms.Add("@Id", DbType.Int32, direction: ParameterDirection.Output);
            _dbConnection.Execute(
                @"
                    insert into xmlevent(AcquiredTimeUtc, XmlData, Md5, Type)
                    values (@AcquiredTimeUtc, @XmlData, @Md5, @Type);
                    select @Id = @@IDENTITY;
                "
                , dynparms);

            xmlEvent.Id = dynparms.Get<int>("@Id");
            return xmlEvent.Id;
        }

        public void Delete(XmlEvent xmlEvent)
        {
            _dbConnection.Execute(@"delete from XmlEvent where id = @id", new {id = xmlEvent.Id});
        }

        public XmlEvent FindXmlEvent(int id)
        {
            // Get row except xml data with dapper
            XmlEvent res = _dbConnection.Query<XmlEvent>(
                "select Id, AcquiredTimeUtc, Md5, Type from XmlEvent where id=@Id"
                , new {Id = id}
                ).FirstOrDefault();

            if (res != null)
            {
                // Got a row, now the hard part to get xml data. Use plain ADO
                string cmdString = string.Format("select XmlData from XmlEvent where id = {0}", id);
                SqlCommand cmd = new SqlCommand(cmdString, _dbConnection);
                SqlDataReader sqlDataReader = cmd.ExecuteReader();
                if (sqlDataReader.Read())
                {
                    SqlXml sqlXml = sqlDataReader.GetSqlXml(0);
                    res.XmlData = new XmlDocument();
                    res.XmlData.LoadXml(sqlXml.Value);
                }
            }
            return res;
        }

        public int Insert(Company company)
        {
            // get current existing companu entry if it exist
            var oldCompany = _dbConnection.Query<Company>(
                @"
                        select * from Company
                        where CompanyId = @CompanyId
                    "
                , new {CompanyId = company.CompanyId}
                ).OrderByDescending(c => c.ValidBeginUtc).FirstOrDefault();
            if (oldCompany != null)
            {
                // Update existing company current to previous 
                var dynparms = new DynamicParameters();
                dynparms.Add("@ValidEndUtc", company.ValidBeginUtc, DbType.DateTime2);
                dynparms.Add("@Id", oldCompany.Id, DbType.Int32);
                _dbConnection.Execute(
                    @"
                        update Company set ValidEndUtc = @ValidEndUtc
                        where Id = @Id
                    "
                    , dynparms
                    );
            }
            // Insert new current
            company.Id = _dbConnection.Query<int>(
                @"
                    insert into Company( CompanyId, ValidBeginUtc, XmlId)
                    values (@CompanyId, @ValidBeginUtc, @XmlId);
                    select cast(scope_identity() as int)
                "
                , company
                ).First();
            return company.Id;
        }

        public void Delete(Company company)
        {
            _dbConnection.Execute(@"delete from Company where id = @id", new {id = company.Id});
        }

        public Company FindCompany(int id)
        {
            return _dbConnection.Query<Company>("select * from Company where id = @Id"
                , new {Id = id}).FirstOrDefault();
        }

        public int Insert(SequenceItem seqItem)
        {
            // get current existing companu entry if it exist
            var oldSeqItem = _dbConnection.Query<SequenceItem>(
                @"
                        select * from SequenceItem
                        where SeqId = @SeqId
                "
                , new {SeqId = seqItem.SeqId}
                ).OrderByDescending(s => s.ValidBeginUtc).FirstOrDefault();
            if (oldSeqItem != null)
            {
                // Update existing company current to previous 
                var dynparms = new DynamicParameters();
                dynparms.Add("ValidEndUtc", seqItem.ValidBeginUtc, DbType.DateTime2);
                dynparms.Add("Id", oldSeqItem.Id, DbType.Int32);
                _dbConnection.Execute(
                    @"
                        update SequenceITem set ValidEndUtc = @ValidEndUtc
                        where Id = @Id
                    "
                    , dynparms
                    );
            }
            // Insert new current
            seqItem.Id = _dbConnection.Query<int>(
                @"
                    insert into SequenceItem( SeqId, ValidBeginUtc, XmlId)
                    values (@SeqId, @ValidBeginUtc, @XmlId);
                    select cast(scope_identity() as int)
                "
                , seqItem
                ).First();
            return seqItem.Id;
        }

        public void Delete(SequenceItem seqItem)
        {
            _dbConnection.Execute(@"delete from SequenceItem where id = @id", new {id = seqItem.Id});
        }

        public SequenceItem FindSequenceItem(int id)
        {
            return _dbConnection.Query<SequenceItem>("select * from SequenceItem where id = @Id"
                , new {Id = id}).FirstOrDefault();
        }

        public int Insert(InstrumentDefinition instDef)
        {
            // get current existing company entry if it exist
            var oldInstDef = _dbConnection.Query<InstrumentDefinition>(
                @"
                                        select * from InstrumentDefinition
                                        where InstId = @InstId
                                    "
                , new {InstId = instDef.InstId}
                ).OrderByDescending(s => s.ValidBeginUtc).FirstOrDefault();
            if (oldInstDef != null)
            {
                // Update existing company current to previous 
                var dynparms = new DynamicParameters();
                dynparms.Add("ValidEndUtc", instDef.ValidBeginUtc, DbType.DateTime2);
                dynparms.Add("Id", oldInstDef.Id, DbType.Int32);
                _dbConnection.Execute(
                    @"
                        update InstrumentDefinition set ValidEndUtc = @ValidEndUtc
                        where Id = @Id
                    "
                    , dynparms
                    );
            }
            // Insert new current
            instDef.Id = _dbConnection.Query<int>(
                @"
                    insert into InstrumentDefinition( InstId, ValidBeginUtc, XmlId)
                    values (@InstId, @ValidBeginUtc, @XmlId);
                    select cast(scope_identity() as int)
                "
                , instDef
                ).First();
            return instDef.Id;
        }

        public void Delete(InstrumentDefinition instDef)
        {
            _dbConnection.Execute(@"delete from InstrumentDefinition where id = @id", new {id = instDef.Id});
        }

        public InstrumentDefinition FindInstrumentDefintion(int id)
        {
            return _dbConnection.Query<InstrumentDefinition>(
                "select * from InstrumentDefinition where id = @Id"
                , new {Id = id}).FirstOrDefault();
        }

        public XmlEvent SelectLatestXmlEvent(XmlMessageType xmlType)
        {
            return _dbConnection.Query<XmlEvent>(
                @"
                    select [Id],[AcquiredTimeUtc], [MD5],[Type] from XmlEvent
                    where Type = @Type
                "
                , new {Type = (int) xmlType}
                ).OrderByDescending(c => c.AcquiredTimeUtc).FirstOrDefault();
        }

        public IEnumerable<T> Select<T>(string table)
        {
            return _dbConnection.Query<T>(@"select * from " + table);
        }
    }
}