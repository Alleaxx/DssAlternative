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
        private readonly NavigationManager Navigation;
        public JsonSerializerOptions JsonOptions { get; set; }
        public DSS(NavigationManager manager)
        {
            Navigation = manager;
            JsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                IgnoreNullValues = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            };
        }





        public event Action<IProject> OnProjectSelectChange;
        public event Action OnStateLoaded;
        public event Action OnProjectRemoved;

        public IRatingSystem RatingSystem { get; private set; } = new RatingSystem();
       
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
