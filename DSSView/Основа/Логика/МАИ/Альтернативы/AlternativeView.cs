using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    class AlternativeView : ViewElement
    {
        public override string ToString() => Alternative.ToString();

        private Problem Problem { get; set; }
        public AlternativeAHP Alternative { get; set; }
        public AlternativeView(Problem problem,AlternativeAHP alt)
        {
            Problem = problem;
            Alternative = alt;
        }

        public override IEnumerable<IViewElement> Elements => Alternative.Groups.Select(g => new AlternativeCriteriaGroupView(Problem,g));
    }
    class AlternativeCriteriaGroupView : ViewElement
    {
        public override string ToString() => $"{Group.Criteria} - {Group.Relations.Count}";

        private Problem Problem { get; set; }
        public AlternativeCriteriaGroup Group { get; set; }

        public AlternativeCriteriaGroupView(Problem problem, AlternativeCriteriaGroup g)
        {
            Problem = problem;
            Group = g;
        }

        public override IEnumerable<IViewElement> Elements => Group.Relations.Where(i => !i.Self).Select(r => new AlternativeAHPRelationView(Problem,r));
    }
    class AlternativeAHPRelationView : ViewElement
    {
        public override string ToString() => Relation.ToString();

        private Problem Problem { get; set; }
        public AlternativeAHPRelation Relation { get; set; }


        public AlternativeAHPRelationView(Problem problem,AlternativeAHPRelation relation)
        {
            Problem = problem;
            Relation = relation;
        }
    }

}
