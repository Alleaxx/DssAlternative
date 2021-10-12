using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface IRelationsCorrectness
    {
        bool AreCorrect { get; }
        bool AreKnown { get; }
        bool AreConsistenct { get; }

        IEnumerable<ICheck> Errors(INode node);

        bool IsNodeUnknown(INode node);
        bool IsNodeConsistenct(INode node);
    }
    public class RelationsCorrectness : IRelationsCorrectness
    {
        private IProblem Pr { get; set; }
        public RelationsCorrectness(IProblem problem)
        {
            Pr = problem;
        }



        public bool AreCorrect => AreConsistenct && AreKnown;

        public bool AreKnown => RelationsUnknown.Count() == 0;
        public bool AreConsistenct => NodesNotConsistent.Count() == 0;

        //Список критериев, матрицы по которым не согласованы
        private IEnumerable<INode> NodesNotConsistent => Pr.NodesWithRels.Where(n => !Pr.GetMtxRelations(n).Consistency.IsCorrect());
        private IEnumerable<INodeRelation> RelationsUnknown => Pr.RelationsRequired.Where(r => r.Unknown);

        public IEnumerable<ICheck> Errors(INode node)
        {
            List<ICheck> checks = new List<ICheck>();
            if (IsNodeUnknown(node))
                checks.Add(new CheckResult("Матрица не заполнена","known",false,$"Не все отношения по критерию '{node.Name}' заполнены"));
            if (!IsNodeConsistenct(node))
                checks.Add(new CheckResult("Матрица не согласована","consistent",false, $"Отношения по критерию '{node.Name}' не согласованы"));
            return checks;
        }

        public bool IsNodeUnknown(INode node)
        {
            return Pr.RelationsRequired.Where(r => r.Main == node && r.Unknown).Count() > 0;
        }
        public bool IsNodeConsistenct(INode node)
        {
            return Pr.GetMtxRelations(node).Consistency.IsCorrect();
        }

    }
}
