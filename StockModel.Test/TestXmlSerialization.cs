using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CompositeModel;
using CompositeModel.Xml;

namespace StockModel.Test {

    [TestClass]
    public class TestXmlSerialization {

        [TestMethod]
        public void TestLeaf() {
            // Arrange
            Leaf<string> leaf = new Leaf<string>("Name");
            leaf.Value = "Curt";
            // Act
            string xml = XmlSerialization.Serialize<ComponentType>(XmlSerialization.ToComponentType(leaf));
            // Assert
            Assert.IsTrue(xml.Length > 0);
            ComponentType component = XmlSerialization.Deserialize<ComponentType>(xml);
            Assert.AreEqual(ComponentTypeType.Leaf, component.Type);
            Leaf leafXCS = (Leaf)component;
            Assert.AreEqual(leaf.Value, leafXCS.Item);
        }

        [TestMethod]
        public void TestComposite() {
            // Arrange
            CompositeModel.Composite composite = new CompositeModel.Composite("Compo");
            Leaf<string> leafUser = new Leaf<string>("User");
            leafUser.Value = "Curt";
            composite.Add(leafUser);
            Leaf<int> leafAntal = new Leaf<int>("Number");
            leafAntal.Value = 3;
            composite.Add(leafAntal);
            // Act 1.
            string xml = XmlSerialization.Serialize<ComponentType>(XmlSerialization.ToComponentType(composite));
            // Assert 1.
            Assert.IsTrue(xml.Length > 0);
            // Act 2.
            ComponentType component = XmlSerialization.Deserialize<ComponentType>(xml);
            // Assert 2.
            Assert.AreEqual(ComponentTypeType.Compo, component.Type);
            CompositeModel.Xml.Composite compositeXCS = (CompositeModel.Xml.Composite)component;
            Assert.AreEqual(composite.Name, compositeXCS.Name);
            Assert.AreEqual(composite.Count, compositeXCS.Component.Length);
            int cnt = 0;
            foreach (var cmpnt in compositeXCS.Component) {
                if (cmpnt.Name == "User") {
                    cnt++;
                    Leaf<string> leaf = (Leaf<string>)composite.Get("User");
                    Assert.AreEqual(leaf.Value, ((Leaf)cmpnt).Item);
                }
                else if (cmpnt.Name == "Number") {
                    cnt++;
                    Leaf<int> leaf = (Leaf<int>)composite.Get("Number");
                    Assert.AreEqual(leaf.Value, ((Leaf)cmpnt).Item);
                }
                else {
                    Assert.IsTrue(false);
                }
            }
            Assert.AreEqual(2, cnt);
        }
    }
}
