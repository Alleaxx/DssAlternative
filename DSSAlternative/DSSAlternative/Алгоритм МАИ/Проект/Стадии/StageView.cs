using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public class StageView : Stage
    {
        public StageView(IProject problem) : base(problem)
        {

        }
        protected override void AddRules()
        {
            bool areRelationsKnown = Project.ProblemActive.CorrectnessRels.AreKnown;
            bool areRelationsConsistent = Project.ProblemActive.CorrectnessRels.AreConsistenct;

            if (!areRelationsKnown)
            {
                AddClass("warning");
            }
            if (!areRelationsConsistent)
            {
                AddClass("error");
            }
        }
    }
}
