namespace ElisBackend.Core.Domain.Entities.Filters {
    public class FilterStock : FilterBase {
        public string Name { get; set; }
        public string Isin { get; set; }
        public string CurrencyCode { get; set; }
        public string ExchangeName { get; set; }
        public string OrderBy { get; set; }
    }
}
