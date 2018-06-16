using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockModel.Model {

    // Classic default composite node with childs.
    public class Composite : Component, IComposite {
        private Dictionary<string, IComponent> childs = null;
        private ComponentChangedEventHandler changedEventHandler;
        public Composite(string name)
            : base(name) {
            childs = new Dictionary<string, IComponent>();
            changedEventHandler = ChangedListener;
            _type = ComponentType.Compo;
        }

        public void Add(IComponent component) {
            // Mounts this composite listener, so all listeners on this 
            // component also gets an event, when the child changes.
            component.AddChangedListener(changedEventHandler);
            childs.Add(component.Name, component);
            Onchanged(this);
        }

        public void Remove(string name) {
            childs[name].RemoveChangedListener(changedEventHandler);
            childs.Remove(name);
            Onchanged(this);
        }

        public void ChangedListener(IComponent component) {
            Onchanged(this); // Overwrites the original event sender
        }

        public IEnumerator<IComponent> GetEnumerator() {
            return childs.Values.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public IComponent this[string name] {
            get { return Get(name); }
            set { Set(value); }
        }

        public IComponent Get(string name) {
            IComponent result = null;
            if (childs.ContainsKey(name)) {
                result = childs[name];
            }
            return result;
        }

        public void Set(IComponent c) {
            if (childs.ContainsKey(c.Name)) {
                childs[c.Name] = c;
                Onchanged(this);
            }
        }
        public int Count { get { return childs.Count; } }

    }
}
