using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ED.Wp3.Server.BE.PrognosisMetadata.Model
{
    public class Composite : Component, IEnumerable<Component>
    {
        private Dictionary<string, Component> childs = null;
        private ComponentChangedEventHandler changedEventHandler;
        public Composite(string name)
            : base(name)
        {
            childs = new Dictionary<string, Component>();
            changedEventHandler = ChangedListener;
            _type = ComponentType.Compo;
        }

        public void Add(Component c)
        {
            // Mounts this composite listener, so all listeners on this 
            // component also gets an event, when the child changes.
            c.AddChangedListener(changedEventHandler);
            childs.Add(c.Name, c);
            Onchanged(this);
        }

        public void Remove(string name)
        {
            childs[name].RemoveChangedListener(changedEventHandler);
            childs.Remove(name);
            Onchanged(this);
        }

        public void ChangedListener(Component component)
        {
            Onchanged(this); // Overwrites the original event sender
        }

        public IEnumerator<Component> GetEnumerator()
        {
            return childs.Values.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Component this[string name]
        {
            get { return Get(name); }
            set { Set(value); }
        }

        public Component Get(string name)
        {
            Component ret = null;
            if ( childs.ContainsKey(name) )
            {
                ret = childs[name];
            }
            return ret;
        }

        public void Set(Component c)
        {
            if ( childs.ContainsKey(c.Name) )
            {
                childs[c.Name] = c;
                Onchanged(this);
            }
        }
    }
}
