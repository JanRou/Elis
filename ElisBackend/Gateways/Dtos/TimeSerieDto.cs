namespace ElisBackend.Gateways.Dtos {
 
    public class TimeSerieDataDto {
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public decimal Volume { get; set; }
    }

    public class TimeSerieDto {
        public string StockName { get; set; }
        public string StockIsin { get; set; }
        public List<TimeSerieDataDto> TimeSerieData { get; set; }
    }

}
