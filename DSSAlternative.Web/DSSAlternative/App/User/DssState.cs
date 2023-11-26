using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using DSSAlternative.AHP;
using DSSAlternative.AHP.Templates;

namespace DSSAlternative.Web.AppComponents
{
    public class DssState
    {
        public override string ToString()
        {
            if (Saved == new DateTime())
            {
                return "Сохраненного состояния нет";
            }
            else
            {
                return $"Сохранено {Saved:dd MMM yyyy HH:mm}, задач: {OpenedTemplates?.Length}";
            }
        }
        public DateTime Saved { get; set; }
        public int SelectedTemplateIndex { get; set; }
        public TemplateProject[] OpenedTemplates { get; set; } = Array.Empty<TemplateProject>();

        public DssState()
        {

        }
        public DssState(IDssProjects dss)
        {
            Saved = DateTime.Now;
            OpenedTemplates = dss.Projects.Where(p => p.IsActiveHierCreated).Select(pr => new TemplateProject(pr)).ToArray();
            if (OpenedTemplates.Contains(dss.Project as TemplateProject))
            {
                SelectedTemplateIndex = dss.Projects.IndexOf(dss.Project);
            }
            else
            {
                SelectedTemplateIndex = 0;
            }
        }
    }
}
