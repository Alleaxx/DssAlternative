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
    public interface IDssProjects
    {
        event Action<IProject> OnProjectSelectChange;
        event Action OnStateLoaded;
        event Action OnProjectAdded;
        event Action OnProjectRemoved;


        public List<IProject> Projects { get; }
        public IProject Project { get; }


        void SelectProject(IProject project);
        void SelectEmptyProject();
        void AddProject();
        void AddProject(ITemplate template);
        void SetProject(ITemplate template);
        void RemoveProject(IProject project);
        void LoadState(DssState state);
    }
    public class DssProjects : IDssProjects
    {
        private readonly NavigationManager Navigation;
        public DssProjects(NavigationManager manager)
        {
            Navigation = manager;
            Projects = new List<IProject>();
        }


        public event Action<IProject> OnProjectSelectChange;
        public event Action OnStateLoaded;
        public event Action OnProjectAdded;
        public event Action OnProjectRemoved;


        //Все задачи
        public List<IProject> Projects { get; private set; }
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

        //Добавляет новую задачу по шаблону
        public void AddProject(ITemplate template)
        {
            AddProject(new Project(template.CloneThis()));
        }
        public void AddProject()
        {
            AddProject(AhpHierarchy.CreateNewProblem());
        }

        //Заменяет текущую задачу
        public void SetProject(ITemplate template)
        {
            var oldProject = Project;

            var newProject = new Project(template.CloneThis());

            Projects.Remove(oldProject);
            Projects.Add(newProject);

            SelectProject(newProject);
        }


        private void AddProject(IProject project)
        {
            bool alreadyExist = Projects.Any(p => HierarchyNodes.CompareEqual(p.HierarchyActive, project.HierarchyActive));

            Projects.Add(project);
            OnProjectAdded?.Invoke();
        }
        public void RemoveProject(IProject project)
        {
            int indexOfRemoved = Projects.IndexOf(project);

            Projects.Remove(project);
            bool isRemovingProjectSelected = Project == project;
            bool anotherProjectAvail = Projects.Count > 0;


            if (isRemovingProjectSelected && anotherProjectAvail)
            {
                int newIndex = indexOfRemoved == 0 ? 0 : indexOfRemoved - 1;
                SelectProject(Projects.ElementAt(newIndex));
            }
            else if (!anotherProjectAvail)
            {
                SelectEmptyProject();
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
                    SelectProject(Projects.First());
                }
                OnStateLoaded?.Invoke();
            }
        }

        public void SelectEmptyProject()
        {
            var emptyProject = AhpHierarchy.CreateEmptyProblem();
            SelectProject(emptyProject);
        }
    }
}
