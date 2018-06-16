using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Tables {

    /// <summary>
    /// Serie desribes a timeserie of data.
    /// </summary>
    public interface ISerieDim : IId {
        /// <summary>
        /// Name of the serie.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Reference to type of value including value shift.
        /// </summary>
        int ValueId { get; set; }
        /// <summary>
        /// Reference to currency.
        /// </summary>
        int CurrencyId { get; set; }
        /// <summary>
        /// Reference to attribute of serie.
        /// </summary>
        int SerieAttributeId { get; set; }
    }

    public class SerieDim : ISerieDim {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DatasourceId { get; set; }
        public int ValueId { get; set; }
        public int CurrencyId { get; set; }
        public int SerieAttributeId { get; set; }
    }
}
