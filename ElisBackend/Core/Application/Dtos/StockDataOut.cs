namespace ElisBackend.Core.Application.Dtos {
    public class StockDataOut(string isin, string timeSerieName, int countTimeSerieFacts) {
        public string Isin { get; set; } = isin;
        public string TimeSerieName { get; set; } = timeSerieName;
        public int CountTimeSerieFacts { get; set; } = countTimeSerieFacts;
    }
}
