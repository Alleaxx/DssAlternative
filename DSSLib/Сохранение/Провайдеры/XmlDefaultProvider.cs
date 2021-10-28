using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace DSSLib
{
    //XML
    public class XmlDefaultProvider<T> : ITextProvider<T>
    {
        public string ToTextString(T Object)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(T));

            StringBuilder stringBuilder = new StringBuilder();
            using (StringWriter sw = new StringWriter(stringBuilder))
            {
                try
                {
                    formatter.Serialize(sw, Object);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
            }
            return stringBuilder.ToString();
        }
        public T FromTextString(string XML)
        {
            XmlSerializer formatterOptions = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(XML))
            {
                try
                {
                    T data = (T)formatterOptions.Deserialize(reader);
                    return data;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return default;
                }
            }
        }
    }
}
