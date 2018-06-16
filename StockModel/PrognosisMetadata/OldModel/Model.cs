using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ED.Wp3.Server.BE.PrognosisMetadata.Model
{
    // TODO THhis is nearly exact copy from Atlas. Isolate the functionality and publish it as a nuget package.
    public class TreeString
    {
        public TreeString()
        {
            Value = "";
        }
        public int Level { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public interface IModel
    {
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
        bool SetInterception<T>(Composite rootNode, string path, ILeafInterceptor<T> leafInterceptor);
        /// <summary>
        /// Traverse the model and returns a level and component name.
        /// </summary>
        /// <returns>Collection of tree strings holdin </returns>
        IEnumerable<TreeString> ToTreeString();
    }
    public class Model : IModel
    {
        private Composite root = null;

        public Model() { }
        public Model(Composite composite)
        {
            root = composite;
        }
        /// <summary>
        /// Gets the node pointed out by path.
        /// </summary>
        /// <param name="path">Path to node</param>
        /// <returns>Node</returns>
        public Component Get(string path)
        {
            string[] nodeNames = getNodeNames(path);
            return get(root, nodeNames, 0);
        }
        /// <summary>
        /// Gets the node relatively
        /// </summary>
        /// <param name="node">Node to lookup from.</param>
        /// <param name="path">Relative path to node.</param>
        /// <returns></returns>
        public Component Get(Composite node, string path)
        {
            string[] nodeNames = getNodeNames(path);
            return get(node, nodeNames, 0);
        }
        /// <summary>
        /// Sets a node or leaf. Either you replace the node or leaf, or you insert a new node. 
        /// The path points out the parent node.
        /// </summary>
        /// <param name="path">Path to node</param>
        /// <param name="component">New node replacing exisitng or new</param>
        public void Set(string path, Component component)
        {
            string[] nodeNames = getNodeNames(path);
            if ( root == null )
            {
                if ( ( nodeNames.Length == 0 ) && ( typeof(Composite) == component.GetType() ) )
                {
                    root = (Composite) component;
                }
                else
                {
                    throw new ArgumentException("No root in model. Create root first and call with Composite.");
                }
            }
            else
            {
                Set(root, path, component);
            }
        }
        /// <summary>
        /// Sets a node relative from lookup node.
        /// </summary>
        /// <param name="node">Look up from node.</param>
        /// <param name="path">Relative path from lookup node.</param>
        /// <param name="component">New or replacing node.</param>
        public void Set(Composite node, string path, Component component)
        {
            string[] nodeNames = getNodeNames(path);
            Component parentNode = get(node, nodeNames, 0);
            if ( ( parentNode != null ) && ( parentNode.GetType() == typeof(Composite) ) )
            {
                // We found it and it is a composite
                if ( null != ( (Composite) parentNode )[component.Name] )
                {
                    // A node exists, replaces it
                    ( (Composite) parentNode )[component.Name] = component;
                }
                else
                {
                    // It's a new node
                    ( (Composite) parentNode ).Add(component);
                }
            }
        }
        /// <summary>
        /// Mount change listener to node in path.
        /// </summary>
        /// <param name="path">path to node.</param>
        /// <param name="eventHandler">Event listener</param>
        /// <returns></returns>
        public bool Subscribe(string path, ComponentChangedEventHandler eventHandler)
        {
            Component node = Get(path);
            bool result = node != null;
            if ( result )
            {
                node.AddChangedListener(eventHandler);
            }
            else
            {
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
        public bool Unsubscribe(string path, ComponentChangedEventHandler eventHandler)
        {
            Component node = Get(path);
            bool result = node != null;
            if ( result )
            {
                node.RemoveChangedListener(eventHandler);
            }
            else
            {
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
        public bool SetInterception<T>(string path, ILeafInterceptor<T> leafInterceptor)
        {
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
        public bool SetInterception<T>(Composite rootNode, string path, ILeafInterceptor<T> leafInterceptor)
        {
            Component node = Get(rootNode, path);
            bool result = ( node != null )
                && ( typeof(Leaf<T>) == node.GetType() ); // Contract coding please!
            if ( result )
            {
                ( (Leaf<T>) node ).LeafInterceptor = leafInterceptor;
            }
            else
            {
                if ( node == null )
                {
                    throw new ArgumentException("No node found in model for path: " + path);
                }
                else
                {
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
        private string[] getNodeNames(string path)
        {
            int pathLength = ( path.Last() == '/' ) ? ( path.Length - 1 ) : path.Length; // strip of trailing slash'/'
            string[] nodeNames = new string[0];
            if ( pathLength > 0 )
            {
                int byPassSlash = path[0] == '/' ? 1 : 0; // set to 1 in order to remove starting slash '/' if it's there
                pathLength -= byPassSlash;                // decrease path length with 1 accordingly
                nodeNames = path.Substring(byPassSlash, pathLength).Split('/');
            }
            return nodeNames;
        }
        // Get the node pointed out by the path in array nodeNames
        private Component get(Component node, string[] nodeNames, int ix)
        {
            Component ret = null;
            if ( ( node != null ) && ( node.Name == nodeNames[ix] ) )
            {
                if ( ix == ( nodeNames.Length - 1 ) )
                {
                    // Ok last node name found
                    ret = node;
                }
                else
                {
                    ++ix; // Next name in path
                    ret = get(( (Composite) node )[nodeNames[ix]], nodeNames, ix);
                }
            }
            return ret;
        }
        private IEnumerable<TreeString> innerToTreeString(Component component, int level)
        {
            yield return new TreeString() { Level = level, Name = component.Name };
            if (component.Type == ComponentType.Compo)
            {
                Composite composite = component as Composite;
                foreach (Component comp in composite)
                {
                    foreach (TreeString treeString in innerToTreeString(comp, level + 1))
                    {
                        yield return treeString;
                    }
                }
            }
            else
            {
                yield return
                    new TreeString()
                    {
                        Level = level + 1,
                        Name = component.Name,
                        Value = ((ILeaf) component).Item.ToString()
                    };
            }
        }
        public IEnumerable<TreeString> ToTreeString()
        {
            return innerToTreeString(root, 0);
        }
    }

}
