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
    public interface IDssTemplates
    {
        event Action OnTemplatesLoaded;
        List<ITemplate> Templates { get;}
    }
    public class DssTemplates : IDssTemplates
    {
        private readonly HttpClient HttpClient;
        private readonly IDssJson Json;

        public event Action OnTemplatesLoaded;
        public List<ITemplate> Templates { get; private set; }
        public DssTemplates(HttpClient client, IDssJson json)
        {
            HttpClient = client;
            Json = json;
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
                if(template != null)
                {
                    Templates.Add(template);
                }
            }
            OnTemplatesLoaded?.Invoke();
            Console.WriteLine("Шаблоны загружены");
        }
        private async Task<Template> LoadTemplate(string path)
        {
            string json = await HttpClient.GetStringAsync($"{path}");
            Template template = Json.FromJson<Template>(json);
            return template;
        }
    }
}
