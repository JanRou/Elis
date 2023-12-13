using System;
using DataAccessLayer;


// TODO: add url
//  netsh http add urlacl url=http://+:8080/ user=Jan

// TODO: remove url
//  netsh http delete urlacl url=http://+:8080/

namespace WebServiceStockModel {

    public class Program {

        private static readonly string _url = "http://localhost:8080";


        // TODO LOG i DATABASE

        public static Registry Registry;

        static void Main(string[] args) {


            Registry = new Registry() {
                    Url = _url
                ,   CallCounter = 0
            };
            WebService webService = new WebService(Registry);
            WinService winService = new WinService(webService) {
                    Description = "Stock Model WebServer"
                ,   DislpayName = "Stock Model WebServer"
                ,   ServiceName = "StockModelWebServer"
                ,   UserInteractive = Environment.UserInteractive
            };
            winService.Run();
        }
    }

}