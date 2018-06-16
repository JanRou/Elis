using StockImportService.Pipeline;
using System.Collections.Generic;
using System.IO;

namespace StockImportService.Importers.NasdaqOmxCsv {
    public class NasdaqOmxCsvReader : IOperation<PipelineDto<string, NasdaqOmxCsvlineDto>> {

        private StreamReader _reader;
        public NasdaqOmxCsvReader(StreamReader reader) {
            _reader = reader;
        }

        public IEnumerable<PipelineDto<string, NasdaqOmxCsvlineDto>>
            Execute(IEnumerable<PipelineDto<string, NasdaqOmxCsvlineDto>> input) {
            // First two lines and keep the 3. 
            string line = " ";
            for (int i = 0; (i < 3) && !(_reader.EndOfStream || line == string.Empty); i++) {
                line = _reader.ReadLine();
            }
            while (!((line == null) || (line == string.Empty))) {
                var p = new PipelineDto<string, NasdaqOmxCsvlineDto>();
                p.In = line;
                yield return p;
                // next line from input
                line = _reader.ReadLine();
            }
        }
    }
}
