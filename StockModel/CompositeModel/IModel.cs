using System.Collections.Generic;

namespace CompositeModel {

    public interface IModel {

        /// <summary>
        /// Gets a component in the model.
        /// </summary>
        /// <param name="path">Reference to the component in the model.</param>
        /// <returns></returns>
        Component Get(string path);

        /// <summary>
        /// Get the component in the tree with from node as root. This is a relative get.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        Component Get(Composite node, string path);

        /// <summary>
        /// Adds a component to the model specified by path.
        /// </summary>
        /// <param name="path">Path in the tree to the composite node.</param>
        /// <param name="component">Component to add.</param>
        void Set(string path, Component component);

        /// <summary>
        /// Adds a component to the model with node as root. This is a relative Set.
        /// </summary>
        /// <param name="node">Root of the tree that a component is added.</param>
        /// <param name="path">Path in tree with the node as root.</param>
        /// <param name="component">Component to add.</param>
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
        /// Set the intecept class for the leaf specified by path.
        /// </summary>
        /// <typeparam name="T">Class implementing the interceptor interface.</typeparam>
        /// <param name="path">Path to leaf from model root.</param>
        /// <param name="interceptor">instance of class for intercepting.</param>
        /// <returns>Returns true when interception is mounted on leaf.</returns>
        bool SetInterception<T>(string path, IInterceptor<T> interceptor);

        /// <summary>
        /// Set the intecept class for the leaf specified by path from the node.
        /// </summary>
        /// <typeparam name="T">Class implementing the interceptor interface.</typeparam>
        /// <param name="rootNode">Node considred as root properly a branch of the model.</param>
        /// <param name="path">Path to leaf from node.</param>
        /// <param name="interceptor">Instance of class for intercepting.</param>
        /// <returns>Returns true when interception is mounted on leaf.</returns>
        bool SetInterception<T>(Composite rootNode, string path, IInterceptor<T> interceptor);

        /// <summary>
        /// Traverse the model and returns a level and component name.
        /// </summary>
        /// <returns>Collection of tree strings holdin </returns>
        IEnumerable<ModelString> ToModelString();
    }
}
