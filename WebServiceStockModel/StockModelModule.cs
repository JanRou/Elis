using Nancy;

namespace WebServiceStockModel {
    public class StockModelModule : NancyModule {
        public StockModelModule(IRegistry registry) {

            Get["/v1/{path*}"] = parameters => {
                registry.CallCounter++;
                return   "<html><head><title>Response</title></head><body><p>"+ parameters.path + "</p></body></html>";
            };

            Post["/v1/{path*}/{dto}"] = parameters => {
                string path = parameters.path;
                string dto = parameters.dto;
                // HER TIl JSON deserialisering af dto
                return HttpStatusCode.OK;
            };
                
        }
    } 
}
