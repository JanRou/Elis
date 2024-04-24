using System.ComponentModel.DataAnnotations.Schema;

namespace ElisBackend.Gateways.Repositories.Daos {
    public class TimeZoneDao(string name, int offset, DateTime fromUtc, DateTime toUtc, int exchangeId) {
        public int Id { get; set; }
        public string Name { get; set; } = name;
        public int Offset { get; set; } = offset;
        public DateTime FromUtc { get; set; } = fromUtc;
        public DateTime ToUtc { get; set; } = toUtc;

        [ForeignKey("Exchange")]
        public int ExchangeId { get; set; } = exchangeId;
        public virtual ExchangeDao Exchange { get; set; }

    }
}
