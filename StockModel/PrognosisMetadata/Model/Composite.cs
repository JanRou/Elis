using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ED.Wp3.Server.BE.PrognosisMetadata.Model
{

    // Classic default composite node with childs.
    public class Composite : Component, IComposite {
        private Dictionary<string, IComponent> children = null;
        private ComponentChangedEventHandler changedEventHandler;
        public Composite(string name)
            : base(name) {
            children = new Dictionary<string, IComponent>();
            changedEventHandler = ChangedListener;
            _type = ComponentType.Compo;
        }

        public void Add(IComponent component) {
            // Mounts this composite listener, so all listeners on this 
            // component also gets an event, when the child changes.
            component.AddChangedListener(changedEventHandler);
            children.Add(component.Name, component);
            Onchanged(this);
        }

        public void Remove(string name) {
            children[name].RemoveChangedListener(changedEventHandler);
            children.Remove(name);
            Onchanged(this);
        }

        public void ChangedListener(IComponent component) {
            Onchanged(this); // Overwrites the original event sender
        }

        public IEnumerator<IComponent> GetEnumerator() {
            return children.Values.GetEnumerator();
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
            if (children.ContainsKey(name)) {
                result = children[name];
                if (result.Type == ComponentType.CompoInterceptor)
                {
                    // Create the composite on the fly
                    result.Create();
                }
            }
            return result;
        }

        public void Set(IComponent c) {
            if (children.ContainsKey(c.Name)) {
                children[c.Name] = c;
                Onchanged(this);
            }
        }
        public int Count { get { return children.Count; } }

    }
}
