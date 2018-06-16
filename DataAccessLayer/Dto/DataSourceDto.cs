using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto {
    public class DatasourceDto {
        public DatasourceDto(string ticker, string url, string config, string market) {
            Ticker = ticker;
            Url = url;
            Config = config;
            Market = market;
        }
        public string Ticker { get; set; }
        public string Url { get; set; }
        public string Config { get; set; }
        public string Market { get; set; }
    }
}
