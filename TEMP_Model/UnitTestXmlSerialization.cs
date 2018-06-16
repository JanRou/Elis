using System.Linq;
using ED.Atlas.Service.IC.BE.Messages;
using ED.Atlas.Service.IC.BE.ModelHandlers.CompositeModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Composite = ED.Atlas.Service.IC.BE.ModelHandlers.CompositeModel.Composite;

namespace ED.Atlas.Service.IC.BE.Test {
    [TestClass]
    public class UnitTestXmlSerialization {
        [TestMethod]
        public void TestLeaf() {
            Leaf<string> leaf = new Leaf<string>("Navn");
            leaf.Value = "Kurt";
            string xml = XmlSerialization.Serialize<ComponentType>(leaf.ToComponentType());
            Assert.IsTrue(xml.Length>0, "xml er tom.");
            ComponentType component = XmlSerialization.Deserialize<ComponentType>(xml);
            Assert.AreEqual( ComponentTypeType.Leaf ,component.Type, "component.Type er ikke leaf.");
            Leaf leafXCS = (Leaf) component;
            Assert.AreEqual(leaf.Value, leafXCS.Item, "leafXCS.Item er forkert");
        }
        [TestMethod]
        public void TestComposite() {
            ModelHandlers.CompositeModel.Composite composite = new ModelHandlers.CompositeModel.Composite("Compo");
            Leaf<string> leafUser = new Leaf<string>("User");
            leafUser.Value = "Kurt";
            composite.Add(leafUser);
            Leaf<int> leafAntal = new Leaf<int>("Antal");
            leafAntal.Value = 3;
            composite.Add(leafAntal);
            string xml = XmlSerialization.Serialize<ComponentType>(composite.ToComponentType());
            Assert.IsTrue(xml.Length > 0, "xml er tom.");
            ComponentType component = XmlSerialization.Deserialize<ComponentType>(xml);
            Assert.AreEqual(ComponentTypeType.Compo, component.Type, "component.Type er ikke Compo.");
            Messages.Composite compositeXCS = (Messages.Composite) component;
            Assert.AreEqual(composite.Name, compositeXCS.Name, "compositeXCS.Name er forkert");
            Assert.AreEqual(composite.Count(), compositeXCS.Component.Count(), "Antal blade i compositeXCS er forkert");
            int cnt = 0;
            foreach (var cmpnt in compositeXCS.Component) {
                if (cmpnt.Name == "User") {
                    cnt++;
                    Leaf<string> leaf = (Leaf<string>)composite.Get("User");
                    Assert.AreEqual( leaf.Value, ((Leaf)cmpnt).Item, "Item er forkert for User");
                }
                else if (cmpnt.Name == "Antal") {
                    cnt++;
                    Leaf<int> leaf = (Leaf<int>) composite.Get("Antal");
                    Assert.AreEqual(leaf.Value, ((Leaf) cmpnt).Item, "Item er forkert for Antal");
                }
                else {
                    Assert.IsTrue(false, "cmpnt.Name er ikke fundet");
                }
            }
            Assert.AreEqual( 2, cnt, "cnt er forkert");
        }
    }
}
