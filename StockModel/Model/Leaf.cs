using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockModel.Model {
    public interface ILeaf : IComponent {        
        object Item { get; set; }
    }
    public class Leaf<T> : Component, ILeaf {
        private T _value; // only used when an interceptior is not present

        public Leaf(string name) : base(name) {
            _value = default(T);
            Interceptor = null;
            _type = ComponentType.Leaf;
        }

        public Leaf(string name, ILeafInterceptor<T> leafInterceptor) : base(name) {
            _value = default(T);
            Interceptor = leafInterceptor;
        }

        /// <summary>
        /// Interceptor catches get and set on the leaf's value so it can do something else
        /// than setting og getting the value like get or set a value in the database.
        /// </summary>
        public ILeafInterceptor<T> Interceptor { get; set; }

        /// <summary>
        /// Property value for the leaf. If the interceptor is not null, then is the interceptors
        /// get and set called instead.
        /// </summary>
        public T Value {
            get {
                return Interceptor != null ? Interceptor.Get() : _value;
            }
            set {
                if (Interceptor != null) {
                    Interceptor.Set(value);
                }
                else {
                    _value = value;
                }
                Onchanged(this);
            }
        }

        public object Item {
            get { return this.Value; }
            set { this.Value = (T) value; }
        }
    }
}
