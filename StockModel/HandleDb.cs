using DataAccessLayer.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace StockModel {

    public interface IHandleDb {
        List<IStock> GetStocks();
    }

    public class HandleDb : IHandleDb {
        public List<IStock> GetStocks() {

            return null;
        }
    }
}
