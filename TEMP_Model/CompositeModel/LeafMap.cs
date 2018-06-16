using ED.Atlas.Service.IC.BE.Messages;

namespace ED.Atlas.Service.IC.BE.ModelHandlers.CompositeModel {
    /// <summary>
    /// LeafMap behaves in Model like a leaf. In XmlCollector is LeafMap a composite that
    /// holds the map's (key,value) pair.
    /// </summary>
    public class LeafMap : Leaf<Map> {
        public LeafMap(string name)  : base(name){
            Value = new Map();
            Value.Name = Name;
        }
        public LeafMap(string name, IInterceptor<Map> interceptor) : base(name) {
            Interceptor = interceptor;
        }
        public override ComponentType ToComponentType() {
            Messages.Composite composite = Value.ToComposite();
            composite.Name = Name;  // Be sure the name is set
            return composite;
        }
    }
}


