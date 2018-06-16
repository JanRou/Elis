using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Tables {
    public interface ICurrency : IId {
        string Name { get; set; }
        string Shortname { get; set; }
    }

    public class Currency : ICurrency {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Shortname { get; set; }
    }
}
