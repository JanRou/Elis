using ED.Atlas.Service.IC.BE.Handlers;
using ED.Atlas.Service.IC.BE.Messages;
using ED.Atlas.Service.IC.BE.ModelHandlers.CompositeModel;

namespace ED.Atlas.Service.IC.BE.ModelHandlers {
    /// <summary>
    /// Interceptor to read a front ends configuration in the database given the DataProvider Id.
    /// </summary>
    public class ConfigurationInterceptor : IInterceptor<Map>, IIntradayInterceptorProperties {
        public Map Map { get; set; }
        public IHandleDb HandleDb { get; set; }
        public Map Get() {
            FrontEndConfiguration dbConfiguration = HandleDb.GetFrontEndConfiguration(Map.Get<int>("DataProviderId"));
            var confgiMap = new Map();
            if(!confgiMap.Create(dbConfiguration.ConfigXml)) {
                confgiMap = null;
            }
            return confgiMap;
        }
        public void Set(Map val) {
            HandleDb.SetFrontEndConfiguration( Map.Get<int>("DataProviderId"), val.ToXml());
        }
    }
}
