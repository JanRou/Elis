using Nancy.Hosting.Self;
using System;
using log4net;
using log4net.Config;
using Nancy.TinyIoc;
using Nancy;
using Nancy.Bootstrapper;
using DataAccessLayer;


// TODO: add url
//  netsh http add urlacl url=http://+:8080/ user=Jan

// TODO: remove url
//  netsh http delete urlacl url=http://+:8080/

namespace WebServiceStockModel {

    public class Program {

        private static readonly string _url = "http://localhost:8080";

        private static readonly ILog _log = LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // TODO LOG i DATABASE

        public static Registry Registry;

        static void Main(string[] args) {

            XmlConfigurator.Configure();

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

    public class CustomBootstrapper : DefaultNancyBootstrapper {
        protected override void ApplicationStartup(TinyIoCContainer container
            , IPipelines pipelines) {
            container.Register<IRegistry, Registry>(Program.Registry);
            container.Register<IProgramSettings, ProgramSettings>();
        }
    }

}