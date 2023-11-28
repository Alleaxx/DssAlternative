using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


using DSSAlternative.AHP;
using DSSAlternative.AHP.Templates;

namespace DSSAlternative.Web.AppComponents
{
    /// <summary>
    /// Сохраненный список активных задач
    /// </summary>
    public class ProjectsSavedState
    {
        public DateTime DateTimeSaved { get; set; }
        public int SelectedTemplateIndex { get; set; }
        public TemplateProject[] OpenedProjectTemplates { get; set; } = Array.Empty<TemplateProject>();

        public ProjectsSavedState()
        {
            DateTimeSaved = DateTime.Now;
        }
        public ProjectsSavedState(IProjectsCollection collection)
        {
            DateTimeSaved = DateTime.Now;
            OpenedProjectTemplates = collection.ActiveProjects.Where(p => p.IsActiveHierCreated).Select(pr => new TemplateProject(pr)).ToArray();
            SelectedTemplateIndex = collection.ActiveProjects.ToList().IndexOf(collection.SelectedProject);
        }
    }
}
