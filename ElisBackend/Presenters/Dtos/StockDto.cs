using ElisBackend.Core.Domain.Abstractions;
using ElisBackend.Core.Domain.Entities;

namespace ElisBackend.Presenters.Dtos {
    public class StockDto {
        public string Name { get; set; }
        public string Isin { get; set; }
        public string ExchangeUrl { get; set; }
    }
}
