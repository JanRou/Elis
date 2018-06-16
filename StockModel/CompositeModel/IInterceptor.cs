namespace CompositeModel {
    /// <summary>
    /// An interceptor intercepts get and set operations on a leaf's value. The value
    /// type is generic, though it is limited to a base type and leafMap.
    /// The get and set operations redirects the interceptor to calls to other data sources 
    /// like a database access layer.
    /// </summary>
    /// <typeparam name="TValue">Værdi typen som interceptoren skal hente og sætte.</typeparam>
    public interface IInterceptor<TValue> {
        /// <summary>
        /// Gets a value.
        /// </summary>
        /// <returns>The value</returns>
        TValue Get();
        /// <summary>
        /// Sets a value.
        /// </summary>
        /// <param name="val">The value</param>
        void Set(TValue val);
    }
}
