using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using DSSLib;

namespace DSSAHP
{
    public abstract class AdviceSystemRelations<T> : DSSLib.NotifyObj, IAdviceSystem
    {
        public override string ToString() => "Система сравнения критериев";

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


        public T[] NodesRelations { get; protected set; }
        public CollectionViewSource NodesRelationsView { get; protected set; }

        public T Selected
        {
            get => selected;
            set
            {
                selected = value;
                OnPropertyChanged();
            }
        }
        private T selected;

        public AdviceSystemRelations()
        {
            NextCommand = new RelayCommand(MoveNext, IsNextAvailable);
            PrevCommand = new RelayCommand(MovePrevious, IsPrevAvailable);
        }

        public RelayCommand NextCommand { get; set; }
        public RelayCommand PrevCommand { get; set; }

        private bool IsNextAvailable(object obj)
        {
            return NodesRelations.ToList().IndexOf(Selected) < NodesRelations.Length - 1;
        }
        private bool IsPrevAvailable(object obj)
        {
            return NodesRelations.ToList().IndexOf(Selected) != 0;
        }

        private void MoveNext(object obj)
        {
            Selected = NodesRelations[NodesRelations.ToList().IndexOf(Selected) + 1];
        }
        private void MovePrevious(object obj)
        {
            Selected = NodesRelations[NodesRelations.ToList().IndexOf(Selected) - 1];
        }


        public void SetProblem(Problem newProblem)
        {
            if(newProblem != Problem)
            {
                //if(Problem != null)
                //    TransferMatrixFromOld(Problem, newProblem);
                Problem = newProblem;

                RecreateRelations(newProblem);
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

        protected abstract void RecreateRelations(Problem problem);
    }



    public class AdviceSystemRelationNode : AdviceSystemRelations<NodeFiller>, IAdviceSystem
    {
        public override string ToString() => "Система единовременного попарного сравнения критериев";

        protected override void RecreateRelations(Problem problem)
        {
            NodesRelations = problem.AllCriterias.Where(crit => crit.Inner.Count > 0).Select(criteria => new NodeFiller(criteria)).ToArray();

            NodesRelationsView = new CollectionViewSource();
            NodesRelationsView.Source = NodesRelations;
            NodesRelationsView.View.GroupDescriptions.Add(new PropertyGroupDescription("Node.Level"));

            OnPropertyChanged(nameof(NodesRelations));
            OnPropertyChanged(nameof(NodesRelationsView));


            if (NodesRelations.Length > 0)
                Selected = NodesRelations.First();
        }
    }
    public class NodeFiller
    {
        public Node Node { get; set; }
        public RelationFiller[] Fillers { get; set; }

        public RelayCommand ClearAllCommand { get; set; }
        private void ClearAll(object obj)
        {
            foreach (var item in Fillers)
            {
                item.ClearInfoCommand.Execute(null);
            }
        }

        public NodeFiller(Node node)
        {
            Node = node;
            Fillers = node.GetReqRelationsThis().Select(rel => new RelationFiller(rel)).ToArray();
            ClearAllCommand = new RelayCommand(ClearAll, obj => true);
        }
    }


    public class AdviceSystemRelation : AdviceSystemRelations<RelationFiller>, IAdviceSystem
    {
        public override string ToString() => "Система последовательного попарного сравнения критериев";

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

        protected override void RecreateRelations(Problem problem)
        {
            NodesRelations = problem.GetReqRelationsInner().Select(rel => new RelationFiller(rel)).ToArray();

            NodesRelationsView = new CollectionViewSource();
            NodesRelationsView.Source = NodesRelations;
            NodesRelationsView.View.GroupDescriptions.Add(new PropertyGroupDescription("Relation.Main.Name"));

            OnPropertyChanged(nameof(NodesRelations));
            OnPropertyChanged(nameof(NodesRelationsView));


            if (NodesRelations.Length > 0)
                Selected = NodesRelations.First();
        }
    }
    public class RelationFiller : DSSLib.NotifyObj
    {
        public override string ToString() => $"Выбор по {Relation}";

        public event Action<RelationFiller> Canceled;


        public NodeRelation Relation { get; set; }
        private double OriginalValue { get; set; }


        public double Value
        {
            get => value;
            set
            {
                this.value = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Results));
                UpdateRelation();
            }
        }
        private double value = 1;
        public string Results => NodeRelation.GetTextRelationFor(Value);


        public Node Best
        {
            get => best;
            set
            {
                best = value;
                OnPropertyChanged();
            }
        }
        private Node best;

        public Node Worst
        {
            get => worst;
            set
            {
                worst = value;
                OnPropertyChanged();
            }
        }
        private Node worst;


        public static double RelationMod = 1;

        private void UpdateRelation()
        {
            if (Relation.From == Best)
                Relation.Value = Value * RelationMod;
            else
                Relation.Mirrored.Value = Value * RelationMod;
        }


        public RelayCommand ToggleCommand { get; private set; }
        private void Toggle(object obj)
        {
            Node oldWorst = worst;
            Worst = Best;
            Best = oldWorst;
            UpdateRelation();
        }
        public RelayCommand ClearInfoCommand { get; private set; }
        private void ClearInfo(object obj)
        {
            Value = 1;
            Relation.Value = OriginalValue;

            Canceled?.Invoke(this);
        }



        public RelationFiller(NodeRelation relation)
        {
            Relation = relation;
            OriginalValue = Relation.Value;
            best = Relation.From;
            worst = Relation.To;

            ToggleCommand = new RelayCommand(Toggle, RelayCommand.IsTrue);
            ClearInfoCommand = new RelayCommand(ClearInfo, RelayCommand.IsTrue);

        }
    }
}
