using System;

namespace CompositeModel {
    public interface ILeaf {
        LeafBaseType GetItemType();
    }

    public class Leaf<T> : Component, ILeaf {

        private T _value; // only used when an interceptior is not present

        public Leaf(string name) : base(name) {
            _value = default(T);
            Interceptor = null;
            _type = ComponentBaseType.Leaf;
        }

        public Leaf(string name, IInterceptor<T> interceptor) : base(name) {
            _value = default(T);
            Interceptor = interceptor;
        }

        /// <summary>
        /// Interceptor catches get and set on the leaf's value so it can do something else
        /// than setting og getting the value i.ex. get or set a value in the database.
        /// </summary>
        public IInterceptor<T> Interceptor { get; set; }

        /// <summary>
        /// Property value for the leaf. If the interceptor is set, then is the interceptors get and 
        /// set called instead.
        /// </summary>
        public T Value {
            get {
                if (Interceptor != null) {
                    return Interceptor.Get();
                }
                else {
                    return _value;
                }
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

        public override string ToString() {
            return Value.ToString();
        }
        /// <summary>
        /// Gets the base type of the leaf
        /// </summary>
        /// <returns>XSD defined base type</returns>
        public LeafBaseType GetItemType() {
            // TODO: _value.GetType and parse enum to make this a one liner
            LeafBaseType ret = LeafBaseType.@string;
            if (_value is int) {
                ret = LeafBaseType.@int;
            }
            else if (_value is double) {
                ret = LeafBaseType.@double;
            }
            else if (_value is decimal) {
                ret = LeafBaseType.@decimal;
            }
            else if (_value is DateTime) {
                ret = LeafBaseType.datetime;
            }
            else if (_value is uint) {
                ret = LeafBaseType.positiveInteger;
            }
            else if (_value is long) {
                ret = LeafBaseType.@long;
            }
            return ret;
        }
    }

    //public class LeafBase : Component {

    //    private object _value; // only used when an interceptior is not present

    //    public LeafBase(string name) : base(name) {
    //        _value = null;
    //        Interceptor = null;
    //        _type = ComponentBaseType.Leaf;
    //    }

    //    public LeafBase(string name, IInterceptor<object> interceptor) : base(name) {
    //        _value = null;
    //        Interceptor = interceptor;
    //    }

    //    /// <summary>
    //    /// Interceptor catches get and set on the leaf's value so it can 
    //    /// get or set a value in the database.
    //    /// </summary>
    //    public IInterceptor<object> Interceptor { get; set; }

    //    /// <summary>
    //    /// Property value for the leaf. If the interceptor is set, then is the interceptors get and 
    //    /// set called instead.
    //    /// </summary>
    //    public Object Value {
    //        get {
    //            if (Interceptor != null) {
    //                return Interceptor.Get();
    //            }
    //            else {
    //                return _value;
    //            }
    //        }
    //        set {
    //            if (Interceptor != null) {
    //                Interceptor.Set(value);
    //            }
    //            else {
    //                _value = value;
    //            }
    //            Onchanged(this);
    //        }
    //    }

    //    public override string ToString() {
    //        return Value.ToString();
    //    }
    //    /// <summary>
    //    /// Gets the base type of the leaf
    //    /// </summary>
    //    /// <returns>Defined base type</returns>
    //    public virtual LeafBaseType GetItemType() {
    //        // TODO: _value.GetType and parse enum to make this a one liner
    //        LeafBaseType ret = LeafBaseType.@string;
    //        if (_value is int) {
    //            ret = LeafBaseType.@int;
    //        }
    //        else if (_value is double) {
    //            ret = LeafBaseType.@double;
    //        }
    //        else if (_value is decimal) {
    //            ret = LeafBaseType.@decimal;
    //        }
    //        else if (_value is DateTime) {
    //            ret = LeafBaseType.datetime;
    //        }
    //        else if (_value is uint) {
    //            ret = LeafBaseType.positiveInteger;
    //        }
    //        else if (_value is long) {
    //            ret = LeafBaseType.@long;
    //        }
    //        return ret;
    //    }

    //}

}
