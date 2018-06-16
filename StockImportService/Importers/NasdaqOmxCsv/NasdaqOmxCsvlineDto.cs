using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockImportService.Importers.NasdaqOmxCsv {
    public class NasdaqOmxCsvlineDto {
        public NasdaqOmxCsvlineDto() { }
        public NasdaqOmxCsvlineDto(
                DateTime dateUtc
            , int bid, int ask, int open, int high, int low, int close, int avg, int total
            , int turn, int trades
        ) {
            DateUtc = dateUtc; Bid = bid; Ask = ask; OpeningPrice = open; HighPrice = high;
            LowPrice = low; ClosingPrice = close; AveragePrice = avg; TotalVolume = total;
            Turnover = turn; Trades = trades;
        }
        public DateTime DateUtc { get; set; }
        public int Bid { get; set; }
        public int Ask { get; set; }
        public int OpeningPrice { get; set; }
        public int HighPrice { get; set; }
        public int LowPrice { get; set; }
        public int ClosingPrice { get; set; }
        public int AveragePrice { get; set; }
        public int TotalVolume { get; set; }
        public int Turnover { get; set; }
        public int Trades { get; set; }
    }
}