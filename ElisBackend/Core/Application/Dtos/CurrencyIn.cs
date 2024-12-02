namespace ElisBackend.Core.Application.Dtos {
    public class CurrencyIn(string name, string code) {
        public string Name { get; set; } = name;
        public string Code { get; set; } = code;
    }
}
