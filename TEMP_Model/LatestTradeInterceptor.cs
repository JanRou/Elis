using System;
using ED.Atlas.Service.IC.BE.Handlers;
using ED.Atlas.Service.IC.BE.Messages;
using ED.Atlas.Service.IC.BE.ModelHandlers.CompositeModel;

namespace ED.Atlas.Service.IC.BE.ModelHandlers {
    public class LatestTradeInterceptor : IInterceptor<Map>, IIntradayInterceptorProperties {
        public Map Map { get; set; } // Trade contains several values
        public IHandleDb HandleDb { get; set; }
        public Map Get() {
            return ToMap(HandleDb.GetLatestTrade(Map.Get<int>("PricezoneId"), Map.Get<int>("MarketSegment")));
        }
        public void Set(Map val) {
            // Last trade is never set.
        }
        /// <summary>
        /// ToMap returns an empty map in case a trade is null. 
        /// </summary>
        /// <param name="trade">Trade which will be a map.</param>
        /// <returns>Map with selected data fra the trade or an empty map.</returns>
        public Map ToMap(Trade trade) {
            Map ret = new Map();
            if (trade != null) {
                ret.Add<long>("ContractId", trade.ContractId, ItemChoiceType.@long);
                ret.Add<DateTime>("BeginTime", trade.BeginTime, ItemChoiceType.@datetime);
                ret.Add<string>("InstrumentCode", trade.InstrumentCode, ItemChoiceType.@string);
                ret.Add<LeafSide>("Direction"
                    , trade.Direction == "B" ? LeafSide.B : LeafSide.S
                    , ItemChoiceType.@side);
                ret.Add<DateTime>("TradingTime", trade.TradingTime, ItemChoiceType.@datetime);
            }
            return ret;
        }
    }
}
