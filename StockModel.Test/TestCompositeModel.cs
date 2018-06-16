using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockModel.Model;

namespace StockModel.Test {
    [TestClass]
    public class TestCompositeModel {

        [TestMethod]
        public void TestConstructCompositeAdd() {
            IComposite root = new Composite("StockModel");
            root.Add(new Composite("NasDaqOMX"));
            root.Add(new Composite("DB"));
            Assert.AreEqual(2, root.Count);
            Assert.AreEqual("NasDaqOMX", root.Get("NasDaqOMX").Name);
            Assert.AreEqual("DB", root.Get("DB").Name);
        }

        [TestMethod]
        public void TestConstructLeaf() {
            ILeaf leaf1 = new Leaf<string>("Leaf1");
            leaf1.Item = "Value1";
            Assert.AreEqual("Leaf1", leaf1.Name);
            Assert.AreEqual("Value1", leaf1.Item);
            ILeaf leaf2 = new Leaf<string>("Leaf2") { Value = "Value2" };
            Assert.AreEqual("Leaf2", leaf2.Name);
            Assert.AreEqual("Value2", leaf2.Item);
            var leaf3 = new Leaf<string>("Leaf3") { Value = "Value3" };
            Assert.AreEqual("Leaf3", leaf3.Name);
            Assert.AreEqual("Value3", leaf3.Value);
        }

        [TestMethod]
        public void TestCompositeRemove() {
            IComposite root = new Composite("StockModel");
            root.Add(new Composite("NasDaqOMX"));
            root.Add(new Composite("DB"));
            Assert.AreEqual("DB", root.Get("DB").Name);
            root.Remove("DB");
            Assert.IsTrue(root.Get("DB")==null);
        }

        [TestMethod]
        public void TestCompositeEnumerator() {
            IComposite root = new Composite("StockModel");
            root.Add(new Composite("NasDaqOMX"));
            root.Add(new Composite("DB"));
            int cnt = 0;
            foreach (Component component in root) {
                cnt++;
            }
            Assert.AreEqual(2, cnt);
        }

        [TestMethod]
        public void TestCompositeEventHandler() {
            IComposite root = new Composite("StockModel");
            ComponentChangedEventHandler el = new ComponentChangedEventHandler(ChangedListener);
            root.AddChangedListener(el);
            ILeaf leaf = new Leaf<string>("Leaf") {Value = "Value"};
            root.Add(leaf);
            Assert.IsTrue( bEventCalled);
            Assert.AreEqual(root.Name, componentReceived.Name);
            Assert.AreEqual("Value", ((Leaf<string>)root.Get("Leaf")).Value);
            root.RemoveChangedListener(el);
            bEventCalled = false;
            root.Remove("Leaf");
            Assert.IsFalse(bEventCalled);
            Assert.IsNull(((Leaf<string>)root.Get("Leaf"))); ;
        }

        public bool bEventCalled = false;
        public IComponent componentReceived = null;

        public void ChangedListener(IComponent component) {
            bEventCalled = true;
            componentReceived = component;
        }

        [TestMethod]
        public void TestCompositeEventHandlerWhenSubnodeChanged() {
            Composite root = new Composite("StockModel");
            Leaf<string> leaf = new Leaf<string>("Leaf") { Value="Value" };
            root.Add(leaf);
            Assert.AreEqual("Value", ((Leaf<string>)root.Get("Leaf")).Value);
            ComponentChangedEventHandler el = new ComponentChangedEventHandler(ChangedListener);
            root.AddChangedListener(el);
            // Set a value 
            leaf.Value = "Other value";
            Assert.IsTrue(bEventCalled);
            Assert.AreEqual(root.Name, componentReceived.Name);
            Assert.AreEqual("Other value", ((Leaf<string>)root.Get("Leaf")).Value);
        }

        public class TestDoubleUpInterception : ILeafInterceptor<int> {
            private int v = 2;
            public bool bCalled { get; set; }
            public int Get() {
                bCalled = true;
                return v * 2;
            }
            public void Set(int val) {
                bCalled = true;
                v = val;
            }
        }

        [TestMethod]
        public void TestLeafInterception() {
            Leaf<int> leaf = new Leaf<int>("Test");
            TestDoubleUpInterception interceptor = new TestDoubleUpInterception() { bCalled = false };
            leaf.Interceptor = interceptor;
            leaf.Value = 4;
            Assert.IsTrue(interceptor.bCalled);
            Assert.AreEqual(8, leaf.Value);
        }

        //[TestMethod]
        //public void Test() {
        //  // Arrange
        //  // Act
        //  // Assert
        //}   
    }
}
