using System;
using ED.Atlas.Service.IC.BE.Messages;

namespace ED.Atlas.Service.IC.BE.ModelHandlers.CompositeModel {
    public class Leaf<T> : Component {
        private T _value; // only used when an interceptior is not present
        public Leaf(string name) : base(name) {
            _value = default(T);
            Interceptor = null;
            _type = ComponentTypeType.Leaf;
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
                if(Interceptor != null) {
                    return Interceptor.Get();
                }
                else {
                    return _value;
                }
            }
            set {
                if(Interceptor != null) {
                    Interceptor.Set(value);
                }
                else {
                    _value = value;
                }
                Onchanged(this);
            }
        }
        public override ComponentType ToComponentType() {
            Leaf xmlLeaf = new Leaf();
            xmlLeaf.Name = Name;
            xmlLeaf.Item = Value;
            xmlLeaf.ItemElementName = GetItemType();
            xmlLeaf.Type = ComponentTypeType.Leaf;
            return xmlLeaf;
        }
        /// <summary>
        /// Gets the base XML type of the leaf
        /// </summary>
        /// <returns>XSD defined base type</returns>
        public ItemChoiceType GetItemType() {
            ItemChoiceType ret = ItemChoiceType.@string;
            if(_value is int) {
                ret = ItemChoiceType.@int;
            }
            else if (_value is double) {
                ret = ItemChoiceType.@double;
            }
            else if(_value is decimal) {
                ret = ItemChoiceType.@decimal;
            }
            else if(_value is DateTime) {
                ret = ItemChoiceType.datetime;
            }
            else if(_value is uint) {
                ret = ItemChoiceType.positiveInteger;
            }
            else if (_value is long) {
                ret = ItemChoiceType.@long;
            }
            return ret;
        }
    }
}
