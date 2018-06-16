using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ED.Wp3.Server.BE.PrognosisMetadata
{    
    public class PrognosisMetadataProductionDto
    {
        public string ModelId { get; set; }
        public string ModelName { get; set; }
        public string ModelDescription { get; set; }
        public string WindAreaId { get; set; }
        public string WindAreaName { get; set; }
        public int ControlCode { get; set; }
        public string ControlName { get; set; }
        public string ControlValue { get; set; }
        public string ControlValueType { get; set; }
        public DateTime Updated { get; set; }
    }
}
