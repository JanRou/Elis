using System;
using System.Collections.Generic;
using System.Linq;
using CompositeModel.Xml;

namespace CompositeModel {

    public class Model : IModel {

        private Composite root = null;

        public Model() { }

        public Model(Composite composite) {
            root = composite;
        }

        /// <summary>
        /// Gets the node pointed out by path.
        /// </summary>
        /// <param name="path">Path to node</param>
        /// <returns>Node</returns>
        public Component Get(string path) {
            string[] nodeNames = getNodeNames(path);
            return get(root, nodeNames, 0);
        }

        /// <summary>
        /// Gets the node relatively
        /// </summary>
        /// <param name="node">Node to lookup from.</param>
        /// <param name="path">Relative path to node.</param>
        /// <returns></returns>
        public Component Get(Composite node, string path) {
            string[] nodeNames = getNodeNames(path);
            return get(node, nodeNames, 0);
        }

        /// <summary>
        /// Sets a node or leaf. Either you replace the node or leaf, or you insert a new node. 
        /// The path points out the parent node.
        /// </summary>
        /// <param name="path">Path to node</param>
        /// <param name="component">New node replacing exisitng or new</param>
        public void Set(string path, Component component) {
            string[] nodeNames = getNodeNames(path);
            if (root == null) {
                if ((nodeNames.Length == 0) &&  (typeof(Composite)==component.GetType()) ) {
                    root = (Composite) component;
                }
                else {
                    throw new ArgumentException("No root in model. Create root first and call with Composite.");
                }
            }
            else {
                Set( root, path, component);
            }
        }

        /// <summary>
        /// Sets a node relative from lookup node.
        /// </summary>
        /// <param name="node">Look up from node.</param>
        /// <param name="path">Relative path from lookup node.</param>
        /// <param name="component">New or replacing node.</param>
        public void Set(Composite node, string path, Component component) {
            string[] nodeNames = getNodeNames(path);
            Component parentNode = get(node, nodeNames, 0);
            if ((parentNode != null) && (parentNode.GetType() == typeof (Composite))) {
                // We found it and it is a composite
                if (null != ((Composite) parentNode)[component.Name]) {
                    // A node exists, replaces it
                    ((Composite) parentNode)[component.Name] = component;
                }
                else {
                    // It's a new node
                    ((Composite) parentNode).Add(component);
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
            Component node = Get(path);
            bool ret = node != null;
            if (ret) {
                node.AddChangedListener(eventHandler);
            }
            else {
                throw new ArgumentException("No node found in model for path: " + path);
            }
            return ret;
        }

        /// <summary>
        /// Unmounts change listener to node in path.
        /// </summary>
        /// <param name="path">Path to node.</param>
        /// <param name="eventHandler">Listener to unmount.</param>
        /// <returns></returns>
        public bool Unsubscribe(string path, ComponentChangedEventHandler eventHandler) {
            Component node = Get(path);
            bool ret = node != null;
            if(ret) {
                node.RemoveChangedListener(eventHandler);
            }
            else {
                throw new ArgumentException("No node found in model for path: " + path);
            }
            return ret;
        }

        /// <summary>
        /// Sets interceptor on leaf node.
        /// </summary>
        /// <typeparam name="T">Base type of interceptor.</typeparam>
        /// <param name="path">Path to leaf node from root.</param>
        /// <param name="interceptor">The interceptor to set.</param>
        /// <returns></returns>
        public bool SetInterception<T>(string path, IInterceptor<T> interceptor) {
            return SetInterception(root, path, interceptor);
        }

        /// <summary>
        /// Sets interceptor on leaf node pointed out by relative path from a lookup node.
        /// </summary>
        /// <typeparam name="T">Base type of interceptor.</typeparam>
        /// <param name="rootNode">Look up node</param>
        /// <param name="path">Relative path from lookup node.</param>
        /// <param name="interceptor">Interceptor to set.</param>
        /// <returns></returns>
        public bool SetInterception<T>(Composite rootNode, string path, IInterceptor<T> interceptor) {
            Component node = Get(rootNode, path);
            bool ret = (node != null) && ( (typeof(Leaf<T>) == node.GetType()) ); // Contract coding please!
            if(ret) {
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
            return ret;            
            
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
        private Component get(Component node, string[] nodeNames, int ix) {
            Component ret = null;
            if ((node != null) && (node.Name == nodeNames[ix])) {
                if(ix == (nodeNames.Length-1)) {
                    // Ok last node name found
                    ret = node;
                }
                else {
                    ++ix; // Next name in path
                    ret = get(((Composite)node)[nodeNames[ix]], nodeNames, ix);
                }
            }
            return ret;
        }

        /// <summary>
        /// The model as a collection of ModelString.
        /// </summary>
        /// <returns>Enumerable of ModelString objects</returns>
        public IEnumerable<ModelString> ToModelString() {
            return innerToModelString(root, 0);
        }

        // Inner recursive ModelString parsing
        private IEnumerable<ModelString> innerToModelString( Component component, int level) {
            yield return new ModelString() {Level = level, Name = component.Name};
            if (component.Type == ComponentBaseType.Compo) {
                Composite composite = component as Composite;
                foreach (Component comp in composite) {
                    foreach (ModelString modelString in innerToModelString(comp, level + 1)) {
                        yield return modelString;
                    }
                }
            }
            else {
                // Leaf<T>
                yield return new ModelString() 
                                    {     Level = level + 1
                                        , Name = component.Name
                                        , Value = component.ToString()
                                    };
            }
        }

    }
}
