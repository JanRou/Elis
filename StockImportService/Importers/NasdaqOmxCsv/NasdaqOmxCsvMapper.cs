using DataAccessLayer.Dto;
using StockImportService.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace StockImportService.Importers.NasdaqOmxCsv {

    public class NasdaqOmxCsvMapper : IOperation<PipelineDto<string, NasdaqOmxCsvlineDto>> {

        private int _multiplier;
        private string _zeros;
        private int _valueShift;
        public NasdaqOmxCsvMapper(int valueShift) {
            // always positive valueshift
            _valueShift = valueShift;
            _zeros = "";
            _multiplier = 1;
            for (int i = 0; i < valueShift; i++) {
                _multiplier *= 10;
                _zeros += "0";
            }
        }
        // Sample line:
        // Date;Bid;Ask;Opening price;High price;Low price;Closing price;Average price;Total volume;Turnover;Trades;
        // 2017-02-24;174,0;175,0;180,0;180,0;172,5;174,0;175,84;48128;8451395,5;488;
        public IEnumerable<PipelineDto<string, NasdaqOmxCsvlineDto>> Execute(IEnumerable<PipelineDto<string, NasdaqOmxCsvlineDto>> input) {
            int lineNumber = 0;
            foreach (PipelineDto<string, NasdaqOmxCsvlineDto> p in input) {
                lineNumber++;
                string[] fields = p.In.Split(';');
                if (fields.Length == 12) {
                    p.Out = new NasdaqOmxCsvlineDto();
                    // Only date, Closing price and total volume
                    p.Out.DateUtc = DateTime.Parse(fields[0]).ToUniversalTime();
                    //p.Out.Bid = GetDecimal(fields[i++]);
                    //p.Out.Ask = GetDecimal(fields[i++]);
                    //p.Out.OpeningPrice = GetDecimal(fields[i++]);
                    //p.Out.HighPrice = GetDecimal(fields[i++]);
                    //p.Out.LowPrice = GetDecimal(fields[i++]);
                    p.Out.ClosingPrice = GetDecimal(fields[6]);
                    //p.Out.AveragePrice = GetDecimal(fields[i++]);
                    p.Out.TotalVolume = GetNumber(fields[8]);
                    //p.Out.Turnover = GetDecimal(fields[i++]);
                    //p.Out.Trades = GetNumber(fields[i++]);
                    yield return p;
                }
                else {
                    Console.WriteLine($"Error in line {lineNumber}");
                }
            }
        }

        public bool IsNotEmpty(string field) {
            return field != string.Empty;
        }

        public int GetNumber(string num) {
            return IsNotEmpty(num) ? int.Parse(num) : 0;
        }

        // price "174,5" returns 17450 
        public int GetDecimal(string decim) {
            int result = 0;
            if (IsNotEmpty(decim)) {
                string[] fields = decim.Split(',');
                int c = int.Parse(fields[0]);
                // only valueshift number of decimals: 0,5 gives 50; 0,23 gives 23; "" gives 0
                int d = 0;
                if (fields.Length > 1) {
                    // There was a ',' in the decimal
                    d = int.Parse((fields[1] + _zeros).Substring(0, _valueShift));
                }
                result = (c * _multiplier) + d;
            }
            return result;
        }
    }
}
