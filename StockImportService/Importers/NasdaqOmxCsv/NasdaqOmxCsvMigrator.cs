using DataAccessLayer;
using DataAccessLayer.Tables;
using StockImportService.Pipeline;
using System.Collections.Generic;

namespace StockImportService.Importers.NasdaqOmxCsv {

    public class NasdaqOmxCsvMigrator : IOperation<PipelineDto<string, NasdaqOmxCsvlineDto>> {

        private IDal _dal;
        private int _serieIdClose; 
        private int _serieIdVolume;

        public NasdaqOmxCsvMigrator( int serieIdClose, int serieIdVolume, IDal dal) 
        {
            _serieIdClose = serieIdClose;
            _serieIdVolume = serieIdVolume;
            _dal = dal;
        }

        public IEnumerable<PipelineDto<string, NasdaqOmxCsvlineDto>> Execute(IEnumerable<PipelineDto<string, NasdaqOmxCsvlineDto>> input) {

            foreach (PipelineDto<string, NasdaqOmxCsvlineDto> p in input) {
                // Write closing price and volume
                _dal.WriteTimeserieWriter( new SerieFact() {
                            SerieId = _serieIdClose
                        ,   Time = p.Out.DateUtc
                        ,   Value = p.Out.ClosingPrice
                    }
                );
                _dal.WriteTimeserieWriter(new SerieFact() {
                            SerieId = _serieIdVolume
                        ,   Time = p.Out.DateUtc
                        ,   Value = p.Out.TotalVolume
                    }
                );
                yield return p;
            }
            yield break;
        }
    }
}
