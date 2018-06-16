using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ED.Wp3.Server.BE.PrognosisMetadata
{
    public class PrognosisMetadataWeatherDto
    {
        public int WeatherProviderId { get; set; }
        public string WeatherProviderName { get; set; }
        public DateTime CalculationTime { get; set; }
        public DateTime ForecastStart { get; set; }
        public DateTime ForecastEnd { get; set; }
        public DateTime Updated { get; set; }
    }
}
