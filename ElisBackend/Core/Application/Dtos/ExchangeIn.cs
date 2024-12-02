namespace ElisBackend.Core.Application.Dtos {
    public class ExchangeIn(string name, string country, string url) {
        public string Name { get; set; } = name;
        public string Country { get; set; } = country;
        public string Url { get; set; } = url;
    }
}
