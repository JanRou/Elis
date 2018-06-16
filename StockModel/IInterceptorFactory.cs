
using StockModel.Model;

namespace StockModel {
    /// <summary>
    /// Generic implementation of IInterceptorFactory. The factory creates an instance of class
    /// TInterceptor, The factory sets properties for the instance, so it uses the right database and
    /// Id. The TInterceptor class gets and sets the generic class TValue.
    /// </summary>
    public class InterceptorFactory : IInterceptorFactory {
        /// <summary>
        /// Creates an instance of the inceptor class with concrete values for id and handler.
        /// </summary>
        /// <typeparam name="TValue">Type of leaf</typeparam>
        /// <typeparam name="TInterceptor">Class which implements interceptor interface.</typeparam>
        /// <param name="composite">The Composite of properties used in the interceptor to select right value.</param>
        /// <returns></returns>
        public ILeafInterceptor<TValue> Create < TValue, TInterceptor >(Composite composite) 
                where TInterceptor : IInterceptorProperties, ILeafInterceptor<TValue>, new() {
            return new TInterceptor { Composite = composite };
        }
    }
    public interface IInterceptorFactory {
        /// <summary>
        /// The interface for the interceptor-factory of interceptors to the Composite structure.
        /// The interceptorers have to use a handler to the database and an Id.
        /// </summary>
        /// <typeparam name="TValue">Value class in Leaf, whic the Interceptor have to set and get.</typeparam>
        /// <typeparam name="TInterceptor">Interceptor class, which the factory creates instance of.</typeparam>
        /// <param name="composite">Composite of values in the database.</param>
        /// <returns>Interceptor instance, which may be used with database (sql, xml or other source).</returns>
        ILeafInterceptor<TValue> Create<TValue, TInterceptor>(Composite composite) 
                    where TInterceptor : IInterceptorProperties, ILeafInterceptor<TValue>, new();
    }
    /// <summary>
    /// Interface for the Inceptor-class that have to implement to propeties, which the factory sets. The properties are
    /// HandleDb for the the database and Map for values from the database. 
    /// </summary>
    public interface IInterceptorProperties {
        Composite Composite { get; set; }
    }
}
