using ED.Atlas.Service.IC.BE.Handlers;
using ED.Atlas.Service.IC.BE.Messages;
using ED.Atlas.Service.IC.BE.ModelHandlers.CompositeModel;
using Composite = ED.Atlas.Service.IC.BE.ModelHandlers.CompositeModel.Composite;

namespace ED.Atlas.Service.IC.BE.ModelHandlers {
    public class IntradayModel : Model {
        private readonly IHandleDb _handleDb = null;
        private readonly IIntradayInterceptorFactory _interceptorFactory = null;
        public IntradayModel(IHandleDb handleDb, IIntradayInterceptorFactory interceptorFactory) {
            _handleDb = handleDb;
            _interceptorFactory = interceptorFactory;
        }
        public void Init() {
            string path = "/Intraday";
            Set("/", new CompositeModel.Composite("Intraday"));
            Set(path, new CompositeModel.Composite("DataProviders"));
            path += "/" + "DataProviders";
            foreach (var dataProvider in _handleDb.GetDataProviders()) {
                CompositeModel.Composite dpComposite = CreateDataProvider(dataProvider);
                Set(path, dpComposite);
            }
        }
        private CompositeModel.Composite CreateDataProvider(DataProvider dataProvider) {
            CompositeModel.Composite dpComposite = new CompositeModel.Composite(dataProvider.ShortName);
            string path = "/" + dpComposite.Name;
            Set( dpComposite, path, CreateConfiguration(dataProvider));
            Set( dpComposite, path, new CompositeModel.Composite("PriceZones"));
            path += "/" + "PriceZones";
            foreach (var priceZone in _handleDb.GetPriceZones(dataProvider.Id)) {
                Set(dpComposite, path, CreatePricezone(priceZone));
            }
            return dpComposite;
        }
        private LeafMap CreateConfiguration(DataProvider dataProvider) {
            Map map = new Map(); // Interceptor properties map
            map.Add("DataProviderId", dataProvider.Id, ItemChoiceType.@int);
            LeafMap dpConfigLeaf = new LeafMap("Configuration"
                , _interceptorFactory.Create<Map, ConfigurationInterceptor>(_handleDb, map));
            return dpConfigLeaf;
        }
        private CompositeModel.Composite CreatePricezone(PriceZone priceZone) {
            CompositeModel.Composite pzComposite = new CompositeModel.Composite(priceZone.Name);
            string path = "/" + pzComposite.Name;
            Leaf<string> timeZoneInfo = new Leaf<string>("Timezone");
            // Note: There is no need for an interceptor, because the timezone do not change.
            // In case it does then the front end is restarted in that very case.
            timeZoneInfo.Value = priceZone.TimezoneInfo;
            Set(pzComposite, path, CreateMarketSegments(priceZone.Id));
            Set(pzComposite, path, timeZoneInfo);
            Set(pzComposite, path, CreateTrades(priceZone));
            return pzComposite;
        }
        private Component CreateMarketSegments(int pzId) {
            // Create leaf map with aliases
            LeafMap marketSegmentsLeaf = new LeafMap("MarketSegments");
            // For the price zone create a marketsegment name - alias map
            foreach (
                MarketSegmentAlias marketSegmentAlias in _handleDb.GetMarketSegmentAliasesForPriceZone(pzId)) {
                if (!marketSegmentsLeaf.Value.Contains(marketSegmentAlias.Name)) {
                    // new market, so segment add name and alias                         
                    marketSegmentsLeaf.Value.Add<string>(
                        marketSegmentAlias.Name
                        , marketSegmentAlias.Alias ?? ""
                        , ItemChoiceType.@string);
                }
            }
            return marketSegmentsLeaf;
        }
        private CompositeModel.Composite CreateTrades(PriceZone priceZone) {
            CompositeModel.Composite tradesComposite = new CompositeModel.Composite("Trades");
            string path = "/" + tradesComposite.Name;
            Set( tradesComposite, path, CreateLatests(priceZone.Id));
            return tradesComposite;
        }
        private CompositeModel.Composite CreateLatests(int priceZoneId) {
            CompositeModel.Composite latestComposite = new CompositeModel.Composite("Latest");
            string path = "/" + latestComposite.Name;
            foreach (MarketSegment marketSegment in _handleDb.GetMarketSegmentsForPriceZone(priceZoneId)) {
                Map map = new Map(); // Interceptor properties map
                map.Add("PricezoneId", priceZoneId, ItemChoiceType.@int);
                map.Add("MarketSegment", marketSegment.Id, ItemChoiceType.@int );
                LeafMap latestTradeForMarketSegmentLeaf = new LeafMap(marketSegment.Name
                    , _interceptorFactory.Create<Map, LatestTradeInterceptor>(_handleDb, map));
                Set(latestComposite, path, latestTradeForMarketSegmentLeaf);
            }
            return latestComposite;
        }
    }
}
