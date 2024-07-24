namespace ElisBackend.Core.Application.Dtos {
    public class TimeSerieDataIn(string date, decimal price, decimal volume)
    {
        public string Date { get; set; } = date;
        public decimal Price { get; set; } = price;
        public decimal Volume { get; set; } = volume;
    }
}
