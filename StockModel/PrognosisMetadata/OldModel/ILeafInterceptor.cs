using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ED.Wp3.Server.BE.PrognosisMetadata.Model
{
    /// <summary>
    /// A leaf interceptor intercepts get and set operations on a leaf's value. The value
    /// type is generic, though it is limited to a base type and leafMap.
    /// The get and set operations redirect the interceptor to calls to other data sources 
    /// like a database access layer.
    /// </summary>
    /// <typeparam name="TValue">The value type, which the interceptor have to get and set.</typeparam>
    public interface ILeafInterceptor<TValue>
    {
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
