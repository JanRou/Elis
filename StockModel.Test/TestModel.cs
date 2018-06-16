using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using StockModel.Model;

namespace StockModel.Test {

    [TestClass]
    public class TestModel {

        [TestMethod]
        public void TestGet() {

            // Arrange
            IModel dut = new Model.Model();
            IComposite test = new Composite("Test");
            // Act

            dut.Set("/",test);
            IComponent res = dut.Get("/Test");

            // Assert
            Assert.AreEqual("Test", res.Name);
        }

        [TestMethod]
        public void TestGetWihtSubNode() {

            // Arrange
            IComposite test = new Composite("Test");
            IComposite sub = new Composite("Sub");
            test.Add(sub);
            IModel dut = new Model.Model();
            dut.Set("/", test);

            // Act
            IComponent res = dut.Get("/Test/Sub");

            // Assert
            Assert.AreEqual("Sub", res.Name);
        }

        [TestMethod]
        public void TestGetWihtSubNodeWithLeaf() {

            // Arrange
            IComposite test = new Composite("Test");
            IComposite sub = new Composite("Sub");
            ILeaf leaf = new Leaf<int>("Number") { Value = 3 };
            sub.Add(leaf);
            test.Add(sub);
            IModel dut = new Model.Model();
            dut.Set("/", test);

            // Act
            IComponent res = dut.Get("/Test/Sub/Number");

            // Assert
            Assert.AreEqual("Number", res.Name);
            Assert.AreEqual( ComponentType.Leaf, res.Type);
            Assert.AreEqual( 3, ((Leaf<int>)res).Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Exception is not raised.")]
        public void TestSetRootWrong() {            
            IModel model = new Model.Model();
            model.Set("/ImBad", new Composite("ImBad"));
        }

        [TestMethod]
        public void TestSubscribe() {
            // Arrange
            IComposite test = new Composite("Test");
            IComposite sub = new Composite("Sub");
            IModel dut = new Model.Model();
            bool bListenerCalled = false;
            IComponent ComponentChanged = null;
            ComponentChangedEventHandler componentChangedEventhandler =
                    new ComponentChangedEventHandler((c) => {
                        bListenerCalled = true;
                        ComponentChanged = c;
                    } );
            dut.Set("/", test);

            // Act
            dut.Subscribe("/Test", componentChangedEventhandler);
            dut.Set( "/Test", sub);

            // Assert
            Assert.IsTrue(bListenerCalled);
            Assert.AreEqual("Test", ComponentChanged.Name);
        }

        [TestMethod]
        public void TestChangedEventWhenLeafValueChanged() {
            // Arrange
            IComposite test = new Composite("Test");
            IComposite sub = new Composite("Sub");
            Leaf<int> leaf = new Leaf<int>("Number") { Value = 3 };
            sub.Add(leaf);
            IModel dut = new Model.Model();
            bool bListenerCalled = false;
            IComponent ComponentChanged = null;
            ComponentChangedEventHandler componentChangedEventhandler =
                    new ComponentChangedEventHandler((c) => {
                        bListenerCalled = true;
                        ComponentChanged = c;
                    });
            dut.Set("/", test);
            dut.Set("/Test", sub);
            dut.Subscribe("/Test", componentChangedEventhandler);

            // Act
            leaf.Value = 6;

            // Assert
            Assert.IsTrue(bListenerCalled);
        }

        [TestMethod]
        public void TestUnsubscribe() {

            // Arrange
            IComposite test = new Composite("Test");
            IComposite sub = new Composite("Sub");
            ILeaf leaf = new Leaf<int>("Number") { Value = 3 };
            IModel dut = new Model.Model();
            bool bListenerCalled = false;
            IComponent ComponentChanged = null;
            ComponentChangedEventHandler componentChangedEventhandler =
                    new ComponentChangedEventHandler((c) => {
                        bListenerCalled = true;
                        ComponentChanged = c;
                    });
            dut.Set("/", test);
            dut.Subscribe("/Test", componentChangedEventhandler);

            // Act
            dut.Set("/Test", sub);
            bListenerCalled = !bListenerCalled; // 
            dut.Unsubscribe("/Test", componentChangedEventhandler);
            dut.Set("/Test/Sub", leaf);

            // Assert
            Assert.IsFalse(bListenerCalled);
        }

        public class Interceptor : ILeafInterceptor<int> {

            private int _value;
            public Interceptor(int value) {
                _value = value;
            }
            public bool Called { get; set; }

            public int Get() {
                return _value + 1;
            }

            public void Set(int val) {
                _value = val;
                Called = true;
            }
        }

        [TestMethod]
        public void TestInterceptor() {

            // Arrange
            int baseValue = 3;
            IComposite test = new Composite("Test");
            IComposite sub = new Composite("Sub");
            test.Add(sub);
            ILeaf leaf = new Leaf<int>("Number") {
                    Value = 1
                ,   Interceptor = new Interceptor(1)
            };
            sub.Add(leaf);
            IModel dut = new Model.Model();
            dut.Set("/", test);

            // Act
            ((Leaf<int>)dut.Get("/Test/Sub/Number")).Value = baseValue;
            int res = ((Leaf<int>)dut.Get("/Test/Sub/Number")).Value;

            // Assert
            Assert.AreEqual( baseValue+1, res);
        }

        [TestMethod]
        public void TestToModelString() {

            // Arrange
            IComposite test = new Composite("Test");
            IComposite sub = new Composite("Sub");
            test.Add(sub);
            ILeaf leaf = new Leaf<int>("Number") { Value = 3   };
            sub.Add(leaf);
            IModel dut = new Model.Model();
            dut.Set("/", test);

            // Act
            List<ModelString> modelView = new List<ModelString>();
            foreach (ModelString modelString in dut.ToModelString()) {
                modelView.Add(modelString);
            }

            // Assert
            Assert.AreEqual( 4, modelView.Count);
            Assert.AreEqual( "Sub", modelView[1].Name);
            Assert.AreEqual( "3", modelView[3].Value);
        }


        //[TestMethod]
        //public void Test() {

        //    // Arrange

        //    // Act

        //    // Assert
        //}
    }
}
