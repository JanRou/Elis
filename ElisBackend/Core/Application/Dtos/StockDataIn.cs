namespace ElisBackend.Core.Application.Dtos {

    public class StockDataIn(string isin, string timeseriename)
    {
        public string Isin { get; private set; } = isin;
        public string TimeSerieName { get; private set; } = timeseriename;
    }
}
