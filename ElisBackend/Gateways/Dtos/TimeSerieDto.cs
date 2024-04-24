using ElisBackend.Domain.Entities;
using ElisBackend.Gateways.Repositories.Daos;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ElisBackend.Gateways.Dtos {
 
    public class TimeSerieDataDto {
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public decimal Volume { get; set; }
    }

    public class TimeSerieDto {
        public string StockName { get; set; }
        public string StockIsin { get; set; }
        public List<TimeSerieDataDto> TimeSerieData { get; set; }
    }

}
