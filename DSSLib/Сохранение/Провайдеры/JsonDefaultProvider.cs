using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace DSSLib
{
    //JSON
    public class JsonDefaultProvider<T> : ITextProvider<T>
    {
        public JsonSerializerOptions Options { get; private set; }
        public JsonDefaultProvider()
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
