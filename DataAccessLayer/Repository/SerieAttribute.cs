using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Tables {

    /// <summary>
    /// Describes the attribute for a serie.
    /// </summary>
    public interface ISerieAttribute : IId {
        /// <summary>
        /// Name of attribute.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Descripton of attribute.
        /// </summary>
        string Description { get; set; }
    }

    public class SerieAttribute : ISerieAttribute {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
