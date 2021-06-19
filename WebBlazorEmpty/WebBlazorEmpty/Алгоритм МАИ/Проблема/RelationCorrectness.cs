using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBlazorEmpty.AHP
{
    public interface IRelationsCorrecntess
    {
        bool AreRelationsCorrect { get; }
        bool AreRelationsKnown { get; }
        bool AreRelationsConsistenct { get; }

        IEnumerable<ICheck> Errors(INode node);

        bool CheckNodeUnknown(INode node);
        bool CheckNodeConsistenct(INode node);
    }
    public class RelationsCorrectness : IRelationsCorrecntess
    {
        private IProblem Pr { get; set; }
        public RelationsCorrectness(IProblem problem)
        {
            Pr = problem;
        }



        public bool AreRelationsCorrect => AreRelationsConsistenct && AreRelationsKnown;

        public bool AreRelationsKnown => RelationsUnknown.Count() == 0;
        public bool AreRelationsConsistenct => NodesNotConsistent.Count() == 0;

        //Список критериев, матрицы по которым не согласованы
        private IEnumerable<INode> NodesNotConsistent => Pr.NodesWithRels.Where(n => !Pr.GetMatrix(n).Consistency.IsCorrect());
        private IEnumerable<INodeRelation> RelationsUnknown => Pr.RelationsRequired.Where(r => r.Unknown);

        public IEnumerable<ICheck> Errors(INode node)
        {
            List<ICheck> checks = new List<ICheck>();
            if (CheckNodeUnknown(node))
                checks.Add(new CheckResult("Известность","known",false,$"Не все отношения по критерию '{node.Name}' заполнены"));
            if (CheckNodeConsistenct(node))
                checks.Add(new CheckResult("Согласованность","consistent",false, $"Отношения по критерию '{node.Name}' не согласованы"));
            return checks;
        }

        public bool CheckNodeUnknown(INode node)
        {
            return Pr.RelationsRequired.Where(r => r.Main == node && r.Unknown).Count() > 0;
        }
        public bool CheckNodeConsistenct(INode node)
        {
            return Pr.GetMatrix(node).Consistency.IsCorrect();
        }

    }
}
