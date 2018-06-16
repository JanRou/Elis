using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net.Repository.Hierarchy;

namespace ED.Wp3.Server.BE.PrognosisMetadata
{
    public interface IPrognosisMetadataProvider
    {
       List<PrognosisMetadataProductionDto> GetPrognosisProduction();
       List<PrognosisMetadataWeatherDto> GetPrognosisWeather();
    }

    public class PrognosisMetadataProvider : IPrognosisMetadataProvider
    {
        private SqlConnection _mSSqlConnection;
        private SqlCommand _sqlCommand;
        private DbDataReader _dbReader;
        private readonly string _connectionstring;


        public PrognosisMetadataProvider(string connectionstring)
        {
            _connectionstring = connectionstring;
        }

        public List<PrognosisMetadataProductionDto> GetPrognosisProduction()
        {
            List<PrognosisMetadataProductionDto> result = new List<PrognosisMetadataProductionDto>();
            ExecuteCommander(@"SELECT * FROM [dbo].[GetWindproductionPrognosisMetadata] (DEFAULT)");
            try
            {
                while (_dbReader.Read())
                {
                    if ( _dbReader["ModelId"] == null )
                    {
                        throw new ArgumentException($"ModelId not found in result from dataindsamling2.");
                    }
                    PrognosisMetadataProductionDto dto = new PrognosisMetadataProductionDto();
                    int i;
                    dto.ModelId = _dbReader["ModelId"]?.ToString().Trim();
                    dto.ModelName = _dbReader["ModelName"]?.ToString().Trim();
                    dto.ModelDescription = _dbReader["ModelDescription"]?.ToString().Trim();
                    dto.WindAreaId = _dbReader["WindAreaId"]?.ToString().Trim();
                    dto.WindAreaName = _dbReader["WindAreaName"]?.ToString().Trim();
                    dto.ControlCode = int.TryParse(_dbReader["ControlCode"]?.ToString().Trim(), out i) ? i : -1;
                    dto.ControlName = _dbReader["ControlName"]?.ToString().Trim();
                    dto.ControlValue = _dbReader["ControlValue"]?.ToString().Trim();
                    dto.ControlValueType = _dbReader["ControlValueType"]?.ToString().Trim();
                    DateTime dt;
                    if ( DateTime.TryParse(
                        _dbReader["StatusUpdated"].ToString() != string.Empty 
                            ? _dbReader["StatusUpdated"].ToString() : DateTime.MinValue.ToString() , out dt) )
                    {
                        // Got a date
                        dto.Updated = dt;
                    }
                    result.Add(dto);
                }
            }
            catch (Exception)
            {
                // TODO log something or???
                result = null;
            }
            return result;
        }

        public List<PrognosisMetadataWeatherDto> GetPrognosisWeather()
        {
            return null;
        }

        private void ExecuteCommander(string sqlSelectString)
        {
            try
            {
                _mSSqlConnection = new SqlConnection(_connectionstring);
                _sqlCommand = new SqlCommand(sqlSelectString, _mSSqlConnection);
                _sqlCommand.CommandTimeout = 120;
                _mSSqlConnection.Open();

                _dbReader = _sqlCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to initialize sql connection to DataIndsamling2 Integration. {ex.Message}");
            }
        }

    }
}
