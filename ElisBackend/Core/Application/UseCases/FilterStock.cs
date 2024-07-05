using System.Data;

namespace ElisBackend.Core.Application.UseCases
{
    public class FilterBase {
        public int Take { get; set; } // = 0 betyder hent alt
        public int Skip { get; set; }
    }

    public class FilterStock : FilterBase {
        public string Name { get; set; }
        public string Isin { get; set; }
        public string CurrencyCode { get; set; }
        public string ExchangeName { get; set; }
        public string OrderBy { get; set; }
    }


}
