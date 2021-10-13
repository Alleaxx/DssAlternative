using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public class StageRelation : Stage
    {
        public readonly INodeRelation Relation;
        public StageRelation(IProject project, INodeRelation rel) : base(project)
        {
            Relation = rel;
        }

        protected override void AddRules()
        {
            bool relationsUnknown = Relation.Unknown;
            bool relationConsistent = Project.ProblemActive.GetMtxRelations(Relation.Main).Consistency.IsCorrect();

            AddClass("safe");
            if (relationsUnknown)
            {
                AddClass("warning");
            }
            if (!relationConsistent)
            {
                AddClass("error");
            }
            if(Project.RelationSelected == Relation)
            {
                AddClass("active");
            }
        }
    }
}
