using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public class StageResults : Stage
    {
        public StageResults(IProject project) : base(project)
        {

        }
        protected override void AddRules()
        {
            bool areRelationsCorrect = Project.ProblemActive.CorrectnessRels.AreCorrect;

            if (!areRelationsCorrect)
            {
                AddClass("none");
            }
        }
    }
}
