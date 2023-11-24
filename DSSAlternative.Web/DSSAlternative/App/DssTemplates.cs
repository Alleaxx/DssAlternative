using DSSAlternative.AHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace DSSAlternative.Web.AppComponents
{
    public interface IDssTemplates
    {
        event Action OnTemplatesLoaded;
        List<ITemplateProject> Templates { get;}
        void RemoveTemplate(ITemplateProject template);
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
        public List<ITemplateProject> Templates { get; private set; }
        public DssTemplates(HttpClient client, IDssJson json)
        {
            HttpClient = client;
            Json = json;
            LoadTemplates();
        }
        private async void LoadTemplates()
        {
            Templates = new List<ITemplateProject>();
            foreach (var path in LoadPathes)
            {
                ITemplateProject template = await LoadTemplate($"sample-data/{path}");
                if(template != null)
                {
                    Templates.Add(template);
                }
            }
            OnTemplatesLoaded?.Invoke();
            Console.WriteLine("Шаблоны загружены");
        }
        private async Task<TemplateProject> LoadTemplate(string path)
        {
            string json = await HttpClient.GetStringAsync($"{path}");
            TemplateProject template = Json.FromJson<TemplateProject>(json);
            return template;
        }

        public void RemoveTemplate(ITemplateProject template)
        {
            Templates.Remove(template);
        }
    }
}
