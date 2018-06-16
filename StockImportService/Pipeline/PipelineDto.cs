using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockImportService.Pipeline {
    public class PipelineDto<TI,TO> {
        public TI In { get; set; }
        public TO Out { get; set; }
    }
}
