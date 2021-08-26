using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace DSSLib
{
    //Преобразовать объект в строковое представление
    public interface ITextProvider<T>
    {
        string ToTextString(T obj);
        T FromTextString(string text);
    }


    //XML
    public class XmlProvider<T> : ITextProvider<T>
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

    //JSON
    public class JsonProvider<T> : ITextProvider<T>
    {
        public JsonSerializerOptions Options { get; private set; }
        public JsonProvider()
        {
            Options = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };
        }

        public T FromTextString(string text)
        {
            return JsonSerializer.Deserialize<T>(text);
        }

        public string ToTextString(T obj)
        {
            return JsonSerializer.Serialize(obj);
        }
    }



}
