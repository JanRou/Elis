using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ED.Wp3.Server.BE.PrognosisMetadata.Model
{
    public enum ComponentType
    {
        Compo, Leaf, Error
    }

    // Delegate for event handling of change in a branch of the composite tree.
    public delegate void ComponentChangedEventHandler(Component component);

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
    }
    public abstract class Component : IComponent
    {
        public Component(string name)
        {
            Name = name;
        }

        private event ComponentChangedEventHandler ComponentChangedHandler;

        protected ComponentType _type;
        public ComponentType Type { get { return _type; } }
        public string Name { get; set; }

        public void AddChangedListener(ComponentChangedEventHandler eventHandler)
        {
            ComponentChangedHandler += eventHandler;
        }

        public void RemoveChangedListener(ComponentChangedEventHandler eventHandler)
        {
            ComponentChangedHandler -= eventHandler;
        }

        public void Onchanged(Component component)
        {
            ComponentChangedHandler?.Invoke(component);
        }
    }
}
