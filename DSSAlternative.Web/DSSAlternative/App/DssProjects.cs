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
using DSSAlternative.AHP.HierarchyInfo;
using DSSAlternative.AHP.Templates;
using DSSAlternative.AHP.Relations;

namespace DSSAlternative.Web.AppComponents
{
    public interface IDssProjects
    {
        /// <summary>
        /// Событие при смене выбранного проекта
        /// </summary>
        event Action<IProject> OnSelectedProjectChanged;

        /// <summary>
        /// Событие при обновлении активной иерархии выбранного проекта и при изменении отношений
        /// </summary>
        event Action<IProject> OnSelectedProjectActiveHierChanged;

        /// <summary>
        /// Событие при изменении редактируемой иерархии выбранного проекта
        /// </summary>
        event Action<IProject> OnSelectedProjectEditingHierChanged;

        event Action OnStateLoaded;
        event Action OnProjectAdded;
        event Action OnProjectRemoved;


        public List<IProject> Projects { get; }
        public IProject Project { get; }


        void SelectProject(IProject project);
        void SelectEmptyProject();
        void AddProject();
        void AddProject(ITemplateProject template);
        void SetProject(ITemplateProject template);
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

        public event Action<IProject> OnSelectedProjectChanged;
        public event Action<IProject> OnSelectedProjectActiveHierChanged;
        public event Action<IProject> OnSelectedProjectEditingHierChanged;

        public event Action OnStateLoaded;
        public event Action OnProjectAdded;
        public event Action OnProjectRemoved;


        //Все задачи
        public List<IProject> Projects { get; private set; }
        public IProject Project { get; private set; }

        //Выбор активного проекта
        public void SelectProject(IProject project)
        {
            var oldProject = Project;
            DeselectProject(oldProject);
            Project = project;
            OnSelectedProjectChanged?.Invoke(project);
            if(project != null)
            {
                project.OnActiveHierChanged += ProjectSelected_OnActiveHierChanged;
                project.OnRelationChanged += ProjectSelected_OnRelationChanged;
                project.OnEditingHierChanged += ProjectSelected_OnEditingHierChanged;
            }
        }
        private void ProjectSelected_OnRelationChanged(IRelationsHierarchy obj)
        {
            OnSelectedProjectActiveHierChanged?.Invoke(Project);
        }
        private void ProjectSelected_OnActiveHierChanged(IHierarchy obj)
        {
            OnSelectedProjectActiveHierChanged?.Invoke(Project);
        }
        private void ProjectSelected_OnEditingHierChanged(IHierarchy obj)
        {
            OnSelectedProjectEditingHierChanged?.Invoke(Project);
        }
        private void DeselectProject(IProject project)
        {
            if(project == null)
            {
                return;
            }
            project.OnActiveHierChanged -= ProjectSelected_OnActiveHierChanged;
            project.OnRelationChanged -= ProjectSelected_OnRelationChanged;
        }


        //Добавляет новую задачу по шаблону
        public void AddProject(ITemplateProject template)
        {
            AddProject(new Project(template.CloneThis()));
        }
        public void AddProject()
        {
            AddProject(HierarchyExamples.CreateNewProblem());
        }

        //Заменяет текущую задачу
        public void SetProject(ITemplateProject template)
        {
            var oldProject = Project;

            var newProject = new Project(template.CloneThis());

            Projects.Remove(oldProject);
            Projects.Add(newProject);

            SelectProject(newProject);
        }


        private void AddProject(IProject project)
        {
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
            var emptyProject = HierarchyExamples.CreateEmptyProblem();
            SelectProject(emptyProject);
        }
    }
}
