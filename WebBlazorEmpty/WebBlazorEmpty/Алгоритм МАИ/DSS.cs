using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

using Microsoft.AspNetCore.Components;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace DSSAlternative.AHP
{
    public class DSS
    {
        static DSS()
        {
            Ex = new DSS();
        }


        public static DSS Ex { get; private set; }

        public event Action<IProject> ProjectChanged;

        public List<ITemplate> Templates { get; set; }
        //Все проблемы
        public List<IProject> Projects { get; private set; } = new List<IProject>();

        //Выбранная проблема
        public IProject Project { get; private set; }

        public IRatingSystem RatingSystem { get; private set; } = new RatingDefaultSystem();

        public void SelectProblem(IProject project)
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
                ProjectChanged?.Invoke(project);
            }
        }

        public void AddProject(IProject project)
        {
            Projects.Add(project);
            Project = project;
        }

        public void RemoveProject(IProject project)
        {
            Projects.Remove(project);
            bool anotherProjectAvail = Projects.Count() > 0;

            if (anotherProjectAvail)
                SelectProblem(Projects.First());
            else
                Project = null;
        }

        public JsonSerializerOptions JsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            IgnoreNullValues = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
        };
    }
}
