namespace CompositeModel {

    public interface IComponent {

        /// <summary>
        /// Name of the component in the tree.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Add change listener.
        /// </summary>
        /// <param name="eventHandler">Listener receiving change events.</param>
        void AddChangedListener(ComponentChangedEventHandler eventHandler);

        /// <summary>
        /// Remove change listener.
        /// </summary>
        /// <param name="eventHandler">Listener to remove.</param>
        void RemoveChangedListener(ComponentChangedEventHandler eventHandler);

    }
}
