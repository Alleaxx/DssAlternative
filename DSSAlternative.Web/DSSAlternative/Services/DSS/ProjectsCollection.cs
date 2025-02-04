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
    /// <summary>
    /// Интерфейс коллекции текущих активных проектов
    /// </summary>
    public interface IProjectsCollection
    {
        /// <summary>
        /// Возникает при смене выбранного проекта или при его обновлени
        /// (изменилась редактируемая иерархия или активная, изменилось отношение)
        /// </summary>
        event Action<IProject> OnSelectedProjectChanged;

        /// <summary>
        /// Возникает при удалении или добавлении задачи в списке
        /// </summary>
        event Action OnActiveProjectsChanged;

        /// <summary>
        /// Список активных задач
        /// </summary>
        public IReadOnlyCollection<IProject> ActiveProjects { get; }

        /// <summary>
        /// Текущая выбранная задача
        /// </summary>
        public IProject SelectedProject { get; }

        /// <summary>
        /// Выбрать задачу
        /// </summary>
        void SelectProject(IProject project);

        /// <summary>
        /// Очистить выбранную задачу (поставить заглушку)
        /// </summary>
        void ClearSelectedProject();

        /// <summary>
        /// Добавить новую задачу
        /// </summary>
        void AddProject();

        /// <summary>
        /// Добавить новую задачу по выбранному проекту
        /// </summary>
        void AddProject(ITemplateProject template);

        /// <summary>
        /// Заменить текущую активную задачу проектом
        /// </summary>
        void SetProject(ITemplateProject template);

        /// <summary>
        /// Удалить задачу
        /// </summary>
        void RemoveProject(IProject project);

        /// <summary>
        /// Загрузить список задач из сохраненного состояния
        /// </summary>
        void LoadProjectsFromState(ProjectsSavedState state);
    }
    
    /// <summary>
    /// Коллекция текущих активных проектов
    /// </summary>
    public class ProjectsCollection : IProjectsCollection
    {
        public event Action<IProject> OnSelectedProjectChanged;
        public event Action OnActiveProjectsChanged;

        //Все задачи
        public IReadOnlyCollection<IProject> ActiveProjects => ActiveProjectsList;
        private List<IProject> ActiveProjectsList { get; set; }
        public IProject SelectedProject { get; private set; }


        public ProjectsCollection()
        {
            ActiveProjectsList = new List<IProject>();
            ClearSelectedProject();
        }


        //Выбор активного проекта
        public void SelectProject(IProject project)
        {
            var oldProject = SelectedProject;
            UnsubscribeProjectEvents(oldProject);
            SelectedProject = project;
            OnSelectedProjectChanged?.Invoke(project);
            if(project != null)
            {
                project.OnActiveHierChanged += ProjectSelected_OnUpdated;
                project.OnRelationChanged += ProjectSelected_OnChanged;
                project.OnEditingHierChanged += ProjectSelected_OnUpdated;
            }
        }
        private void ProjectSelected_OnChanged(IRelationsHierarchy obj)
        {
            OnSelectedProjectChanged?.Invoke(SelectedProject);
        }
        private void ProjectSelected_OnUpdated(IHierarchy obj)
        {
            OnSelectedProjectChanged?.Invoke(SelectedProject);
        }
        private void UnsubscribeProjectEvents(IProject project)
        {
            if(project == null)
            {
                return;
            }
            project.OnActiveHierChanged -= ProjectSelected_OnUpdated;
            project.OnRelationChanged -= ProjectSelected_OnChanged;
        }
        public void ClearSelectedProject()
        {
            var emptyProject = HierarchyExamples.CreateEmptyProblem();
            SelectProject(emptyProject);
        }

        //Добавляет новую задачу по шаблону
        public void AddProject(ITemplateProject template)
        {
            AddProjectToList(new Project(template.CloneThis()));
        }
        public void AddProject()
        {
            AddProjectToList(HierarchyExamples.CreateNewProblem());
        }

        //Заменяет текущую задачу
        public void SetProject(ITemplateProject template)
        {
            var oldProject = SelectedProject;

            var newProject = new Project(template.CloneThis());

            ActiveProjectsList.Remove(oldProject);
            ActiveProjectsList.Add(newProject);

            SelectProject(newProject);
        }


        private void AddProjectToList(IProject project)
        {
            ActiveProjectsList.Add(project);
            OnActiveProjectsChanged?.Invoke();
        }
        public void RemoveProject(IProject project)
        {
            int indexOfRemoved = ActiveProjectsList.IndexOf(project);

            ActiveProjectsList.Remove(project);
            bool isRemovingProjectSelected = SelectedProject == project;
            bool anotherProjectAvail = ActiveProjectsList.Count > 0;


            if (isRemovingProjectSelected && anotherProjectAvail)
            {
                int newIndex = indexOfRemoved == 0 ? 0 : indexOfRemoved - 1;
                SelectProject(ActiveProjectsList.ElementAt(newIndex));
            }
            else if (!anotherProjectAvail)
            {
                ClearSelectedProject();
            }

            OnActiveProjectsChanged?.Invoke();
        }
        public void LoadProjectsFromState(ProjectsSavedState state)
        {
            ActiveProjectsList.Clear();
            if(state.OpenedProjectTemplates == null || !state.OpenedProjectTemplates.Any())
            {
                return;
            }
            var projects = state.OpenedProjectTemplates.Select(t => new Project(t));
            foreach (var project in projects)
            {
                AddProjectToList(project);
            }
            if (ActiveProjects.Any())
            {
                var selected = state.SelectedTemplateIndex <= ActiveProjectsList.Count - 1 ? ActiveProjectsList.ElementAt(state.SelectedTemplateIndex) : ActiveProjectsList.First();
                SelectProject(selected);
            }
        }

    }
}
