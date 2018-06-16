using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto {
    public class SerieDto {
        public SerieDto(
                string name
            ,   string ticker
            ,   string valueType
            ,   string currency
            ,   string attribute
        ) {
            Name = name;
            Ticker = ticker;
            ValueType = valueType;
            Currency = currency;
            Attribute = attribute;
        }

        public string Name { get; set; }
        public string ValueType { get; set; }
        public string Currency { get; set; }
        public string Attribute { get; set; }
        public string Ticker { get; set; }
    }
}
