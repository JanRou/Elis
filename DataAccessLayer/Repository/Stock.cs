using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Tables {

    /// <summary>
    /// Describes a stock.
    /// </summary>
    public interface IStock : IId {
        /// <summary>
        /// Name of the stock.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Short name of the stock.
        /// </summary>
        string Shortname { get; set; }
        /// <summary>
        /// reference to market.
        /// </summary>
        int MarketId { get; set; }
    }

    public class Stock : IStock {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Shortname { get; set; }
        public int MarketId { get; set; }
    }
}
