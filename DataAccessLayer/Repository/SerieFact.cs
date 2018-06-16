using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Tables {

    /// <summary>
    /// Holds actual value over time for a serie.
    /// </summary>
    public interface ISerieFact : IId {
        /// <summary>
        /// Time for the value in UTC.
        /// </summary>
        DateTime Time { get; set; }
        /// <summary>
        /// reference to serie.
        /// </summary>
        int SerieId { get; set; }
        /// <summary>
        /// The value shifted as described in serie's ValueType.
        /// </summary>
        int Value { get; set; }
    }

    public class SerieFact : ISerieFact {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public int SerieId { get; set; }
        public int Value { get; set; }
    }
}
