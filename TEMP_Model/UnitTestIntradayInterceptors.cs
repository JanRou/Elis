using System;
using ED.Atlas.Service.IC.BE.Handlers;
using ED.Atlas.Service.IC.BE.Messages;
using ED.Atlas.Service.IC.BE.ModelHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ED.Atlas.Service.IC.BE.Test {
    [TestClass]
    public class UnitTestIntradayInterceptors {

        [TestMethod]
        public void TestConfigurationInterceptor() {
            // Set up
            int dpId = 1;
            // Opret xml til mock, der simulerer atabasen.
            Map scm = new Map();
            scm.Add("Key", "Value", ItemChoiceType.@string);
            scm.Add("Key2", 2, ItemChoiceType.@int);
            string xml = scm.ToXml();
            // opret configuration fra db
            FrontEndConfiguration feConfig = new FrontEndConfiguration();
            feConfig.ConfigXml = xml;
            // Opret faked handle db, der kan hente (get) og sætte (set) faked configuration
            var mockHandelDb = new Mock<IHandleDb>();
            mockHandelDb.Setup(ci => ci.GetFrontEndConfiguration(dpId)).Returns(feConfig);
            mockHandelDb.Setup(ci => ci.SetFrontEndConfiguration(dpId, xml));
            // Test Get
            IntradayInterceptorFactory interceptorFactory = new IntradayInterceptorFactory();
            Map map = new Map();
            map.Add("DataProviderId", dpId, ItemChoiceType.@int);
            var confInterceptor = (ConfigurationInterceptor) 
                interceptorFactory.Create<Map, ConfigurationInterceptor>(mockHandelDb.Object, map);
            Map scmGet = confInterceptor.Get();
            Assert.AreEqual(scm.Get<string>("Key"), scmGet.Get<string>("Key"),"Key value er forkert.");
            Assert.AreEqual(scm.Get<int>("Key2"), scmGet.Get<int>("Key2"), "Key2 value er forkert.");
            mockHandelDb.Verify(ci => ci.GetFrontEndConfiguration(dpId));
            // Test Set
            confInterceptor.Set(scm);
            mockHandelDb.Verify(ci => ci.SetFrontEndConfiguration(dpId, xml));
        }
        [TestMethod]
        public void TestLatestTradeInterceptor() {
            // Set up
            int priceZoneId = 1;
            int marketSegmentId = 2;
            // Opret xml til som bliver faked hentet fra databasen.
            Trade trade = new Trade();
            trade.ContractId = 123;
            trade.BeginTime = DateTime.Today;
            trade.InstrumentCode = "HH-20140718-37";
            trade.Direction = "B";
            var mockHandelDb = new Mock<IHandleDb>();
            mockHandelDb.Setup(gt => gt.GetLatestTrade(priceZoneId, marketSegmentId)).Returns(trade);
            // Test Get
            IntradayInterceptorFactory interceptorFactory = new IntradayInterceptorFactory();
            Map map = new Map(); // Interceptor properties map
            map.Add("PricezoneId", priceZoneId, ItemChoiceType.@int);
            map.Add("MarketSegment", marketSegmentId, ItemChoiceType.@int);
            var latestTradeInterceptor = (LatestTradeInterceptor)
                interceptorFactory.Create<Map, LatestTradeInterceptor>(mockHandelDb.Object, map);
            Map latesMap = latestTradeInterceptor.Get();
            Assert.AreEqual(trade.ContractId, latesMap.Get<long>("ContractId"), "ContractId er forkert");
            Assert.AreEqual(trade.BeginTime, latesMap.Get<DateTime>("BeginTime"), "BeginTime er forkert");
            Assert.AreEqual(trade.InstrumentCode, latesMap.Get<string>("InstrumentCode"), "ContractId er forkert");
            Assert.AreEqual(
                    trade.Direction == "B" ? LeafSide.B : LeafSide.S
                ,   latesMap.Get<LeafSide>("Direction"), "Direction er forkert");
            mockHandelDb.Verify(gt => gt.GetLatestTrade(priceZoneId, marketSegmentId));
        }

        //[TestMethod]
        //public void TestMethod1() {
        //}

    }
}
