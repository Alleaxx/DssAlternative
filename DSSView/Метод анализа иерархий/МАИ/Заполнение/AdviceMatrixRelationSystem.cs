using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DSSView
{

    public class AdviceSystemRelationsMatrix : DSSLib.NotifyObj, IAdviceSystem
    {
        public override string ToString() => "Система матричного попарного сравнения критериев";



        public Node[] Criterias { get; private set; }
        public CollectionViewSource CriteriasView { get; private set; }


        public Problem Problem
        {
            get => problem;
            set
            {
                problem = value;
                OnPropertyChanged();
            }
        }
        private Problem problem;


        public AdviceSystemRelationsMatrix()
        {

        }

        public void SetProblem(Problem newProblem)
        {
            if(newProblem != Problem)
            {
                if(Problem != null)
                    TransferMatrixFromOld(Problem, newProblem);
                Problem = newProblem;

                RecreateCriterias(newProblem);
            }
        }
        public void ClearProblem()
        {
            SetProblem(new Problem());
        }


        private void TransferMatrixFromOld(Problem oldProblem, Problem problem)
        {
            var oldRelations = oldProblem.FillRelationsAll;
            var newRelations = problem.FillRelationsAll;
            foreach (var newRelation in newRelations)
            {
                if(oldRelations.Find(oldRel =>
                    oldRel.Main.Name == newRelation.Main.Name &&
                    oldRel.From.Name == newRelation.From.Name &&
                    oldRel.To.Name == oldRel.To.Name) is NodeRelation rel)
                {
                    newRelation.Value = rel.Value;
                }
            }
        }


        private void RecreateCriterias(Problem problem)
        {
            Criterias = problem.AllCriterias.Where(crit => crit.Inner.Count > 1).ToArray();

            CriteriasView = new CollectionViewSource();
            CriteriasView.Source = Criterias;
            CriteriasView.View.GroupDescriptions.Add(new PropertyGroupDescription("Level"));

            OnPropertyChanged(nameof(Criterias));
            OnPropertyChanged(nameof(CriteriasView));
        }
    }
}
