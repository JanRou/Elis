using System.Collections;
using System.Collections.Generic;

namespace CompositeModel {

    public class Composite : Component, IEnumerable<Component> {

        private Dictionary<string, Component> childs = null;
        private ComponentChangedEventHandler changedEventHandler;

        public Composite(string name)
            : base(name) {
            childs = new Dictionary<string, Component>();
            changedEventHandler = new ComponentChangedEventHandler(ChangedListener);
            _type = ComponentBaseType.Compo;
        }

        public void Add(Component c) {
            // Mounts this composite listener, so all listeners on this 
            // component also gets an event, when the child changes.
            c.AddChangedListener(changedEventHandler);
            childs.Add(c.Name, c);
            Onchanged(this);
        }

        public void Remove(string name) {
            childs[name].RemoveChangedListener(changedEventHandler);
            childs.Remove(name);
            Onchanged(this);
        }

        public IEnumerator<Component> GetEnumerator() {
            return childs.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public Component this[string name] {
            get { return Get(name); }
            set { Set(value); }
        }

        public Component Get(string name) {
            Component ret = null;
            if (childs.ContainsKey(name)) {
                ret = childs[name];

            }
            return ret;
        }

        public void Set(Component c) {
            if (childs.ContainsKey(c.Name)) {
                childs[c.Name] = c;
                Onchanged(this);
            }
        }

        public void ChangedListener(Component component) {
            Onchanged(this); // Overwrites the original event sender
        }

        /// <summary>
        /// Returns the number of childs
        /// </summary>
        public int Count { get { return childs.Count; } }
    }
}
