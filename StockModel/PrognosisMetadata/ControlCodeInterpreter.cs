using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ED.Wp3.Server.BE.PrognosisMetadata.Model;

namespace ED.Wp3.Server.BE.PrognosisMetadata
{
    public interface IControlCodeInterpreter
    {
        ControlCodeDescriptor Convert(int code, string value);
    }

    public class ControlCodeDescriptor
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public Type Type { get; set; }
        public string RunCategory { get; set; }
    }

    public delegate ControlCodeDescriptor ConvertDel(string s, ProductionControlDescriptor pcd);

    public class ProductionControlDescriptor
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public ConvertDel ConvertFunc { get; set; }
        public Type Type { get; set; }
        public string RunCategory { get; set; }
        public ControlCodeDescriptor Convert(string s) { return ConvertFunc ( s, this); }
    }
    public class ControlCodeInterpreter : IControlCodeInterpreter
    {
        // Todo have to be injected
        public readonly string LatestRunCategory = "Latest run";
        public readonly string CurrentRunCategory = "Current run";
        public readonly string NextRunCategory = "Next run";

        private Dictionary<int, ProductionControlDescriptor> _controlCodeControlDescriptors;

        public ControlCodeInterpreter()
        {
            _controlCodeControlDescriptors = new Dictionary<int, ProductionControlDescriptor>();
            _controlCodeControlDescriptors = bootstrap();           
        }

        public ControlCodeDescriptor Convert(int code, string value)
        {
            ControlCodeDescriptor result = null;
            try
            {
                result = _controlCodeControlDescriptors.ContainsKey(code)
                    ? _controlCodeControlDescriptors[code].Convert(value) : null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        private Dictionary<int, ProductionControlDescriptor> bootstrap()
        {
            Dictionary<int, ProductionControlDescriptor> ccds = new Dictionary<int, ProductionControlDescriptor>();
            // Create standard converters of more used types
            ConvertDel ConvertDT = (s, p) =>
            {
                var ccd = new ControlCodeDescriptor()
                {
                    Code = p.Code,
                    Name = p.Name,
                    RunCategory = p.RunCategory,
                    Type = p.Type,
                };
                DateTime dt;
                if ( DateTime.TryParse( s != string.Empty ? s : DateTime.MinValue.ToString(), out dt) )
                {
                    ccd.Value = dt;
                }
                return ccd;
            };
            ConvertDel ConvertBool = (s, p) => new ControlCodeDescriptor()
            {
                Code = p.Code,
                Name = p.Name,
                RunCategory = p.RunCategory,
                Type = p.Type,
                Value = s == "1"
            };
            ConvertDel ConvertString = (s, p) => new ControlCodeDescriptor()
            {
                Code = p.Code,
                Name = p.Name,
                RunCategory = p.RunCategory,
                Type = p.Type,
                Value = (s)
            };
            ConvertDel ConvertDecimal = (s, p) =>
            {
                var ccd = new ControlCodeDescriptor()
                {
                    Code = p.Code,
                    Name = p.Name,
                    RunCategory = p.RunCategory,
                    Type = p.Type,
                };
                decimal d;
                if ( Decimal.TryParse( s != string.Empty ? s : "-999.0", out d) )
                {
                    ccd.Value = d;
                }
                return ccd;
            };
            // Create dictionary
            ccds.Add(1, new ProductionControlDescriptor()
            {
                Code = 1,
                Name = "Error code",
                Type = typeof(int),
                RunCategory = LatestRunCategory,
                ConvertFunc = (s, p) =>
                {
                    var ccd = new ControlCodeDescriptor()
                    {
                        Code = p.Code,
                        Name = p.Name,
                        RunCategory = p.RunCategory,
                        Type = p.Type,
                        
                    };
                    int i;
                    if ( int.TryParse(s != string.Empty ? s : "-999", out i) )
                    {
                        ccd.Value = i;
                    }
                    return ccd;
                }
            });
            ccds.Add(2, new ProductionControlDescriptor()
            {
                Code = 2,
                Name = "Calculation time",
                Type = typeof(DateTime),
                RunCategory = LatestRunCategory,
                ConvertFunc = ConvertDT
            });
            ccds.Add(3, new ProductionControlDescriptor()
            {
                Code = 3,
                Name = "Weather forecast start time",
                Type = typeof(DateTime),
                RunCategory = LatestRunCategory,
                ConvertFunc = ConvertDT
            });
            ccds.Add(4, new ProductionControlDescriptor()
            {
                Code = 4,
                Name = "Machine",
                Type = typeof(string),
                RunCategory = LatestRunCategory,
                ConvertFunc = ConvertString
            });
            ccds.Add(5, new ProductionControlDescriptor()
            {
                Code = 5,
                Name = "Expected capacity",
                Type = typeof(decimal),
                RunCategory = LatestRunCategory,
                ConvertFunc = ConvertDecimal
            });
            ccds.Add(6, new ProductionControlDescriptor()
            {
                Code = 6,
                Name = "Production time",
                Type = typeof(DateTime),
                RunCategory = LatestRunCategory,
                ConvertFunc = ConvertDT
            });
            ccds.Add(7, new ProductionControlDescriptor()
            {
                Code = 7,
                Name = "Maximum capacity fulfilled",
                Type = typeof(bool),
                RunCategory = LatestRunCategory,
                ConvertFunc = ConvertBool
            });
            ccds.Add(101, new ProductionControlDescriptor()
            {
                Code = 101,
                Name = "Ongoing",
                Type = typeof(bool),
                RunCategory = CurrentRunCategory,
                ConvertFunc = ConvertBool
            });
            ccds.Add(102, new ProductionControlDescriptor()
            {
                Code = 102,
                Name = "Calculation time",
                Type = typeof(DateTime),
                RunCategory = CurrentRunCategory,
                ConvertFunc = ConvertDT
            });
            ccds.Add(103, new ProductionControlDescriptor()
            {
                Code = 103,
                Name = "Weather forecast start time",
                Type = typeof(DateTime),
                RunCategory = CurrentRunCategory,
                ConvertFunc = ConvertDT
            });
            ccds.Add( 104, new ProductionControlDescriptor()
            {
                Code = 104,
                Name = "Machine",
                Type = typeof(string),
                RunCategory = CurrentRunCategory,
                ConvertFunc = ConvertString
            });
            ccds.Add(105, new ProductionControlDescriptor()
            {
                Code = 105,
                Name = "Expected capacity",
                Type = typeof(decimal),
                RunCategory = CurrentRunCategory,
                ConvertFunc = ConvertDecimal
            });
            ccds.Add(106, new ProductionControlDescriptor()
            {
                Code = 106,
                Name = "Production time",
                Type = typeof(DateTime),
                RunCategory = CurrentRunCategory,
                ConvertFunc = ConvertDT
            });
            ccds.Add(107, new ProductionControlDescriptor()
            {
                Code = 107,
                Name = "Expected end time",
                Type = typeof(DateTime),
                RunCategory = CurrentRunCategory,
                ConvertFunc = ConvertDT
            });
            ccds.Add(201, new ProductionControlDescriptor()
            {
                Code = 201,
                Name = "Expected start time",
                Type = typeof(DateTime),
                RunCategory = NextRunCategory,
                ConvertFunc = ConvertDT
            });
            ccds.Add(202, new ProductionControlDescriptor()
            {
                Code = 202,
                Name = "Expected end time",
                Type = typeof(DateTime),
                RunCategory = NextRunCategory,
                ConvertFunc = ConvertDT
            });
            return ccds;
        }
    }
}
