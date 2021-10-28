using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface IRelationsCorrectness : ICorrectness
    {
        bool AreKnown { get; }
        bool AreConsistenct { get; }

        bool IsNodeUnknown(INode node);
        bool IsNodeConsistenct(INode node);
    }
    public class RelationsCheck : Correctness, IRelationsCorrectness
    {
        private IProblem Problem { get; set; }
        public RelationsCheck(IProblem problem)
        {
            Problem = problem;
        }
        protected override void OwnCheck()
        {
            base.OwnCheck();
            if (!AreConsistenct)
            {
                AddFail("Неизвестность", "Отношения не заполнены");
            }
            if (!AreKnown)
            {
                AddFail("Несогласованность", "Некоторые отношения неизвестны");
            }
        }

        public bool AreKnown => !RelationsUnknown().Any();
        public bool AreConsistenct => !NodesNotConsistent().Any();

        private IEnumerable<INode> NodesNotConsistent()
        {
            return Problem.NodesWithRels.Where(node => !Problem.GetMtxRelations(node).IsCorrect);
        }
        private IEnumerable<INodeRelation> RelationsUnknown()
        {
            return Problem.RelationsRequired.Where(r => r.Unknown);
        }

        public bool IsNodeUnknown(INode node)
        {
            return Problem.RelationsRequired.Where(r => r.Main == node && r.Unknown).Any();
        }
        public bool IsNodeConsistenct(INode node)
        {
            return Problem.GetMtxRelations(node).IsCorrect;
        }
    }
}
