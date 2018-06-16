using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ED.Wp3.Server.BE.PrognosisMetadata.Model
{
    // enum to signal type of component
    public enum ComponentType
    {
            Compo               // Composite classic
        ,   Leaf                // Leaf classic
        ,   CompoInterceptor    // On the fly constructed composite
    }

    // Delegate for event handling of change in a branch of the composite tree.
    public delegate void ComponentChangedEventHandler(IComponent component);

    public interface IComponent
    {
        /// <summary>
        /// Name of the component in the tree.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Gets the type of the component Compo or Leaf.
        /// </summary>
        ComponentType Type { get; }
        /// <summary>
        /// Adds a event handler to listener
        /// </summary>
        /// <param name="EventHandler">Event handler to call</param>
        void AddChangedListener(ComponentChangedEventHandler eventHandler);
        /// <summary>
        /// Remove a event handler form listener
        /// </summary>
        /// <param name="eventHandler">Event handler</param>
        void RemoveChangedListener(ComponentChangedEventHandler eventHandler);
        /// <summary>
        /// Creates a composite on the fly. This means the composite may be created
        /// based on results from the database. 
        /// </summary>
        void Create();

        void Onchanged(IComponent component);
    }

    public abstract class Component : IComponent
    {
        public Component(string name) {
            Name = name;
        }

        private event ComponentChangedEventHandler ComponentChangedHandler;

        protected ComponentType _type;
        public ComponentType Type { get { return _type; } }
        public string Name { get; set; }

        public void AddChangedListener(ComponentChangedEventHandler eventHandler) {
            ComponentChangedHandler += eventHandler;
        }

        public void RemoveChangedListener(ComponentChangedEventHandler eventHandler) {
            ComponentChangedHandler -= eventHandler;
        }

        public void Onchanged(IComponent component) {
            ComponentChangedHandler?.Invoke(component);
        }

        public virtual void Create() { }
    }
}
