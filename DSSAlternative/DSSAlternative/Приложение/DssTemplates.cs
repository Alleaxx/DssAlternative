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
        private readonly string[] LoadPathes = new string[]
        {
            "legacy-study.json",
            "legacy-study-advanced.json",
            "legacy-riscs.json",
            "project-team-task.json",
            "project-tools-task.json",
            "work-task.json",
            "buy-task.json",
        };


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
            Templates = new List<ITemplate>();
            foreach (var path in LoadPathes)
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
