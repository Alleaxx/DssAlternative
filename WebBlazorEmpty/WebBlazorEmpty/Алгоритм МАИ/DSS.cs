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

        public void SelectProblem(IProject project)
        {
            IProject old = Project;
            Project = project;
            ProjectChanged?.Invoke(project);

            if(old != null)
            {
                old.Updated -= Update;
            }
            project.Updated += Update;

            void Update()
            {
                ProjectChanged?.Invoke(project);
            }
        }

        public void AddProblem(ITemplate template) => AddProblem(new Project(template));
        public void AddProblem(IProject project)
        {
            Projects.Add(project);
            Project = Projects.Last();
        }

        public JsonSerializerOptions JsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            IgnoreNullValues = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
        };
    }
}
