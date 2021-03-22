using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DSSView
{
    public interface ISystemRelations
    {
        void SetProblem(Problem problem);
    }
    public class AdviceSystemRelations : NotifyObj, ISystemRelations
    {
        public event Action<AdviceSystemRelations> Finished;


        public Node[] Criterias { get; private set; }
        public CollectionViewSource CriteriasView { get; private set; }

        public RelationFiller[] Relations { get; private set; }
        public CollectionViewSource RelationsView { get; private set; }


        public int Progress => Relations.Where(r => r.Filled).Count();

        public RelationFiller Selected
        {
            get => selected;
            set
            {
                selected = value;
                OnPropertyChanged();
            }
        }
        private RelationFiller selected;

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


        public RelayCommand FinishedCommand { get; set; }
        private void Finish(object obj)
        {
            Finished?.Invoke(this);
        }

        public AdviceSystemRelations()
        {
            FinishedCommand = new RelayCommand(Finish, obj => true);
        }

        public void SetProblem(Problem newProblem)
        {
            if(Problem != null)
                TransferMatrixFromOld(Problem, newProblem);
            Problem = newProblem;
            RecreateCriterias(newProblem);
            RecreateRelations(newProblem);
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
        private void RecreateRelations(Problem problem)
        {
            Relations = problem.GetReqRelationsInner().Select(rel => new RelationFiller(rel)).ToArray();

            RelationsView = new CollectionViewSource();
            RelationsView.Source = Relations;
            RelationsView.View.GroupDescriptions.Add(new PropertyGroupDescription("Relation.Main.Level"));

            OnPropertyChanged(nameof(Relations));
            OnPropertyChanged(nameof(RelationsView));

            foreach (var rel in Relations)
            {
                rel.Finished += Relation_Finished;
                rel.Canceled += Rel_Canceled;
            }

            if (Relations.Length > 0)
                Selected = Relations.First();
        }


        private void Rel_Canceled(RelationFiller obj)
        {
            OnPropertyChanged(nameof(Progress));
        }
        private void Relation_Finished(RelationFiller obj)
        {
            int pos = Relations.ToList().IndexOf(obj);
            if (pos < Relations.Length - 1)
                Selected = Relations[pos + 1];
            OnPropertyChanged(nameof(Progress));

            if (Relations.Length == Progress)
                Finished?.Invoke(this);
        }
    }
    public class RelationFiller : NotifyObj
    {
        public override string ToString() => $"Выбор по {Relation}";


        public event Action<RelationFiller> Finished;
        public event Action<RelationFiller> Canceled;


        public NodeRelation Relation { get; set; }
        private double SourceValue { get; set; }


        public MatrixConsistenct Consistenct => Relation.Main.Matrix.Consistency;


        public int Value
        {
            get => value;
            set
            {
                this.value = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Results));
                OnPropertyChanged(nameof(FromValue));
                OnPropertyChanged(nameof(ToValue));
            }
        }
        private int value = 0;
        private int RealValue => Math.Abs(Value) + 1;

        public Node Best => Value < 0 ? Relation.From : Relation.To;
        public double FromValue => Value < 0 ? RealValue : 1;
        public Node Worst => Value < 0 ? Relation.To : Relation.From;
        public double ToValue => Value > 0 ? RealValue : 1;
        public string Results
        {
            get
            {
                switch (RealValue)
                {
                    case 1:
                        return $"'{Best.Name}' и '{Worst.Name}' равнозначны";
                    case 2:
                        return $"'{Best.Name}' и '{Worst.Name}' почти равнозначны";
                    case 3:
                        return $"'{Best.Name}' немного преобладает над '{Worst.Name}'";
                    case 4:
                        return $"'{Best.Name}' немного преобладает над '{Worst.Name}'";
                    case 5:
                        return $"'{Best.Name}' преобладает над '{Worst.Name}'";
                    case 6:
                        return $"'{Best.Name}' преобладает над '{Worst.Name}'";
                    case 7:
                        return $"'{Best.Name}' сильно преобладает над '{Worst.Name}'";
                    case 8:
                        return $"'{Best.Name}' сильно преобладает над '{Worst.Name}'";
                    case 9:
                        return $"'{Best.Name}' абсолютно превосходит '{Worst.Name}'";
                    default:
                        return "Неизвестное отношение";
                }
            }
        }



        public bool Filled
        {
            get => filled;
            set
            {
                filled = value;
                OnPropertyChanged();
            }
        }
        private bool filled;

        public RelayCommand FinishCommand { get; private set; }
        private void Finish(object obj)
        {
            if (Value < 0)
                Relation.Value = RealValue;
            else if (Value > 0)
                Relation.Mirrored.Value = RealValue;
            else
                Relation.Value = RealValue;

            Filled = true;
            OnPropertyChanged(nameof(Consistenct));
            Finished?.Invoke(this);
        }

        public RelayCommand CancelFinishCommand { get; private set; }
        private void CancelFinish(object obj)
        {
            Relation.Value = SourceValue;
            Filled = false;
            OnPropertyChanged(nameof(Consistenct));
            Canceled?.Invoke(this);
        }




        public RelationFiller(NodeRelation relation)
        {
            Relation = relation;
            SourceValue = relation.Value;
            FinishCommand = new RelayCommand(Finish, obj => true);
            CancelFinishCommand = new RelayCommand(CancelFinish, obj => Filled);
        }
    }
}
