using DSSAlternative.AHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace DSSAlternative.AppComponents
{

    public class TemplatesEditor
    {
        private readonly HttpClient HttpClient;
        public JsonSerializerOptions JsonOptions { get; set; }

        public event Action OnTemplatesLoaded;
        public List<ITemplate> Templates { get; private set; }
        public TemplatesEditor(HttpClient client)
        {
            HttpClient = client;
            JsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                IgnoreNullValues = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            };
            LoadTemplates();
        }
        private async void LoadTemplates()
        {
            string[] Pathes = new string[]
            {
                "legacy-study.json",
                "legacy-study-advanced.json",
                "legacy-riscs.json",
                "project-team-task.json",
                "project-riscs-task.json",
                "project-tools-task.json",
                "work-task.json",
                "study-task.json",
                "buy-task.json",
            };
            Templates = new List<ITemplate>();
            foreach (var path in Pathes)
            {
                ITemplate template = await LoadTemplate($"sample-data/{path}");
                Templates.Add(template);
            }
            OnTemplatesLoaded?.Invoke();
            Console.WriteLine("Шаблоны загружены");
        }
        private async Task<Template> LoadTemplate(string path)
        {
            string json = await HttpClient.GetStringAsync($"{path}");
            Template template = JsonSerializer.Deserialize<Template>(json, JsonOptions);
            return template;
        }
    }
}
