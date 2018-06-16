using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CompositeModel.Xml {

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
                // Note: Generate UTF-16 / Unicode xml    
                xmlWriter = XmlWriter.Create(new StringWriter(sb), xmlWriterSettings);                 
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

        public static Xml.ComponentType ToComponentType(Component component) {
            return component.Type == ComponentBaseType.Compo 
                ? CompositeToComponentType(component) 
                    : LeafToComponentType(component);
        }

        /// <summary>
        /// Convert Composite to XML ComponentType. 
        /// </summary>
        /// <returns>Serialized component.</returns>
        public static Xml.ComponentType CompositeToComponentType(Component component) {
            CompositeModel.Composite composite = (CompositeModel.Composite)component;
            var xmlComposite = new Composite();
            xmlComposite.Name = component.Name;
            xmlComposite.Type = ComponentTypeType.Compo;
            xmlComposite.Component = new ComponentType[composite.Count];
            int ix = 0;
            foreach (var compo in composite) {
                xmlComposite.Component[ix++] = ToComponentType(compo);
            }
            return xmlComposite;
        }

        /// <summary>
        /// Convert a leaf given a component of unknown type
        /// </summary>
        /// <param name="component">The leaf component to convert</param>
        /// <returns></returns>
        public static Xml.ComponentType LeafToComponentType(Component component) {
            switch ( ((ILeaf)component).GetItemType()) {
                case LeafBaseType.datetime:
                    return LeafToComponentType<DateTime>((Leaf<DateTime>)component);
                case LeafBaseType.decimalFrac3:
                case LeafBaseType.decimalFrac4:
                case LeafBaseType.@decimal:
                    return LeafToComponentType<decimal>((Leaf<decimal>)component);
                case LeafBaseType.@double:
                    return LeafToComponentType<double>((Leaf<double>)component);
                case LeafBaseType.@int:
                    return LeafToComponentType<int>((Leaf<int>)component);
                case LeafBaseType.@long:
                    return LeafToComponentType<long>((Leaf<long>)component);
                case LeafBaseType.positiveInteger:
                    return LeafToComponentType<uint>((Leaf<uint>)component);
                case LeafBaseType.@string:
                    return LeafToComponentType<string>((Leaf<string>)component);
                default:
                    return null;

            }
        }

        /// <summary>
        /// Convert Leaf<T> to XML ComponentType.
        /// </summary>
        /// <typeparam name="T">The actual type of the leaf (int, string etc.)</typeparam>
        /// <param name="leaf">The leaf it self.</param>
        /// <returns></returns>
        public static Xml.ComponentType LeafToComponentType<T>(Leaf<T> leaf) {
            Leaf xmlLeaf = new Leaf();
            xmlLeaf.Name = leaf.Name;
            xmlLeaf.Item = leaf.Value;
            xmlLeaf.ItemElementName = (ItemChoiceType) leaf.GetItemType();
            xmlLeaf.Type = ComponentTypeType.Leaf;
            return xmlLeaf;
        }

    }

}
