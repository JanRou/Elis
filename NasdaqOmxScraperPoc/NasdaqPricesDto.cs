using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NasdaqOmxScraper {
    /*
        {
	        "prices":[ priser double ],
	        ,"highPrices":[ decimla tal],
	        "lowPrices":[double tal],
	        "volumes":[double tal],
	        "dates":[ double tal],
	        "compare1Prices":null
	        ,"compare2Prices":null
	        ,"compare3Prices":null
	        "timeTics":null,
	        "compareTimeTics":null,
	        "firstPrice":7347.9926520000008,
	        "intraDays":null,
	        "intraDayTimeSpan":0.0,
	        "xMin":0.0,
	        "xMax":0.0,
	        "betas":null,
	        "bollingerbandTop":null,
	        "bollingberbandBottom":null,
	        "correlations":null,
	        "macds":null,
	        "macdSignals":null,
	        "momentums":null,
	        "movingAverage1":null,
	        "movingAverage2":null,
	        "movingAverage3":null,
	        "oscillators":null,
	        "rsis":null,
	        "standardDeviations":null,
	        "firstStochastics":null,
	        "secondStochastics":null,
	        "thirdStochastics":null,
	        "volatilities":null
        }
     */


    //[IgnoreDataMember]
    //public DateTime MyDateTime { get; set; }

    //[DataMember(Name = "MyDateTime")]
    //private int MyDateTimeTicks {
    //    get { return (int)(this.MyDateTime - unixEpoch).TotalSeconds; }
    //    set { this.MyDateTime = unixEpoch.AddSeconds(Convert.ToInt32(value)); }
    //}

    public class NasdaqPricesDto {
        [JsonProperty("prices")]                public List<double>   RawPrices { get; set; }
        [JsonProperty("highPrices")]            public List<double>   RawHighPrices { get; set; }
        [JsonProperty("lowPrices")]             public List<double>   RawLowPrices { get; set; }
        [JsonProperty("volumes")]               public List<double>   RawVolumes { get; set; }
        [JsonProperty("dates")]                 public List<double>   RawDates { get; set; }        
        [JsonProperty("compare1Prices")]        public double         Compare1Prices { get; set; }
        [JsonProperty("compare2Prices")]        public double         Compare2Prices { get; set; }
        [JsonProperty("compare3Prices")]        public double         Compare3Prices { get; set; }
        [JsonProperty("timeTics")]              public double         RimeTics { get; set; }
        [JsonProperty("compareTimeTics")]       public double         CompareTimeTics { get; set; }
        [JsonProperty("firstPrice")]            public double         FirstPrice { get; set; }
        [JsonProperty("intraDays")]             public double         IntraDays { get; set; }
        [JsonProperty("intraDayTimeSpan")]      public double         IntraDayTimeSpan { get; set; }
        [JsonProperty("xMin")]                  public double         XMin { get; set; }
        [JsonProperty("xMax")]                  public double         XMax { get; set; }
        [JsonProperty("betas")]                 public double         Betas { get; set; }
        [JsonProperty("bollingerbandTop")]      public double         BollingerbandTop { get; set; }
        [JsonProperty("bollingberbandBottom")]  public double         BollingberbandBottom { get; set; }
        [JsonProperty("correlations")]          public double         Correlations { get; set; }
        [JsonProperty("macds")]                 public double         Macds { get; set; }
        [JsonProperty("macdSignals")]           public double         MacdSignals { get; set; }
        [JsonProperty("momentums")]             public double         Momentums { get; set; }
        [JsonProperty("movingAverage1")]        public double         MovingAverage1 { get; set; }
        [JsonProperty("movingAverage2")]        public double         MovingAverage2 { get; set; }
        [JsonProperty("movingAverage3")]        public double         MovingAverage3 { get; set; }
        [JsonProperty("oscillators")]           public double         Oscillators { get; set; }
        [JsonProperty("rsis")]                  public double         Rsis { get; set; }
        [JsonProperty("standardDeviations")]    public double         StandardDeviations { get; set; }
        [JsonProperty("firstStochastics")]      public double         FirstStochastics { get; set; }
        [JsonProperty("secondStochastics")]     public double         SecondStochastics { get; set; }
        [JsonProperty("thirdStochastics")]      public double         ThirdStochastics { get; set; }
        [JsonProperty("volatilities")]          public double         Volatilities { get; set; }
        // TODO may be this can be done so Newton converts in to Dates directly
        private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        public List<DateTime> Dates {
            get {
                var result = new List<DateTime>();
                foreach (double rawDate in RawDates) {
                    result.Add(unixEpoch.AddMilliseconds(rawDate));
                }
                return result.Count > 0 ? result : null;
            }
        }
        public IEnumerable<(double price, double highPrice, double lowPrice, double volume, DateTime date)> Prices() {
            for (int i = 0; i < RawPrices.Count; i++) {
                yield return (
                    price: RawPrices[i], 
                    highPrice: RawHighPrices[i],
                    lowPrice: RawLowPrices[i],
                    volume: RawVolumes[i],
                    date: Dates[i]);
            }
        }
    }
}
