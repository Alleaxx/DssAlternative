using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class AdviceSystem : NotifyObj
    {
        public override string ToString() => $"Система заполнения проблемы {Problem.Name}";

        public Problem Problem { get; private set; }

        public ISystemRelations RelationsSystem { get; set; }
        public AdviceSystemHierarchy HierarchySystem { get; set; }
        public AdviceSystemResults ResultsSystem { get; set; }


        public AdviceSystem() : this(new Problem())
        {

        }
        public AdviceSystem(Problem problem)
        {
            Problem = problem;
            HierarchySystem = new AdviceSystemHierarchy(problem);

            RelationsSystem = new AdviceSystemRelations();
            RelationsSystem = new SingleChoiceSystemRelations();

            RelationsSystem.SetProblem(problem);

            ResultsSystem = new AdviceSystemResults();
            ResultsSystem.SetProblem(problem);

            HierarchySystem.Finished += HierarchySystem_Finished;
        }

        private void HierarchySystem_Finished(Problem obj)
        {
            RelationsSystem.SetProblem(obj);
            ResultsSystem.SetProblem(obj);
        }
    }

}
