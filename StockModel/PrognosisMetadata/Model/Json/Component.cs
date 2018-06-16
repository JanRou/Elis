using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.Responses;
using Newtonsoft.Json.Converters;

namespace ED.Wp3.Server.BE.PrognosisMetadata.Model.Json
{
    public abstract class Component
    {
        public string Name { get; set; }
        public ComponentType Type { get; set; }

        public void Deserialize(){}
    }
    public class Composite : Component
    {
        public Composite()
        {
            Children = new Dictionary<string, Component>();
        }
        public Dictionary<string, Component> Children { get; set; }
    }
    public class Leaf : Component
    {
        public object Item { get; set; }
    }
}
