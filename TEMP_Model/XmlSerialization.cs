using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ED.Atlas.Service.IC.BE.Messages {
    public enum XmlEncoding {
          Utf16 = 0
        , Utf8 = 1
    }
    public class XmlSerialization {
        public static string Serialize<T>(T t, XmlEncoding enc = XmlEncoding.Utf8) {
            Encoding encoding = enc == XmlEncoding.Utf8 ? Encoding.UTF8 : Encoding.Unicode; 
            var xmlWriterSettings = new XmlWriterSettings() {
                  Encoding = encoding
                , NamespaceHandling = NamespaceHandling.Default
                , OmitXmlDeclaration = true
                , Indent = false
                , ConformanceLevel = ConformanceLevel.Document
            };
            var xmlSerializer = new XmlSerializer(typeof(T));
            var sb = new StringBuilder();
            XmlWriter xmlWriter;
            if (encoding == Encoding.UTF8) {
                 xmlWriter = XmlWriter.Create(new StringWriterUtf8(sb), xmlWriterSettings);
            }
            else {
                xmlWriter = XmlWriter.Create(new StringWriter(sb), xmlWriterSettings); // Note: Generate UTF-16 / Unicode xml    
            }
            xmlSerializer.Serialize(xmlWriter, t);
            return sb.ToString();
        }
        public static T Deserialize<T>(string xml) {
            TextReader txtreader = new StringReader(xml);
            var deSerializer = new XmlSerializer(typeof(T));
            return (T) deSerializer.Deserialize(txtreader);
        }
        class StringWriterUtf8 : StringWriter {
            public StringWriterUtf8(StringBuilder sb)
                : base(sb) {
                // Intentionally nothing here
            }
            public override Encoding Encoding {
                get { return Encoding.UTF8; }
            }
        }
    }
}
