using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NasdaqOmxScraperPoc {
    class Program {
        static void Main(string[] args) {

            string url = "http://www.nasdaqomxnordic.com/aktier";
            SeleniumPoc nasdaq = new SeleniumPoc(url);
            nasdaq.GetNasdaqOmxStocklist();
            /*
            string url = "http://www.nasdaqomxnordic.com/webproxy/DataFeedProxy.aspx?" +
                "SubSystem=History&Action=GetChartData&inst.an=id%2Cnm%2Cfnm%2Cisin%2Ctp%2Cchp%2Cycp&FromDate=1986-01-01&ToDate=2018-12-07&" +
                "json=true&showAdjusted=true&app=%2Faktier%2Fmicrosite-MicrositeChart-history&timezone=CET&DefaultDecimals=false&Instrument=CSE3331";

            // returnerer json til en kurve med dagspriser siden 1986 for AMBU-B. Aktier kan findes på aktie-siden ved at gemme tabellen med aktier
         
            // Create a request for the URL. 		
            WebRequest request = WebRequest.Create(url);
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            // Display the status.
            Console.WriteLine(response.StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Store the content in file
            using (System.IO.StreamWriter file =new System.IO.StreamWriter(@"C:\Temp\Test.txt")) {
                file.Write(reader.ReadToEnd());
            }
            //string responseFromServer = reader.ReadToEnd();
            // Display the content.
            //Console.WriteLine(responseFromServer);
            // Cleanup the streams and the response.
            reader.Close();
            dataStream.Close();
            response.Close();
            */
        }
    }
}
