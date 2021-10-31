using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

using Microsoft.AspNetCore.Components;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Net.Http;

using DSSAlternative.AHP;

namespace DSSAlternative.AppComponents
{
    public class DSS
    {
        private readonly HttpClient HttpClient;
        private readonly NavigationManager Navigation;
        public DSS(HttpClient client, NavigationManager manager)
        {
            HttpClient = client;
            Navigation = manager;
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





        public event Action<IProject> OnProjectSelectChange;
        public event Action OnTemplatesLoaded;
        public event Action OnStateLoaded;
        public event Action OnProjectRemoved;

        public IRatingSystem RatingSystem { get; private set; } = new RatingSystem();
        public List<ITemplate> Templates { get; private set; }
       
        //Все проблемы
        public List<IProject> Projects { get; private set; } = new List<IProject>();
        public IProject Project { get; private set; }


        public void SelectProject(IProject project)
        {
            if (Project != null)
            {
                Project.UpdatedHierOrRelationChanged -= Update;
            }

            Project = project;
            project.UpdatedHierOrRelationChanged += Update;
            Update();

            void Update()
            {
                OnProjectSelectChange?.Invoke(project);
            }
        }

        public void AddProject(IProject project)
        {
            Projects.Add(project);
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
                Navigation.NavigateTo("Start");
            }

            OnProjectRemoved?.Invoke();
        }

        public void LoadState(DssState state)
        {
            Projects.Clear();
            if(state.OpenedTemplates != null)
            {
                var projects = state.OpenedTemplates.Select(t => new Project(t));
                foreach (var project in projects)
                {
                    AddProject(project);
                }
                if (projects.Any())
                {
                    var selected = Project = Projects[state.SelectedTemplateIndex];
                    SelectProject(selected);
                }
                OnStateLoaded?.Invoke();
            }
        }
    }
}
