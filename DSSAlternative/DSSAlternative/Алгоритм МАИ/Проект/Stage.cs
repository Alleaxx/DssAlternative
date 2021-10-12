using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface IStage : IStyled
    {

    }

    public class Stage : IStage
    {
        protected readonly IProject Project;

        public Stage(IProject project)
        {
            Project = project;
        }

        private List<string> Classes { get; set; } = new List<string>();
        public string CssClass()
        {
            Classes.Clear();
            AddRules();
            return string.Join(' ', Classes);
        }
        protected virtual void AddRules()
        {

        }
        protected void AddClass(string str)
        {
            Classes.Add(str);
        }
        public string CssStyle()
        {
            return "";
        }
    }
}
