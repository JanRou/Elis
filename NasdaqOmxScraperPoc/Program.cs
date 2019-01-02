using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NasdaqOmxScraper {
    class Program {
        static void Main(string[] args) {


            string url = "http://www.nasdaqomxnordic.com/aktier";

            var markets = new List<string>() { "CPH" };
            var segments = new List<string>() { "midCap", "smallCap" }; // "largeCap is already selected by default"
            var nasdaqStockQuotes = new NasdaqStockQuotes();
            var scrapeStockList = new ScrapeStockList(url);
            foreach ((string code, string name) stock in scrapeStockList.GetNasdaqOmxStocklist("nordicShares", markets, segments)) {
                // Get prices for stocks scraped
                // Get or create stock in store
                // TODO ...
                // 
                //if (dto != null) {
                //    foreach ((double price, double highPrice, double lowPrice, double volume, DateTime date) price in dto.Prices()) {
                // TODO either create bulk list or store price set one by one in store ...       
                //    }
                //}

                // TEST
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Temp\" + $"{stock.code}" + "Prices.txt")) {
                    file.WriteLine($"{stock.name}({stock.code})");
                    NasdaqPricesDto dto = nasdaqStockQuotes.GetStockQuotes(stock.code, new DateTime(1980, 02, 25));
                    if (dto != null) {
                        foreach ((double price, double highPrice, double lowPrice, double volume, DateTime date) price in dto.Prices()) {
                            file.WriteLine($"{price.date.ToString("yyyy-MM-dd")}; {price.price}; {price.volume}");
                        }
                    }
                    else {
                        Console.WriteLine($"No data (Nasdaq error) for {stock.name}");
                        file.WriteLine("No data Nasdaq returned error");
                    }
                }
                // END TEST
            }
        }
    }
}
