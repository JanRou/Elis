namespace ElisBackend.Core.Application.Dtos {

    public class StockDataIn(string isin, string timeseriename)
    {
        public string Isin { get; set; } = isin;
        public string TimeSerieName { get; set; } = timeseriename;
    }
}
