using System;
using System.Linq;
using ED.Atlas.Service.IC.BE.Messages;
using ED.Atlas.Service.IC.BE.ModelHandlers.CompositeModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Composite = ED.Atlas.Service.IC.BE.ModelHandlers.CompositeModel.Composite;

namespace ED.Atlas.Service.IC.BE.Test {
    [TestClass]
    public class UnitTestComposite {
        [TestMethod]
        public void TestConstructCompositeandAdd() {
            ModelHandlers.CompositeModel.Composite root = new ModelHandlers.CompositeModel.Composite("Intraday");
            root.Add(new ModelHandlers.CompositeModel.Composite("EPEXSPOT"));
            root.Add(new ModelHandlers.CompositeModel.Composite("APX"));
            Assert.AreEqual(2, root.Count(), "Count er forkert");
            Assert.AreEqual("EPEXSPOT", root["EPEXSPOT"].Name, "Epexspot er forkert");
            Assert.AreEqual("APX", root["APX"].Name, "Apx er forkert");            
        }
        [TestMethod]
        public void TestCompositeRemove() {
            ModelHandlers.CompositeModel.Composite root = new ModelHandlers.CompositeModel.Composite("Intraday");
            root.Add(new ModelHandlers.CompositeModel.Composite("EPEXSPOT"));
            root.Add(new ModelHandlers.CompositeModel.Composite("APX"));
            Assert.AreEqual("APX", root["APX"].Name, "Apx er forkert");
            root.Remove("APX");
            Assert.IsTrue(root["APX"]==null, "Composite returnerer ikke null for fjernet APX");
        }
        [TestMethod]
        public void TestConstructCompositeEnumerator() {
            ModelHandlers.CompositeModel.Composite root = new ModelHandlers.CompositeModel.Composite("Intraday");
            root.Add(new ModelHandlers.CompositeModel.Composite("EPEXSPOT"));
            root.Add(new ModelHandlers.CompositeModel.Composite("APX"));
            int cnt = 0;
            foreach (Component component in root) {
                cnt++;
            }
            Assert.AreEqual(2, cnt, "Foreach tæller ikke til 2");
        }
        [TestMethod]
        public void TestConstructLeaf() {
            Leaf<string> blad1 = new Leaf<string>("Blad1");
            blad1.Value = "Værdi1";
            Assert.AreEqual("Blad1", blad1.Name, "Blad1 har forkert navn");
            Assert.AreEqual("Værdi1", blad1.Value, "Blad1 har forkert værdi");
            Leaf<string> blad2 = new Leaf<string>("Blad2") {Value="Værdi2"};
            Assert.AreEqual("Blad2", blad2.Name, "Blad2 har forkert navn");
            Assert.AreEqual("Værdi2", blad2.Value, "Blad2 har forkert værdi");
            var blad3 = new Leaf<string>("Blad3") { Value = "Værdi3" };
            Assert.AreEqual("Blad3", blad3.Name, "Blad3 har forkert navn");
            Assert.AreEqual("Værdi3", blad3.Value, "Blad3 har forkert værdi");
        }
        [TestMethod]
        public void TestCompositeEventHandler() {
            ModelHandlers.CompositeModel.Composite root = new ModelHandlers.CompositeModel.Composite("Intraday");
            ComponentChangedEventHandler el = new ComponentChangedEventHandler(ChangedListener);
            root.AddChangedListener(el);
            Leaf<string> blad = new Leaf<string>("Blad") {Value = "Værdi"};
            root.Add(blad);
            Assert.IsTrue( bEventKaldt,"ChangedListener ikke kaldt");
            Assert.AreEqual(root.Name, componentModtaget.Name, "ChangeListener modtog forkert component");
            Assert.AreEqual("Værdi", ((Leaf<string>)root["Blad"]).Value, "Blad har forkert værdi");
            root.RemoveChangedListener(el);
            bEventKaldt = false;
            root.Remove("Blad");
            Assert.IsFalse(bEventKaldt, "ChangedListener er kaldt, men listener er fjernet");
            Assert.IsNull(((Leaf<string>)root["Blad"]), "root[Blad] returnerer ikke null"); ;
        }
        public bool bEventKaldt = false;
        public Component componentModtaget = null;
        public void ChangedListener(Component component) {
            bEventKaldt = true;
            componentModtaget = component;
        }
        [TestMethod]
        public void TestCompositeEventHandlerWhenChildChanged() {
            ModelHandlers.CompositeModel.Composite root = new ModelHandlers.CompositeModel.Composite("Intraday");
            Leaf<string> blad = new Leaf<string>("Blad") { Value="Værdi" };
            root.Add(blad);
            Assert.AreEqual("Værdi", ((Leaf<string>)root["Blad"]).Value, "Blad har forkert værdi");
            ComponentChangedEventHandler el = new ComponentChangedEventHandler(ChangedListener);
            root.AddChangedListener(el);
            // Sæt en værdi 
            blad.Value = "Anden værdi";
            Assert.IsTrue(bEventKaldt, "ChangedListener er ikke kaldt");
            Assert.AreEqual(root.Name, componentModtaget.Name, "Root har ikke sendt event");
            Assert.AreEqual("Anden værdi", ((Leaf<string>)root["Blad"]).Value, "Blad er ikke Anden værdi");
        }
        public class TestDoubleUpInterception : IInterceptor<int> {
            private int v = 2;
            public bool Kaldt { get; set; }
            public int Get() {
                Kaldt = true;
                return v * 2;
            }
            public void Set(int val) {
                Kaldt = true;
                v = val;
            }
        }
        [TestMethod]
        public void TestLeafInterception() {
            Leaf<int> leaf = new Leaf<int>("Test");
            TestDoubleUpInterception interception = new TestDoubleUpInterception() { Kaldt = false };
            leaf.Interceptor = interception;
            leaf.Value = 4;
            Assert.IsTrue(interception.Kaldt, "TestDoubleUpInterception er ikke kaldt");
            Assert.AreEqual(8, leaf.Value, "Forkert returværdi fra leaf");
        }
        [TestMethod]
        public void TestLeafToComponentType() {
            Leaf<int> leaf = new Leaf<int>("Heltal");
            leaf.Value = 2;
            ComponentType componentType = leaf.ToComponentType();
            Assert.AreEqual( leaf.Name, componentType.Name, "Navn forkert");
            Assert.AreEqual( leaf.Value, ((Leaf)componentType).Item, "Item er forkert");
            Assert.AreEqual( ComponentTypeType.Leaf, componentType.Type, "Forkert type");
            Assert.AreEqual( leaf.GetItemType(), ((Leaf)componentType).ItemElementName
                , "ItemElementName er forkert");
        }
        [TestMethod]
        public void TestNewMapEntryForAllTypes() {
            Map map = new Map();
            foreach (ItemChoiceType itemChoiceType in Enum.GetValues(typeof(ItemChoiceType))) {
                IMapEntry mapEntry = map.NewMapEntry(itemChoiceType);
                Assert.IsNotNull(mapEntry);
            }
        }
        [TestMethod]
        public void TestLeafMapToComponentType() {
            LeafMap leaf = new LeafMap("Map");
            leaf.Value.Add("key", "value", ItemChoiceType.@string); // ! element i map
            ComponentType componentType = leaf.ToComponentType();  // LeafMap bliver til en compo
            Assert.AreEqual(leaf.Name, componentType.Name, "Navn forkert");
            Assert.AreEqual(ComponentTypeType.Compo, componentType.Type, "Forkert type");
            Messages.Composite composite = (Messages.Composite) componentType;
            Assert.IsTrue(composite.Component.Length==1, "Der er ikke 1 element i Component[]");
            string val = leaf.Value.Get<string>("key");
            Leaf mapEntryLeaf = (Leaf)composite.Component[0];
            Assert.AreEqual( "key", mapEntryLeaf.Name, "Entry i leafMap har forkert navn");
            Assert.AreEqual( leaf.Value.Get<string>("key"), mapEntryLeaf.Item, "Entry i leafMap har forkert værdi");
        }
        [TestMethod]
        public void TestEmptyLeafMapToComponentType() {
            LeafMap leaf = new LeafMap("Map"); // Tom leafMap
            ComponentType componentType = leaf.ToComponentType();  // LeafMap bliver til en compo
            Assert.AreEqual(leaf.Name, componentType.Name, "Navn forkert");
            Assert.AreEqual(ComponentTypeType.Compo, componentType.Type, "Forkert type");
            Messages.Composite composite = (Messages.Composite)componentType;
            Assert.IsTrue(composite.Component.Length == 0, "Der er ikke 0 element i Component[]");
            Map mapFromEmptyLeaf= new Map();
            mapFromEmptyLeaf.Create(composite);
            Assert.AreEqual( 0, mapFromEmptyLeaf.Count(), "Count er ikke 0");
        }
        [TestMethod]
        public void TestCompositeToComponentType() {
            ModelHandlers.CompositeModel.Composite composite = new ModelHandlers.CompositeModel.Composite("Compo");
            composite.Add(new ModelHandlers.CompositeModel.Composite("EPEXSPOT"));
            Leaf<int> leaf = new Leaf<int>("Heltal");
            leaf.Value = 2;
            composite.Add(leaf);
            ComponentType component = composite.ToComponentType();
            Assert.AreEqual(composite.Name, component.Name, "Navn forkert");
            Assert.AreEqual(ComponentTypeType.Compo, component.Type, "Forkert type");
            Assert.IsTrue( ((Messages.Composite)component).Component.Length==2
                , "Forkert antal i Component");
            Assert.AreEqual("EPEXSPOT", ((Messages.Composite)component).Component[0].Name
                , "Component[0].Name forkert");
            Assert.AreEqual("Heltal"
                , ((Messages.Composite)component).Component[1].Name
                , "Component[1].Name forkert");
            Assert.AreEqual( ComponentTypeType.Leaf
                , (((Messages.Composite)component).Component[1]).Type
                , "Component[1].Type er forkert");
            Leaf xmlLeaf = (Leaf)((Messages.Composite)component).Component[1];
            Assert.AreEqual(
                    ItemChoiceType.@int
                ,   xmlLeaf.ItemElementName
                , "xmlLeaf type er forkert");
            Assert.AreEqual(leaf.Value, xmlLeaf.Item, "Item er forkert");
        }
   
        //[TestMethod]
        //public void Test() {
        //}   
    }

}
