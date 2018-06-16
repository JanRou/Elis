using DataAccessLayer;
using StockImportService.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockImportService.Importers.NasdaqOmxCsv {

    public class NasdaqOmxCsvFilter : IOperation<PipelineDto<string, NasdaqOmxCsvlineDto>> {
        private IDal _dal;
        private int _closeSerieId;
        private DateTime _lastClose;
        public NasdaqOmxCsvFilter(IDal dal, int closeSerieId, DateTime lastClose) {
            _dal = dal;
            _closeSerieId = closeSerieId;
            _lastClose = lastClose;
        }

        public IEnumerable<PipelineDto<string, NasdaqOmxCsvlineDto>> Execute(
                    IEnumerable<PipelineDto<string, NasdaqOmxCsvlineDto>> input) {
            foreach (PipelineDto<string, NasdaqOmxCsvlineDto> p in input) {
                if (_lastClose < p.Out.DateUtc) {
                    // New close price
                    yield return p;
                }
            }
        }
    }
}
