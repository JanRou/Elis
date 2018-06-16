using ED.Atlas.Service.IC.BE.Handlers;
using ED.Atlas.Service.IC.BE.Messages;
using ED.Atlas.Service.IC.BE.ModelHandlers.CompositeModel;

namespace ED.Atlas.Service.IC.BE.ModelHandlers {
    /// <summary>
    /// generic implementation of IIntradayInterceptorFactory. The factory creates an instance of class
    /// TInterceptor, The factory sets properties for the instance, so it uses the right database and
    /// Id. The TInterceptor class gets and sets the generic class TValue.
    /// </summary>
    public class IntradayInterceptorFactory : IIntradayInterceptorFactory {
        /// <summary>
        /// Creates an instance of the inceptor class with concrete values for id and handler.
        /// </summary>
        /// <typeparam name="TValue">Type of leaf</typeparam>
        /// <typeparam name="TInterceptor">Class which implements interceptor interface.</typeparam>
        /// <param name="handleDb">Handler to the database. It's a protperty that's application specific.</param>
        /// <param name="map">The Map of properties used in the interceptor to select right value.</param>
        /// <returns></returns>
        public IInterceptor<TValue> Create < TValue, TInterceptor >(IHandleDb handleDb, Map map) where TInterceptor
                : IIntradayInterceptorProperties, IInterceptor<TValue>, new() {
            return new TInterceptor { HandleDb = handleDb, Map = map };
        }
    }
    public interface IIntradayInterceptorFactory {
        /// <summary>
        /// The interface for the Intraday interceptor-factory of interceptors to the Composite structure.
        /// The interceptorers hav eto use a handler to the database and an Id.
        /// </summary>
        /// <typeparam name="TValue">Value class in Leaf, whic the Interceptor have to set and get.</typeparam>
        /// <typeparam name="TInterceptor">Interceptor class, whick the factory creates instance of.</typeparam>
        /// <param name="handleDb">Handler to the database.</param>
        /// <param name="map">Map of values in the database.</param>
        /// <returns>Interceptor instance, which may be used with concrete database.</returns>
        IInterceptor<TValue> Create<TValue, TInterceptor>(IHandleDb handleDb, Map map) where TInterceptor
            : IIntradayInterceptorProperties, IInterceptor<TValue>, new();
    }
    /// <summary>
    /// Interface for the Inceptor-class that have to implement to propeties, which the factory sets. The properties are
    /// HandleDb for the the Intraday database and Map for values from the database. 
    /// </summary>
    public interface IIntradayInterceptorProperties {
        IHandleDb HandleDb { get; set; }
        Map Map { get; set; }
    }
}
