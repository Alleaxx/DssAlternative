using DSSAlternative.AHP;
using DSSAlternative.AHP.HierarchyInfo;
using DSSAlternative.AHP.Logs;
using DSSAlternative.AHP.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace DSSAlternative.Web.Services
{
    /// <summary>
    /// Интерфейс сервиса по по сериализации и десереализации JSON-объектов
    /// </summary>
    public interface IJsonService
    {
        /// <summary>
        /// Пытается десериализовать объект указанного типа из JSON.
        /// При неудаче возвращает default и записывает лог
        /// </summary>
        T CreateFromJson<T>(string text);

        /// <summary>
        /// Сериализует объект в JSON
        /// </summary>
        string ConvertToJson<T>(T obj);

        /// <summary>
        /// Преобразует проект в шаблон и затем сериализует в JSON
        /// </summary>
        string ConvertToJson(IProject project);
    }

    /// <summary>
    /// Стандартный сервис по сериализации и десереализации JSON-объектов
    /// </summary>
    public class JsonServiceDefault : IJsonService
    {
        private JsonSerializerOptions Options { get; init; }
        private ILogger Logger { get; init; }

        public JsonServiceDefault(ILogger logger)
        {
            Logger = logger;
            Options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                IgnoreNullValues = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            };
        }

        public string ConvertToJson<T>(T obj)
        {
            try
            {
                string json = JsonSerializer.Serialize(obj, Options);
                return json;
            }
            catch (JsonException ex)
            {
                Logger.AddError(obj, $"Ошибка сериализации в JSON объекта {typeof(T).Name}", ex.Message, LogCategory.JSON);
                return null;
            }
        }
        public string ConvertToJson(IProject project)
        {
            var newTemplate = new TemplateProject(project);
            return ConvertToJson<ITemplateProject>(newTemplate);
        }
        public string ConvertToJson(TemplateProject template)
        {
            return ConvertToJson<TemplateProject>(template);
        }


        public T CreateFromJson<T>(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return default;
            }
            try
            {
                T obj = JsonSerializer.Deserialize<T>(text, Options);
                Logger.AddInfo(text, $"Успешная десериализация {typeof(T).Name}", $"Получен объект {obj}", LogCategory.JSON);
                return obj;
            }
            catch (JsonException ex)
            {
                Logger.AddError(text, "Ошибка при десериализации JSON", ex.Message, LogCategory.JSON);
                return default;
            }
        }
    }
}
