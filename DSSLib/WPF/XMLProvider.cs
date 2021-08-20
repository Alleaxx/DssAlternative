using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace DSSLib
{
    public interface IXMLProvider<T>
    {
        string ToXml(T obj);
        T FromXml(string xml);
    }
    public class DefaultXmlProvider<T> : IXMLProvider<T>
    {
        public string ToXml(T Object)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(T));

            StringBuilder stringBuilder = new StringBuilder();
            using (StringWriter sw = new StringWriter(stringBuilder))
            {
                formatter.Serialize(sw, Object);
            }
            return stringBuilder.ToString();
        }
        public T FromXml(string XML)
        {
            XmlSerializer formatterOptions = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(XML))
            {
                T data = (T)formatterOptions.Deserialize(reader);
                return data;
            }
        }
    }
}
