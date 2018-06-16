using StockImportService.Pipeline;
using System;
using DataAccessLayer;
using System.IO;

namespace StockImportService.Importers.NasdaqOmxCsv {
    public class NasdaqOmxCsvImporterPipeline : IImporterPipeline {

        private Pipeline<PipelineDto<string, NasdaqOmxCsvlineDto>> _pipeLine;

        public NasdaqOmxCsvImporterPipeline(
                StreamReader streamReader
            , int priceShift
            , int serieIdClose
            , DateTime lastClose
            , int serieIdVolume
            , IDal dataAccessLayer
        ) {
            _pipeLine = new Pipeline<PipelineDto<string, NasdaqOmxCsvlineDto>>();
            _pipeLine.Register(new NasdaqOmxCsvReader(streamReader));
            _pipeLine.Register(new NasdaqOmxCsvMapper(priceShift));
            _pipeLine.Register(new NasdaqOmxCsvFilter(dataAccessLayer, serieIdClose, lastClose));
            _pipeLine.Register(new NasdaqOmxCsvMigrator(serieIdClose, serieIdVolume
                    , dataAccessLayer));
        }

        public void Execute() {
            _pipeLine.Execute();
        }
    }
}
