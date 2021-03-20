using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    class CriteriaAHPView : ViewElement
    {
        public override string ToString() => Criteria.ToString();

        private Problem Problem { get; set; }
        public CriteriaAHP Criteria { get; set; }


        public CriteriaAHPView(Problem problem,CriteriaAHP criteria)
        {
            Problem = problem;
            Criteria = criteria;
        }

        public override IEnumerable<IViewElement> Elements => Criteria.Coeffs.Where(c => !c.Self).Select(cr => new CriteriaAHPRelationView(Problem, cr));
    }
    class CriteriaAHPRelationView : ViewElement
    {
        public override string ToString() => CriteriaRelation.ToString();

        private Problem Problem { get; set; }
        public CriteriaAHPRelation CriteriaRelation { get; set; }


        public CriteriaAHPRelationView(Problem problem,CriteriaAHPRelation criteria)
        {
            Problem = problem;
            CriteriaRelation = criteria;
        }
    }
}
