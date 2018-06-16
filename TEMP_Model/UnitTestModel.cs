using System;
using ED.Atlas.Service.IC.BE.ModelHandlers.CompositeModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ED.Atlas.Service.IC.BE.Test {
    [TestClass]
    public class UnitTestModel {
        [TestMethod]
        public void TestGet() {
            // Setup , sti: /Intraday/SpotExchange/EPEXSPOT/Configuration/Server1
            Leaf<string> server1 = new Leaf<string>("Server1") { Value="epex1-sim.epexspot.com"};
            Leaf<string> server2 = new Leaf<string>("Server2")  { Value="epex2-sim.epexspot.com"};
            Composite config = new Composite("Configuration");
            config.Add(server1);
            config.Add(server2);
            Composite spot = new Composite("SpotExchange");
            spot.Add(new Composite("EPEXSPOT"));
            ((Composite)spot["EPEXSPOT"]).Add(config);
            Composite root = new Composite("Intraday");
            root.Add(spot);
            IModel model = new Model(root);
            // Test
            Component result = model.Get("/Intraday/SpotExchange/EPEXSPOT/Configuration/Server1");
            Assert.AreEqual(server1.Name, result.Name, "Forkert navn i resultatet");
            Assert.AreEqual(typeof(Leaf<string>), result.GetType(),"Resultatet har forkert type");
            result = model.Get("/Intraday/");
            Assert.AreEqual(root.Name, result.Name, "Forkert navn i resultatet");
            Assert.AreEqual(root.GetType(), result.GetType(), "Get(\"/Intraday/\") returnerer forkert type");
            result = model.Get("/Intraday/Heltforkert");
            Assert.IsNull( result, "Resultatet er ikke null");
        }
        [TestMethod]
        public void TestSet() {
            // Test Set med model rod
            // sti: /Intraday/SpotExchange/EPEXSPOT/Configuration/Server1
            // Opret rod
            IModel model = new Model();
            Composite root = new Composite("Intraday");
            model.Set("/", root);
            Assert.AreEqual( root.Name, model.Get("/Intraday").Name, "Forkert rod navn");
            model.Set("/Intraday", new Composite("SpotExchange"));
            model.Set("/Intraday/SpotExchange", new Composite("EPEXSPOT"));
            model.Set("/Intraday/SpotExchange", new Composite("APX"));
            model.Set("/Intraday/SpotExchange", new Composite("ELBAS"));
            model.Set("/Intraday/SpotExchange/EPEXSPOT", new Composite("Configuration"));
            Leaf<string> server1 = new Leaf<string>("Server1") {Value= "epex1-sim.epexspot.com"};
            model.Set("/Intraday/SpotExchange/EPEXSPOT/Configuration", server1);
            Assert.AreEqual(server1.Name, model.Get("/Intraday/SpotExchange/EPEXSPOT/Configuration/Server1").Name
                , "Forkert navn på blad Server1");
            Assert.AreEqual(
                  typeof(Leaf<string>)
                , model.Get("/Intraday/SpotExchange/EPEXSPOT/Configuration/Server1").GetType()
                , "Forkert type på blad"
            );
        }
        [TestMethod]
        public void TestSetNode() {
            // Test Set hvor man tilføjer en gren til en node
            IModel model = new Model();
            Composite root = new Composite("Intraday");
            Composite spotEx = new Composite("SpotExchange");
            model.Set(root, "/Intraday", spotEx);
            model.Set(spotEx, "/SpotExchange", new Composite("ELBAS"));
            Assert.AreEqual("ELBAS", model.Get(root, "/Intraday/SpotExchange/ELBAS").Name
                , "Forkert navn på /Intraday/SpotExchange/ELBAS");
            Assert.AreEqual(
                  typeof(Composite)
                , model.Get(root, "/Intraday/SpotExchange/ELBAS").GetType()
                , "Forkert type på ELBAS"
            );
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Exception er ikke rejst.")]
        public void TestSetRootBadly() {
            // Opret rod helt forkert
            IModel model = new Model();
            model.Set("/Heltforkert", new Composite("Intraday"));
        }
        public bool bListenerKaldt = false;
        public Component componentModtaget = null;
        public void ChangedListener(Component component) {
            bListenerKaldt = true;
            componentModtaget = component;
        }
        [TestMethod]
        public void TestSubscribe_Unsubscribe() {
            // Setup
            IModel model = new Model();
            Composite root = new Composite("Intraday");
            model.Set("/", root);
            ComponentChangedEventHandler el = new ComponentChangedEventHandler(ChangedListener);
            model.Subscribe("/Intraday", el);
            // Test
            model.Set("/Intraday", new Composite("SpotExchange"));
            Assert.IsTrue(bListenerKaldt,"Listener er ikke kaldt");
            Assert.AreEqual(root.Name, componentModtaget.Name,"Navn på component er forkert i listener");
            // Set up
            bListenerKaldt = false;
            componentModtaget = null;
            // Test
            model.Unsubscribe("/Intraday", el);
            model.Set("/Intraday", new Composite("Test"));
            Assert.IsFalse(bListenerKaldt, "listener kaldt efter unsubscribe");
            Assert.IsNull(componentModtaget, "component i listener er ikke null");
        }
        public class TestDoubleUpInterception : IInterceptor<int> {
            private int v = 3;
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
        public void TestInterception() {
            // Setup
            IModel model = new Model();
            Composite root = new Composite("Intraday");
            model.Set("/", root);
            Leaf<int> server1 = new Leaf<int>("Rate") { Value = 20};
            model.Set("/Intraday", server1);
            TestDoubleUpInterception interception = new TestDoubleUpInterception();
            interception.Kaldt = false;
            // Test
            bool res = model.SetInterception("/Intraday/Rate", interception);
            Assert.IsTrue(res, "Kunne ikke sætte interception");
            Component component = model.Get("/Intraday/Rate");
            int value = ((Leaf<int>) component).Value;
            Assert.AreEqual(typeof(Leaf<int>), component.GetType(),"Forkert component type");
            Assert.IsTrue(interception.Kaldt,"Interception er ikke kaldt");
            Assert.AreEqual(6, value, "Value er forkert");
        }

        //[TestMethod]
        //public void TestMethod1() {
        //}
    }
}
