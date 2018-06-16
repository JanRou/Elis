using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;

namespace DataAccessLayer.Tables {
    public interface ICountry : IId {
        string Name { get; set; }
        string Shortname2letter { get; set; }
        string Shortname3letter { get; set; }
        int CurrencyId { get; set; }
    }

    public class Country : ICountry {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Shortname2letter { get; set; }
        public string Shortname3letter { get; set; }
        public int CurrencyId { get; set; }
    }
}
