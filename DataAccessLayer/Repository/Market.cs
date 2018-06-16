using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Tables {

    public interface IMarket : IId {
        string Name { get; set; }
        string Shortname { get; set; } // empty or nullable?
        string Provider { get; set; }
        int CountryId { get; set; }
    }

    public class Market : IMarket {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Shortname { get; set; } // empty or nullable?
        public string Provider { get; set; }
        public int CountryId { get; set; }
    }
}
