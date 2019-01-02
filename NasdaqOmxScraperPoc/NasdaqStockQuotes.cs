using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace NasdaqOmxScraper {

    public class NasdaqStockQuotes {

        // Request looks like this:
        //"http://www.nasdaqomxnordic.com/webproxy/DataFeedProxy.aspx?" +
        //        "SubSystem=History&Action=GetChartData&inst.an=id%2Cnm%2Cfnm%2Cisin%2Ctp%2Cchp%2Cycp&FromDate=1986-01-01&ToDate=2018-12-07&" +
        //        "json=true&showAdjusted=true&app=%2Faktier%2Fmicrosite-MicrositeChart-history&timezone=CET&DefaultDecimals=false&Instrument=CSE3331"

        // Date format: 1986-01-01
        // instrument: CSE3331

        private readonly string _urlTemplate;
        private readonly string _dateFormat;

        public NasdaqStockQuotes() {

            //_urlTemplate = "http://www.nasdaqomxnordic.com/webproxy/DataFeedProxy.aspx?" +
            //    "SubSystem=History&Action=GetChartData&inst.an=id%2Cnm%2Cfnm%2Cisin%2Ctp%2Cchp%2Cycp&FromDate={0}&ToDate={1}&" +
            //    "json=true&showAdjusted=true&app=%2Faktier%2Fmicrosite-MicrositeChart-history&timezone=CET&DefaultDecimals=false&Instrument={2}";

            _urlTemplate = "http://www.nasdaqomxnordic.com/WebAPI/api/HighChartsAnalysisTool/GetAnalysisHighChartData?" +
                "Instrument={0}&FromDate={1}&Function=Volume&app=%2Fshares%2Fanalysistool&datasource=prod";

            // Date: 19800225
            _dateFormat = "yyyyMMdd";

        }

        public NasdaqPricesDto GetStockQuotes(string instrument, DateTime fromDate ) {

            string url = string.Format(_urlTemplate, instrument, fromDate.ToString(_dateFormat));
            GetNasdaqStockPricesInJson(url);

            // Convert JSON to dto
            var result = new NasdaqPricesDto();
            var jsonSettings = new JsonSerializerSettings() {
                NullValueHandling = NullValueHandling.Ignore,
                FloatParseHandling = FloatParseHandling.Double
            };
            result = JsonConvert.DeserializeObject<NasdaqPricesDto>( GetNasdaqStockPricesInJson(url), jsonSettings);
            return result;
        }

        public string GetNasdaqStockPricesInJson(string url) {
            HttpWebResponse response = null;
            Stream dataStream = null;
            StreamReader reader = null;
            string result = "";
            try {
                // Create a request for the URL. 		
                WebRequest request = WebRequest.Create(url);
                // If required by the server, set the credentials.
                request.Credentials = CredentialCache.DefaultCredentials;
                // Get the response.
                response = (HttpWebResponse)request.GetResponse();
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                reader = new StreamReader(dataStream);
                result = reader.ReadToEnd();
            }
            catch (Exception excep) {

            }
            finally {
                reader?.Close();
                dataStream?.Close();
                response?.Close();
            }
            return result;
        }

        // TEST
        //public void UseNewtonJsonPoc() {
        //    NasdaqPricesDto stockPrices;
        //    var jsonSettings = new JsonSerializerSettings() {
        //        NullValueHandling = NullValueHandling.Ignore,
        //        FloatParseHandling = FloatParseHandling.Double
        //    };            
            
        //    using (System.IO.StreamReader reader = new System.IO.StreamReader(@"C:\Temp\TestQuotes.json")) {
        //        stockPrices = JsonConvert.DeserializeObject<NasdaqPricesDto>(reader.ReadToEnd(), jsonSettings);
        //    }
        //}
    }
}
