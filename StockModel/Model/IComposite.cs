using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockModel.Model {
    // Implement this interface in case you want to implement a composite
    // that gets it child nodes and leafs from the database. 
    public interface IComposite : IComponent, IEnumerable<IComponent> {
        /// <summary>
        /// Adds a component to the Composite
        /// </summary>
        /// <param name="component">Compoenent</param>
        void Add(IComponent componet);
        /// <summary>
        /// Removes a component from the Composite identified by name
        /// </summary>
        /// <param name="name">Name of componetn</param>
        void Remove(string name);
        /// <summary>
        /// The changed listener prototype
        /// </summary>
        /// <param name="component">Component changed (?)</param>
        void ChangedListener(IComponent component);
        /// <summary>
        /// Get Component in composite
        /// </summary>
        /// <param name="name">Name of component</param>
        /// <returns></returns>
        IComponent Get(string name);
        /// <summary>
        /// Set component in composite
        /// </summary>
        /// <param name="component">Component</param>
        void Set(IComponent component);
        /// <summary>
        /// Returns the number of childs (components)
        /// </summary>
        int Count { get; }
    }
}
