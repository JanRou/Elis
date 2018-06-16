﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockImportService.Importers.NasdaqOmxCsv {
    public class NasdaqOmxStock {
        public NasdaqOmxStock(
            string ticker
            , string currency
            , string stockName
            , string market
            , string valueTypeName
            , string url
            , string volumeValueTypeName
         ) {
            Ticker = ticker;
            Currency = currency;
            StockName = stockName;
            Market = market;
            ValueTypeName = valueTypeName;
            Url = url;
            VolumeValueTypeName = volumeValueTypeName;
        }
        public string Ticker { get; set; }
        public string Currency { get; set; }
        public string StockName { get; set; }
        public string Market { get; set; }
        public string ValueTypeName { get; set; }
        public string Url { get; set; }
        public string VolumeValueTypeName { get; set; }
    }
}
