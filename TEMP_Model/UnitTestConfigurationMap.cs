using ED.Atlas.Service.IC.BE.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ED.Atlas.Service.IC.BE.Test {
    [TestClass]
    public class UnitTestConfigurationMap {

        [TestMethod]
        public void TestAddandGet() {
            Map scm = new Map();
            scm.Add("Key", "Value", ItemChoiceType.@string);
            Assert.AreEqual(scm.Get<string>("Key"), "Value", "Get returnerer ikke korrekt");
        }
        [TestMethod]
        public void TestSet() {
            Map scm = new Map();
            scm.Add<string>("Key", "Value", ItemChoiceType.@string);
            Assert.AreEqual(scm.Get<string>("Key"), "Value", "Get returnerer ikke korrekt");
            scm.Set<string>("Key", "NewValue");
            Assert.AreEqual(scm.Get<string>("Key"), "NewValue", "Get returnerer ikke korrekt efter Set(..)");
        }
        [TestMethod]
        [ExpectedException(typeof(MapKeyNotFoundException))]
        public void TestRemove() {
            Map scm = new Map();
            scm.Add("Key", "Value", ItemChoiceType.@string);
            Assert.AreEqual(scm.Get<string>("Key"), "Value", "Get returnerer ikke korrekt");
            scm.Remove("Key");
            Assert.IsNull(scm.Get<string>("Key"), "Get returnerer ikke null");
        }
        [TestMethod]
        public void TestToCompoandCreateandEnumeration() {
            Map scm2Compo = new Map();
            scm2Compo.Add("Key", "Value", ItemChoiceType.@string);
            Assert.AreEqual(scm2Compo.Get<string>("Key"), "Value", "Get returnerer ikke korrekt");
            scm2Compo.Add("Key2", "Value2", ItemChoiceType.@string);
            Assert.AreEqual(scm2Compo.Get<string>("Key2"), "Value2", "Get returnerer ikke korrekt");
            Composite compo = scm2Compo.ToComposite();
            Map scmFromCompo = new Map();
            scmFromCompo.Create(compo);
            foreach (IMapEntry entry in scm2Compo) {
                Assert.AreEqual(scm2Compo.Get<string>(entry.Key)
                    , scmFromCompo.Get<string>(entry.Key), "Entry.Get<string>(" + entry.Key + ") er forkert");
            }
        }
        [TestMethod]
        public void TestSerialization() {
            IMap spotConf = new Map();
            spotConf.Add("ServerName1", "epex-test1.connect.comxerv.com", ItemChoiceType.@string);
            spotConf.Add("ServerName2", "epex-test2.connect.comxerv.com", ItemChoiceType.@string);
            spotConf.Add("CertificatePath", @"..\..\Resources\Certificate\EPEXTEST.p12", ItemChoiceType.@string);
            spotConf.Add("CertificatePassphrase", "6H9GEcs", ItemChoiceType.@string);
            spotConf.Add("Username", "CXEHPH01", ItemChoiceType.@string);
            spotConf.Add("Password", "ComX2014", ItemChoiceType.@string);
            spotConf.Add("Port", 50000, ItemChoiceType.@int);
            spotConf.Add("Ssl", "ssl", ItemChoiceType.@string);
            spotConf.Add("RequestedHeartbeat", 30, ItemChoiceType.@int);
            spotConf.Add("MarketId", "EPEX", ItemChoiceType.@string);
            spotConf.Add("ApplicationId1", "AJGHETG_0", ItemChoiceType.@string);
            spotConf.Add("ApplicationId2", "AJGHETG_1", ItemChoiceType.@string);
            string xml = spotConf.ToXml();
            Assert.IsTrue(xml.Length>0, "Serialisering fejlede");
            Map scmXml = new Map();
            scmXml.Create(xml);
            Assert.AreEqual(spotConf.Get<string>("Ssl"), scmXml.Get<string>("Ssl"), "[Ssl] value er ikke ens");
            Assert.AreEqual(spotConf.Get<int>("Port"), scmXml.Get<int>("Port"), "[Port] value er ikke ens");
        }

        [TestMethod]
        public void TestTradeDirectionLeaf() {
            IMap map = new Map();
            map.Add( "Direction",  LeafSide.S, ItemChoiceType.side);
            string xml = map.ToXml();
            IMap fromXmlMap = new Map();
            fromXmlMap.Create(xml);
            Assert.AreEqual( map.Get<LeafSide>("Direction"), fromXmlMap.Get<LeafSide>("Direction"), "Direction er forkert");
        }
        [TestMethod]
        public void TestEmptyComponentToMap() {
            Map map = new Map() {Name = "Map"};
            Composite composite = map.ToComposite();
            Map mapFromCompo = new Map();
            mapFromCompo.Create(composite);
            Assert.AreEqual(map.Name, mapFromCompo.Name, "Name er forkert");
            Assert.AreEqual(map.Count, mapFromCompo.Count, "Count er forkert");
        }

        //[TestMethod]
        //public void Test() {
        //}


    }
}
