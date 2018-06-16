using System.Collections.Generic;
using System.Linq;
using ED.Atlas.Service.IC.BE.Handlers;
using ED.Atlas.Service.IC.BE.Messages;
using ED.Atlas.Service.IC.BE.ModelHandlers;
using ED.Atlas.Service.IC.BE.ModelHandlers.CompositeModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Composite = ED.Atlas.Service.IC.BE.ModelHandlers.CompositeModel.Composite;

namespace ED.Atlas.Service.IC.BE.Test {
    [TestClass]
    public class UnitTestIntradayModel {
        [TestMethod]
        public void TestInit() {
            IntradayInterceptorFactory interceptorFactory = new IntradayInterceptorFactory();
            var mockHandleDb = new Mock<IHandleDb>();
            var dataProviders = CreateDataProviders();
            mockHandleDb.Setup(hd => hd.GetDataProviders()).Returns(dataProviders);
            var priceZones = CreatePriceZones();
            mockHandleDb.Setup(hd => hd.GetPriceZones(1)).Returns(priceZones);
            mockHandleDb.Setup(hd => hd.GetPriceZones(2)).Returns(priceZones);
            mockHandleDb.Setup(hd => hd.GetMarketSegmentsForPriceZone(1)).Returns(CreateMarketSegments);
            mockHandleDb.Setup(hd => hd.GetMarketSegmentsForPriceZone(2)).Returns(CreateMarketSegments);
            IntradayModel model = new IntradayModel(mockHandleDb.Object, interceptorFactory);
            // Test
            model.Init();
            int cnt = 0;
            foreach (Component component in (ModelHandlers.CompositeModel.Composite) model.Get("/Intraday/DataProviders")) {
                DataProvider found = dataProviders.Find(x => x.ShortName == component.Name);
                Assert.IsTrue( found!=null, "Forkert navn i ");
                cnt++;
            }
            Assert.AreEqual(2, cnt,"Forkert antal i /Intraday");
            ModelHandlers.CompositeModel.Composite apxLatest = (ModelHandlers.CompositeModel.Composite) model.Get("/Intraday/DataProviders/APX API/PriceZones/UK/Trades/Latest");
            Assert.AreEqual("Latest", apxLatest.Name, "APX latest trade har forkert navn");
            Assert.AreEqual(3, apxLatest.Count(), "Forkert antal leafs i apxLatest");
            LeafMap apxLatestQH = (LeafMap) model.Get("/Intraday/DataProviders/APX API/PriceZones/UK/Trades/Latest/S_QH");
            Assert.AreEqual( "S_QH", apxLatestQH.Name, "Forkert navn på S_QH gren");
        }
        private List<DataProvider> CreateDataProviders() {
            List<DataProvider> dataProviders = new List<DataProvider>();
            DataProvider dp = new DataProvider();
            dp.Id = 1;
            dp.Name = "APX Datacapture API";
            dp.ShortName = "APX API";
            dataProviders.Add(dp);
            dp = new DataProvider();
            dp.Id = 2;
            dp.Name = "ComXerv";
            dp.ShortName = "ComXerv";
            dataProviders.Add(dp);
            return dataProviders;
        }
        private List<PriceZone> CreatePriceZones() {
            List<PriceZone> priceZones = new List<PriceZone>();
            PriceZone pz = new PriceZone() {
                Id = 1,
                Name = "UK",
                DataProviderId = 1,
                ContryId = 826,
                TimezoneInfo = "GMT Standard Time"
            };
            priceZones.Add(pz);
            pz = new PriceZone() {
                Id = 2,
                Name = "DE-EON",
                DataProviderId = 1,
                ContryId = 276,
                TimezoneInfo = "GMT Standard Time"
            };
            priceZones.Add(pz);
            return priceZones;
        }
        private List<MarketSegment> CreateMarketSegments() {
            List<MarketSegment> segments = new List<MarketSegment>();
            MarketSegment ms = new MarketSegment() {
                    DeliveryPeriods = 1
                ,   Id = 1
                ,   InstrumentTemplate = "1H-YYYYMMDD-XX"
                ,   ListingFrequency = 24
                ,   Name = "S_1H"
            };
            segments.Add(ms);
            ms = new MarketSegment() {
                    DeliveryPeriods = 2
               ,    Id = 2
               ,    InstrumentTemplate = "HH-YYYYMMDD-XX"
               ,    ListingFrequency = 48
               ,    Name = "S_HH"
            };
            segments.Add(ms);
            ms = new MarketSegment() {
                    DeliveryPeriods = 4
               ,    Id = 3
               ,    InstrumentTemplate = "QH-YYYYMMDD-XX"
               ,    ListingFrequency = 96
               ,    Name = "S_QH"
            };
            segments.Add(ms);
            return segments;
        } 
        public class TestConfigInterceptor : IInterceptor<Map> {
            public TestConfigInterceptor() {
                GetCalled = false;
                SetCalled = false;
                ValueMap = new Map();
                ValueMap.Add("Test", "Testværdi", ItemChoiceType.@string);
            }
            public bool GetCalled { get; set; }
            public bool SetCalled { get; set; }
            public Map ValueMap { get; set; }
            public Map Get() {
                GetCalled = true;
                return ValueMap;
            }
            public void Set(Map val) {
                SetCalled = true;
                ValueMap =val;
            }
        }
        [TestMethod]
        public void TestInterceptor() {
            IntradayInterceptorFactory interceptorFactory = new IntradayInterceptorFactory();
            var mockHandleDb = new Mock<IHandleDb>();
            var dataProviders = CreateDataProviders();
            mockHandleDb.Setup(hd => hd.GetDataProviders()).Returns(dataProviders);
            IntradayModel model = new IntradayModel(mockHandleDb.Object, interceptorFactory);
            model.Init();
            LeafMap config = (LeafMap) model.Get("/Intraday/DataProviders/APX API/Configuration");
            TestConfigInterceptor interceptor = new TestConfigInterceptor();
            // Test
            Assert.IsTrue(model.SetInterception("/Intraday/DataProviders/APX API/Configuration", interceptor)
                , "model.SetInterception(...) returnere falsk");
            Map getMap = config.Value;
            Assert.IsTrue(interceptor.GetCalled, "Get på interceptor er ikke kaldt");
            Assert.AreEqual("Testværdi", getMap.Get<string>("Test"), "Forkert værdi i map for Test");
            Map setMap = new Map();
            setMap.Add("Test2", 2, ItemChoiceType.@int);
            config.Value = setMap;
            Assert.IsTrue(interceptor.SetCalled, "Set på interceptor er ikke kaldt");
            Assert.AreEqual(2, setMap.Get<int>("Test2"), "Forkert værdi i map for Test2");
        }
        //[TestMethod]
        //public void TestMethod1() {
        //}
    }
}
