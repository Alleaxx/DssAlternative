using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public interface ITreeDecision
    {
        event Action<ITreeDecision> Updated;

        ITreeDecision Link { get; set; }

        ITreeDecision[] Decisions { get; }

        void AddBranch(ITreeDecision item);
        void RemoveBranch(ITreeDecision item);

        void Update();


        double MX { get; }
        double Profit { get; }
        double Chance { get; }


        double ProfitStored { get; }
        double ChanceStored { get; }
        double MXStored { get; }
    }

    public abstract class TreeDecision : NotifyObj, ITreeDecision
    {
        public ITreeDecision Link { get; set; }

        public event Action<ITreeDecision> Updated;

        public string Name { get; set; }
        public string Description { get; set; }



        public virtual double MX { get; private set; }
        public virtual double Chance { get; set; } = 1;
        public double Profit
        {
            get => profit;
            set
            {
                profit = value;
                Update();
            }
        }
        private double profit;



        public double ProfitStored => Link != null ? Profit + Link.ProfitStored : Profit;
        public double ChanceStored => Link != null ? Chance * Link.ChanceStored : Chance;
        public double MXStored => ProfitStored * ChanceStored;





        private ObservableCollection<ITreeDecision> cases;
        public ReadOnlyObservableCollection<ITreeDecision> Cases { get; set; }
        public ITreeDecision[] Decisions => Cases.ToArray();


        public void AddBranch(ITreeDecision newCase)
        {
            cases.Add(newCase);
            Update();
            newCase.Link = this;
            newCase.Updated += UpdateFrom;
        }
        public void RemoveBranch(ITreeDecision existCase)
        {
            cases.Remove(existCase);
            Update();
            existCase.Link = null;
            existCase.Updated -= UpdateFrom;
        }



        public RelayCommand AddBranchCommand { get; set; }
        public RelayCommand RemoveBranchCommand { get; set; }
        public RelayCommand ClearBranchesCommand { get; set; }


        private void AddBranchExe(object obj)
        {
            if (obj.ToString() == "case")
                AddBranch(new CaseTree("Исход", 0, 0));
            else if (obj.ToString() == "alt")
                AddBranch(new AlternativeTree("Выбор"));
        }
        private void RemoveBranchExe(object obj)
        {
            Link.RemoveBranch(obj as ITreeDecision);
        }
        private void ClearBranchesExe(object obj)
        {
            while (cases.Count > 0)
            {
                cases.RemoveAt(cases.Count - 1);
            }
        }


        public TreeDecision() : this("") { }
        public TreeDecision(string name)
        {
            Name = name;
            cases = new ObservableCollection<ITreeDecision>();
            Cases = new ReadOnlyObservableCollection<ITreeDecision>(cases);

            AddBranchCommand = new RelayCommand(AddBranchExe, obj => true);
            RemoveBranchCommand = new RelayCommand(RemoveBranchExe, obj => obj != null && Link != null);
            ClearBranchesCommand = new RelayCommand(ClearBranchesExe, obj => Cases.Count > 0);
        }


        protected void UpdateFrom(ITreeDecision dec)
        {
            Update();
        }
        public virtual void Update()
        {
            MX = GetMXFor(this);
            OnPropertyChanged(nameof(MX));
            OnPropertyChanged(nameof(MXStored));
            OnPropertyChanged(nameof(Profit));
            OnPropertyChanged(nameof(ProfitStored));
            OnPropertyChanged(nameof(Chance));
            OnPropertyChanged(nameof(ChanceStored));
            Updated?.Invoke(this);
        }

        private static double GetMXFor(ITreeDecision item)
        {
            double res = 0;

            IEnumerable<ITreeDecision> collection = EndBranchesFor(item);
            foreach (var dec in collection)
            {
                res += dec.MXStored;
            }

            return res;
        }
        private static IEnumerable<ITreeDecision> EndBranchesFor(ITreeDecision item)
        {
            List<ITreeDecision> decs = new List<ITreeDecision>();
            if (item.Decisions.Length == 0)
                return new ITreeDecision[] { item };

            foreach (var dec in item.Decisions)
            {
                decs.AddRange(EndBranchesFor(dec));
            }
            return decs;
        }
    }



    //Решающий узел
    public class AlternativeTree : TreeDecision
    {
        public AlternativeTree(string name, double profit = 0) : base(name)
        {
            Profit = profit;
            Update();
        }
    }

    //Вероятностный узел
    public class CaseTree : TreeDecision
    {
        public override double Chance
        {
            get => chance;
            set
            {
                chance = value;
                Update();
            }
        }
        private double chance;


        public CaseTree(string name, double chance, double profit) : base(name)
        {
            Chance = chance;
            Profit = profit;
            Update();
        }
    }
}
