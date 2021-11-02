using DSSAlternative.AHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace DSSAlternative.AppComponents
{
    public interface IDssJson
    {
        T FromJson<T>(string text);
        string ToJson(Template template);
        string ToJson(IHierarchy hier);
        string ToJson(IProject project);
        string ToJson<T>(T obj);
    }
    public class DssJson : IDssJson
    {
        private JsonSerializerOptions Options { get; init; }
        public DssJson()
        {
            Options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                IgnoreNullValues = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            };
        }

        public string ToJson<T>(T obj)
        {
            try
            {
                string json = JsonSerializer.Serialize(obj, Options);
                return json;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Не удалось сериализовать объект:");
                Console.WriteLine(ex.Message);
                return "";
            }
        }
        public string ToJson(IProject project)
        {
            return ToJson(new Template(project));
        }
        public string ToJson(IHierarchy hier)
        {
            return ToJson(new Template(hier));
        }
        public string ToJson(Template template)
        {
            return ToJson<Template>(template);
        }

        public T FromJson<T>(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return default;
            }
            try
            {
                T obj = JsonSerializer.Deserialize<T>(text, Options);
                return obj;
            }
            catch (JsonException ex)
            {
                Console.WriteLine(ex.Message);
                return default;
            }
        }
    }
}
