using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace WebServiceStockModel {

    public class WinService {

        private readonly IWebServiceHost _webServiceHost;
        public WinService(IWebServiceHost webServiceHost) {
            _webServiceHost = webServiceHost;
            UserInteractive = false;
        }
        public string Description { get; set; }
        public string DislpayName { get; set; }
        public string ServiceName { get; set; }
        public bool UserInteractive { get; set; }

        public void Run() {
            
            HostFactory.Run(x => {
                x.UseLog4Net();
                x.Service<WinService>(s => {
                    s.ConstructUsing(() => this);
                    s.WhenStarted( (ws, hc) => { return ws.Start(hc); } );
                    s.WhenStopped( ws => ws.Stop() );
                });
                x.RunAsLocalSystem();
                x.SetDescription(Description);
                x.SetDisplayName(DislpayName);
                x.SetServiceName(ServiceName);
                x.StartManually();
            });
        }

        public bool Start(HostControl hostControl) {
            _webServiceHost.Start();
            if (UserInteractive) {
                _webServiceHost.DebugRun();
                hostControl.Stop();
            }
            return true;
        }

        public void Stop() {
            _webServiceHost.Stop();
            if (UserInteractive) {
                _webServiceHost.DebugStop();
            }
        }
    }
}