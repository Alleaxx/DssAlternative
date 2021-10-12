using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

using Microsoft.AspNetCore.Components;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Net.Http;

namespace DSSAlternative.AHP
{
    public class DSS
    {
        private readonly HttpClient HttpClient;
        public DSS(HttpClient client)
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
        public JsonSerializerOptions JsonOptions { get; set; }
        private async void LoadTemplates()
        {
            string[] Pathes = new string[]
            {
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
            Console.WriteLine("Шаблоны загружены");
        }
        private async Task<Template> LoadTemplate(string path)
        {
            string json = await HttpClient.GetStringAsync($"{path}");
            Template template = JsonSerializer.Deserialize<Template>(json, JsonOptions);
            return template;
        }





        public event Action<IProject> OnProjectSelected;

        public IRatingSystem RatingSystem { get; private set; } = new RatingDefaultSystem();
        public List<ITemplate> Templates { get; set; }
        //Все проблемы
        public List<IProject> Projects { get; private set; } = new List<IProject>();
        public IProject Project { get; private set; }


        public void SelectProject(IProject project)
        {
            IProject old = Project;
            Project = project;
            Update();

            if(old != null)
            {
                old.UpdatedHierOrRelationChanged -= Update;
            }
            project.UpdatedHierOrRelationChanged += Update;

            void Update()
            {
                OnProjectSelected?.Invoke(project);
            }
        }

        public void AddProject(IProject project)
        {
            Projects.Add(project);
            SelectProject(project);
        }

        public void RemoveProject(IProject project)
        {
            Projects.Remove(project);
            bool anotherProjectAvail = Projects.Count > 0;


            if (project == Project && anotherProjectAvail)
            {
                SelectProject(Projects.First());
            }
            else if (!anotherProjectAvail)
            {
                Project = null;
            }
        }


        public DssState CreateState()
        {
            return new DssState()
            {
                Saved = DateTime.Now,
                OpenedTemplates = Projects.Select(pr => new Template(pr)),
                SelectedTemplate = Projects.IndexOf(Project)
            };
        }
    }
}
