using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Tables;
using DataAccessLayer.Dto;


namespace DataAccessLayer {

    public interface IDal {
        void Open();
        void Close();
        //void OpenTimeserieWriter();
        //void CloseWriter();
        //void WriteTimeserieWriter(ISerieFact timeserie);
        IStock GetStock(string name, string market);
        int InsertStock(string stockName, string ticker, string market);
        ISerieDim GetSerieForStock(int stockId, string serieAttribute);
        ISerieDim GetSerie(string name);
        int InsertSerie(SerieDto serie);
        void AssociateStockAndSerie(int stockId, int serieId);
        SerieFact GetTimeserieId(DateTime time, int serieId);
    }
}
