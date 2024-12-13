namespace ElisBackend.Core.Domain.Entities.Filters
{
    public class FilterTimeSerieFacts : FilterBase
    {
        public string Name { get; set; }
        public string Isin { get; set; }
        public string TimeSeriesName { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }

}
