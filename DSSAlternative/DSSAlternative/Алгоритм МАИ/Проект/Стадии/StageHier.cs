using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public class StageHierarchy : Stage
    {
        public StageHierarchy(IProject project) : base(project)
        {

        }

        protected override void AddRules()
        {
            bool unsaved = Project.UnsavedChanged;
            if (unsaved)
            {
                AddClass("warning");
            }

            bool isCorrectHierarchy = Project.ProblemEditing.Correctness.Result;
            if (!isCorrectHierarchy)
            {
                AddClass("error");
            }
        }
    }
}
