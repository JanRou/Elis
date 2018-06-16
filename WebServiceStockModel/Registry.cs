using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceStockModel {
    public interface IRegistry {
        int CallCounter { get; set; }
        string Url { get; set; }
    }
    public class Registry : IRegistry {

        public int CallCounter { get; set;  }
        public string Url { get; set; }
    }
}
