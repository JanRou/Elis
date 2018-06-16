using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Tables {

    /// <summary>
    /// Name for the type of value like TotalVolume, closing price etc.
    /// And, it defines the Valueshift, which is in the potence of 10. A closing price with
    /// a ValueShift of 2 means the timeseries values are represented as 
    /// price*100 = price * (10^2). So a value in the timeserie of 17950 for
    /// the closing prices means a price of 179,50. Here are some examples of valueshift:
    /// ValueShift  Multiplier
    /// -2          0,01
    /// -1          0,1
    /// 0           1
    /// 1           10
    /// 2           100
    /// 3           1000
    /// </summary>
    public interface IValuetype : IId {
        string Name { get; set; }
        int ValueShift { get; set; }
    }

    public class Valuetype : IValuetype {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ValueShift { get; set; }
    }
}
