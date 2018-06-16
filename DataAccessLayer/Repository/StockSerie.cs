using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Tables {

    /// <summary>
    /// Relation between Stock and Serie. A stock may have several series like
    /// closing price, volume, MACD signal etc. 
    /// </summary>
    public interface IStockSerieRel {
        /// <summary>
        /// Reference to stock.
        /// </summary>
        int StockId { get; set; }
        /// <summary>
        /// Reference to serie.
        /// </summary>
        int SerieId { get; set; }
    }

    public class StockSerieRel : IStockSerieRel {
        public int StockId { get; set; }
        public int SerieId { get; set; }
    }
}
