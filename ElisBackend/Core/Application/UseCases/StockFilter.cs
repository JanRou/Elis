using System.Data;

namespace ElisBackend.Core.Application.UseCases
{
    // TODO der mangler sortering
    public class StockFilter
    {
        public string Name { get; set; }
        public string Isin { get; set; }
        public string CurrencyShort { get; set; }
        public string ExchangeName { get; set; }
        public int Take { get; set; } // = 0 betyder hent alle fundne
        public int Skip { get; set; }
    }
}
