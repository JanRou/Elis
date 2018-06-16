using System.Collections.Generic;

namespace ED.Atlas.Service.IC.BE.ModelHandlers.CompositeModel {
    public class TreeString {
        public TreeString() {
            Value = "";
        }
        public int Level { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public interface IModel {
        // TODO Program function that traverse the entire model and returns the tree witn names and
        // TODO types, but not the content.
        /// <summary>
        /// Gets a component with the model root as root in the tree.
        /// </summary>
        /// <param name="path">Reference to the component in the three.</param>
        /// <returns></returns>
        Component Get(string path);
        /// <summary>
        /// Get the component in the tree with root node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        Component Get(Composite node, string path);
        /// <summary>
        /// Adds a component to the tree.
        /// </summary>
        /// <param name="path">Path in the tree to the composite node.</param>
        /// <param name="component">Compnent to add.</param>
        void Set(string path, Component component);
        /// <summary>
        /// Adds a component to the tree which root is node. This is a relatively Set.
        /// </summary>
        /// <param name="node">Root of the tree that a component is added.</param>
        /// <param name="path">Path in tree with the node as root.</param>
        /// <param name="component">Compnent to add.</param>
        void Set(Composite node, string path, Component component);
        /// <summary>
        /// Subscribe an event handler to the node specified by path.
        /// </summary>
        /// <param name="path">Path to node.</param>
        /// <param name="eventHandler">The event handler to add.</param>
        /// <returns></returns>
        bool Subscribe(string path, ComponentChangedEventHandler eventHandler);
        /// <summary>
        /// Unsubscribe event handler from node.
        /// </summary>
        /// <param name="path">Path to node.</param>
        /// <param name="eventHandler">Event handler to remove.</param>
        /// <returns></returns>
        bool Unsubscribe(string path, ComponentChangedEventHandler eventHandler);
        /// <summary>
        /// Set the intecep class for the leaf specified by path.
        /// </summary>
        /// <typeparam name="T">Class implementing the interceptor interface.</typeparam>
        /// <param name="path">Path to leaf.</param>
        /// <param name="interceptor">instance of class for intercepting.</param>
        /// <returns>Returns true when interception is mounted on leaf.</returns>
        bool SetInterception<T>(string path, IInterceptor<T> interceptor);
        /// <summary>
        /// Set the intecep class for the leaf specified by path in the tree with root node.
        /// </summary>
        /// <typeparam name="T">Class implementing the interceptor interface.</typeparam>
        /// <param name="rootNode">Node which is the root of the tree.</param>
        /// <param name="path">Path to leaf.</param>
        /// <param name="interceptor">instance of class for intercepting.</param>
        /// <returns>Returns true when interception is mounted on leaf.</returns>
        bool SetInterception<T>(Composite rootNode, string path, IInterceptor<T> interceptor);
        /// <summary>
        /// Traverse the model and returns a level and component name.
        /// </summary>
        /// <returns>Collection of tree strings holdin </returns>
        IEnumerable<TreeString> ToTreeString();
    }
}
