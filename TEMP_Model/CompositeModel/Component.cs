using ED.Atlas.Service.IC.BE.Messages;

namespace ED.Atlas.Service.IC.BE.ModelHandlers.CompositeModel {

    // Delegate for event handling of change in a branch of the composite tree.
    public delegate void ComponentChangedEventHandler(Component component);

    // Composite and Leaf heritates from Component that implements the common functionality
    // for Leaf and Composite.
    public abstract class Component : IComponent {
        public Component(string name) {
            Name = name;
        }
        protected ComponentTypeType _type;
        public ComponentTypeType Type { get { return _type; }
        }
        private event ComponentChangedEventHandler ComponentChangedHandler;
        public string Name { get; set; }
        public void AddChangedListener(ComponentChangedEventHandler eventHandler) {
            ComponentChangedHandler += eventHandler;
        }
        public void RemoveChangedListener(ComponentChangedEventHandler eventHandler) {
            ComponentChangedHandler -= eventHandler;
        }
        public abstract ComponentType ToComponentType();
        public void Onchanged(Component component) {
            if(ComponentChangedHandler != null) {
                ComponentChangedHandler(component);
            }
        }
    }
}
