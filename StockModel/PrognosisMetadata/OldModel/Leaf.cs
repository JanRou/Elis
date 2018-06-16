using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ED.Wp3.Server.BE.PrognosisMetadata.Model
{
    public interface ILeaf
    {
        object Item { get; set; }
    }
    public class Leaf<T> : Component, ILeaf
    {
        private T _value; // only used when an interceptior is not present

        public Leaf(string name) : base(name)
        {
            _value = default(T);
            LeafInterceptor = null;
            _type = ComponentType.Leaf;
        }

        public Leaf(string name, ILeafInterceptor<T> leafInterceptor) : base(name)
        {
            _value = default(T);
            LeafInterceptor = leafInterceptor;
        }

        /// <summary>
        /// Interceptor catches get and set on the leaf's value so it can do something else
        /// than setting og getting the value i.ex. get or set a value in the database.
        /// </summary>
        public ILeafInterceptor<T> LeafInterceptor { get; set; }

        /// <summary>
        /// Property value for the leaf. If the interceptor is set, then is the interceptors get and 
        /// set called instead.
        /// </summary>
        public T Value
        {
            get {
                return LeafInterceptor != null ? LeafInterceptor.Get() : _value;
            }
            set
            {
                if (LeafInterceptor != null)
                {
                    LeafInterceptor.Set(value);
                }
                else
                {
                    _value = value;
                }
                Onchanged(this);
            }
        }

        public object Item
        {
            get { return this.Value; }
            set { this.Value = Value; }
        }
    }
}
