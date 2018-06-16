using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockModel.Model {
    public interface IModel {
        /// <summary>
        /// Gets a component with the model root as root in the tree.
        /// </summary>
        /// <param name="path">Reference to the component in the three.</param>
        /// <returns></returns>
        IComponent Get(string path);
        /// <summary>
        /// Get the component in the tree with root node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        IComponent Get(IComposite node, string path);
        /// <summary>
        /// Adds a component to the tree.
        /// </summary>
        /// <param name="path">Path in the tree to the composite node.</param>
        /// <param name="component">Component to add.</param>
        void Set(string path, IComponent component);
        /// <summary>
        /// Adds a component to the tree which root is node. This is a relatively Set.
        /// </summary>
        /// <param name="node">Root of the tree that a component is added.</param>
        /// <param name="path">Path in tree with the node as root.</param>
        /// <param name="component">Compnent to add.</param>
        void Set(IComposite node, string path, IComponent component);
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
        /// Set the inteceptor class for the leaf specified by path.
        /// </summary>
        /// <typeparam name="T">Class implementing the interceptor interface.</typeparam>
        /// <param name="path">Path to leaf.</param>
        /// <param name="leafInterceptor">instance of class for intercepting.</param>
        /// <returns>Returns true when interception is mounted on leaf.</returns>
        bool SetInterception<T>(string path, ILeafInterceptor<T> leafInterceptor);
        /// <summary>
        /// Set the intecep class for the leaf specified by path in the tree with root node.
        /// </summary>
        /// <typeparam name="T">Class implementing the interceptor interface.</typeparam>
        /// <param name="rootNode">Node which is the root of the tree.</param>
        /// <param name="path">Path to leaf.</param>
        /// <param name="leafInterceptor">instance of class for intercepting.</param>
        /// <returns>Returns true when interception is mounted on leaf.</returns>
        bool SetInterception<T>(IComposite rootNode, string path, ILeafInterceptor<T> interceptor);
        /// <summary>
        /// The model as a collection of ModelString.
        /// </summary>
        /// <returns>Enumerable of ModelString objects</returns>
        IEnumerable<ModelString> ToModelString();
    }

    public class Model : IModel {
        private IComposite root = null;

        public Model() { }
        public Model(IComposite composite) {
            root = composite;
        }
        /// <summary>
        /// Gets the node pointed out by path.
        /// </summary>
        /// <param name="path">Path to node</param>
        /// <returns>Node</returns>
        public IComponent Get(string path) {
            string[] nodeNames = getNodeNames(path);
            return get(root, nodeNames, 0);
        }
        /// <summary>
        /// Gets the node relatively
        /// </summary>
        /// <param name="node">Node to lookup from.</param>
        /// <param name="path">Relative path to node.</param>
        /// <returns></returns>
        public IComponent Get(IComposite node, string path) {
            string[] nodeNames = getNodeNames(path);
            return get(node, nodeNames, 0);
        }
        /// <summary>
        /// Sets a node or leaf. Either you replace the node or leaf, or you insert a new node. 
        /// The path points out the parent node.
        /// </summary>
        /// <param name="path">Path to node</param>
        /// <param name="component">New node replacing exisitng or new</param>
        public void Set(string path, IComponent component) {
            string[] nodeNames = getNodeNames(path);
            if (root == null) {
                if ((nodeNames.Length == 0) && (typeof(Composite) == component.GetType())) {
                    root = (Composite)component;
                }
                else {
                    throw new ArgumentException("No root in model. Create root first and call with Composite.");
                }
            }
            else {
                Set(root, path, component);
            }
        }
        /// <summary>
        /// Sets a node relative from lookup node.
        /// </summary>
        /// <param name="node">Look up from node.</param>
        /// <param name="path">Relative path from lookup node.</param>
        /// <param name="component">New or replacing node.</param>
        public void Set(IComposite node, string path, IComponent component) {
            string[] nodeNames = getNodeNames(path);
            IComponent parentNode = get(node, nodeNames, 0);
            if ((parentNode != null) && (parentNode.GetType() == typeof(Composite))) {
                // We found it and it is a composite
                if (null != ((Composite)parentNode)[component.Name]) {
                    // A node exists, replaces it
                    ((Composite)parentNode)[component.Name] = component;
                }
                else {
                    // It's a new node
                    ((Composite)parentNode).Add(component);
                }
            }
        }
        /// <summary>
        /// Mount change listener to node in path.
        /// </summary>
        /// <param name="path">path to node.</param>
        /// <param name="eventHandler">Event listener</param>
        /// <returns></returns>
        public bool Subscribe(string path, ComponentChangedEventHandler eventHandler) {
            IComponent node = Get(path);
            bool result = node != null;
            if (result) {
                node.AddChangedListener(eventHandler);
            }
            else {
                throw new ArgumentException("No node found in model for path: " + path);
            }
            return result;
        }
        /// <summary>
        /// Unmounts change listener to node in path.
        /// </summary>
        /// <param name="path">Path to node.</param>
        /// <param name="eventHandler">Listener to unmount.</param>
        /// <returns></returns>
        public bool Unsubscribe(string path, ComponentChangedEventHandler eventHandler) {
            IComponent node = Get(path);
            bool result = node != null;
            if (result) {
                node.RemoveChangedListener(eventHandler);
            }
            else {
                throw new ArgumentException("No node found in model for path: " + path);
            }
            return result;
        }
        /// <summary>
        /// Sets interceptor on leaf node.
        /// </summary>
        /// <typeparam name="T">Base type of interceptor.</typeparam>
        /// <param name="path">Path to leaf node from root.</param>
        /// <param name="leafInterceptor">The interceptor to set.</param>
        /// <returns></returns>
        public bool SetInterception<T>(string path, ILeafInterceptor<T> leafInterceptor) {
            return SetInterception(root, path, leafInterceptor);
        }
        /// <summary>
        /// Sets interceptor on leaf node pointed out by relative path from a lookup node.
        /// </summary>
        /// <typeparam name="T">Base type of interceptor.</typeparam>
        /// <param name="rootNode">Look up node</param>
        /// <param name="path">Relative path from lookup node.</param>
        /// <param name="leafInterceptor">Interceptor to set.</param>
        /// <returns>True when succeeded otherwise false.</returns>
        public bool SetInterception<T>(IComposite rootNode, string path, ILeafInterceptor<T> interceptor) {
            IComponent node = Get(rootNode, path);
            bool result = (node != null)
                && (typeof(Leaf<T>) == node.GetType()); // Contract coding please!
            if (result) {
                ((Leaf<T>)node).Interceptor = interceptor;
            }
            else {
                if (node == null) {
                    throw new ArgumentException("No node found in model for path: " + path);
                }
                else {
                    throw new ArgumentException("Expected leaf, but node found in model is composite for path: " + path);
                }
            }
            return result;
        }
        /// <summary>
        /// Convert path to an array of names. A root is an empty array.
        /// </summary>
        /// <param name="path">Path to node</param>
        /// <returns>An array of names or empty array for root</returns>
        private string[] getNodeNames(string path) {
            int pathLength = (path.Last() == '/') ? (path.Length - 1) : path.Length; // strip of trailing slash'/'
            string[] nodeNames = new string[0];
            if (pathLength > 0) {
                int byPassSlash = path[0] == '/' ? 1 : 0; // set to 1 in order to remove starting slash '/' if it's there
                pathLength -= byPassSlash;                // decrease path length with 1 accordingly
                nodeNames = path.Substring(byPassSlash, pathLength).Split('/');
            }
            return nodeNames;
        }
        // Get the node pointed out by the path in array nodeNames
        private IComponent get(IComponent node, string[] nodeNames, int ix) {
            IComponent result = null;
            if ((node != null) && (node.Name == nodeNames[ix])) {
                if (ix == (nodeNames.Length - 1)) {
                    // Ok last node name found
                    result = node;
                }
                else {
                    ++ix; // Next name in path
                    result = get(((IComposite)node).Get(nodeNames[ix]), nodeNames, ix);
                }
            }
            return result;
        }

        public IEnumerable<ModelString> ToModelString() {
            return innerToModelString(root, 0);
        }

        // Inner recursive ModelString parsing
        private IEnumerable<ModelString> innerToModelString(IComponent component, int level) {
            yield return new ModelString() { Level = level, Name = component.Name };
            if (component.Type == ComponentType.Compo) {
                Composite composite = component as Composite;
                foreach (Component comp in composite) {
                    foreach (ModelString modelString in innerToModelString(comp, level + 1)) {
                        yield return modelString;
                    }
                }
            }
            else {
                // Leaf<T>
                yield return new ModelString() {
                        Level = level + 1
                   ,    Name = component.Name
                   ,    Value = ((ILeaf)component).Item.ToString()
                };
            }
        }
    }

}
