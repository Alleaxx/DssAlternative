using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using DSSLib;

namespace DSSAHP
{
    public class AdviceSystemHierarchy : DSSLib.NotifyObj, IHierarchyAdviceSystem
    {
        public override string ToString() => "Иерархическая система задания критериев";


        public event Action<IHierarchyAdviceSystem, Problem> Finished;


        public NodeHierarcy MainNode => Nodes.Where(n => n.Level == 0).First();

        public ObservableCollection<NodeHierarcy> Nodes { get; set; } = new ObservableCollection<NodeHierarcy>();
        public CollectionViewSource NodesView { get; private set; }
        public int NodeLevelsCount => Nodes.GroupBy(n => n.Level).Count();


        public RelayCommand AddCommand { get; set; }
        public RelayCommand AddNextCommand { get; set; }
        public RelayCommand RemoveCommand { get; set; }
        public RelayCommand ClearAllCommand { get; set; }
        public RelayCommand UpdateProblemCommand { get; set; }



        private void Add(NodeHierarcy node)
        {
            Nodes.Add(node);
            Nodes.Last().PropertyChanged += AdviceSystemHierarchy_PropertyChanged; ;
            AdviceSystemHierarchy_PropertyChanged(null, null);

            if (Saved)
                Saved = false;
        }

        private void AdviceSystemHierarchy_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Saved)
                Saved = false;

            OnPropertyChanged(nameof(RelationsFillAmount));
            OnPropertyChanged(nameof(NodeLevelsCount));
        }

        private void Add(object obj)
        {
            if(int.TryParse(obj.ToString(), out int res))
            {
                if (res == 0)
                    res++;

                NodeHierarcy newNode = new NodeHierarcy($"Критерий {Nodes.Count}","Описание критерия",res);
                Add(newNode);
            }
        }
        private void AddNext(object obj)
        {
            if(int.TryParse(obj.ToString(), out int res))
            {
                Add(res + 1);
            }
        }

        private bool IsRemoveAvaillable(object obj) => obj != null && obj is NodeHierarcy node && node.Level != 0;
        private void Remove(object obj)
        {
            if(obj is NodeHierarcy node)
            {
                Nodes.Remove(node);
                node.PropertyChanged -= AdviceSystemHierarchy_PropertyChanged;
                OnPropertyChanged(nameof(RelationsFillAmount));
            }
        }

        private void ClearAllNodes(object obj)
        {
            NodeHierarcy[] nodes = Nodes.Where(n => n.Level > 0).ToArray();
            foreach (var node in nodes)
            {
                Remove(node);
            }
        }


        public int RelationsFillAmount
        {
            get
            {
                var groupedLevel = Nodes.OrderBy(n => n.Level).GroupBy(n => n.Level);
                int amount = 0;

                int prevAmount = 1;
                foreach (var group in groupedLevel)
                {
                    amount += prevAmount * (group.Count() * group.Count() / 3 );
                    prevAmount = group.Count();
                }
                return amount;
            }
        }


        public AdviceSystemHierarchy()
        {
            InitCommands();
            SetProblem(new Problem());
        }
        private void InitCommands()
        {
            AddCommand = new RelayCommand(Add, RelayCommand.IsTrue);
            AddNextCommand = new RelayCommand(AddNext, RelayCommand.IsTrue);
            RemoveCommand = new RelayCommand(Remove, IsRemoveAvaillable);
            UpdateProblemCommand = new RelayCommand(UpdateProblem, IsUpdatingProblemAvailable);
            ClearAllCommand = new RelayCommand(ClearAllNodes, RelayCommand.IsTrue);
        }

        public void SetProblem(Problem problem)
        {
            Nodes.Clear();
            AddNodesFromProblem(problem);
            CreateView();
            Saved = true;
        }
        public void ClearProblem()
        {
            SetProblem(new Problem());
        }


        private void AddNodesFromProblem(Problem problem)
        {
            foreach (var item in problem.Dictionary)
            {
                foreach (var criteria in item.Value)
                {
                    Add(new NodeHierarcy(criteria.Name, criteria.Description, criteria.Level));
                }
            }
        }
        private void CreateView()
        {
            NodesView = new CollectionViewSource();
            NodesView.Source = Nodes;
            NodesView.View.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("Level"));
        }


        public bool Saved
        {
            get => saved;
            set
            {
                saved = value;
                OnPropertyChanged();
            }
        }
        private bool saved;


        public bool IsUpdatingProblemAvail => !Saved && RelationsFillAmount > 1;
        private bool IsUpdatingProblemAvailable(object obj) => IsUpdatingProblemAvail;
        private void UpdateProblem(object obj)
        {
            Problem problem = CreateProblem();
            Saved = true;
            Finished?.Invoke(this, problem);
        }
        private Problem CreateProblem()
        {
            Problem problem = new Problem(MainNode);

            var groupedLevel = Nodes.OrderBy(n => n.Level).GroupBy(n => n.Level);
            foreach (var group in groupedLevel)
            {
                if(group.Key != 0)
                {
                    problem.AddInner(group.Select(node => new Node(problem, node)).ToArray());
                }
            }
            return problem;
        }
    }


    public class NodeHierarcy : DSSLib.Alternative
    {
        public event Action<NodeHierarcy> LevelChanged;

        public int Level
        {
            get => level;
            set
            {
                level = value;
                OnPropertyChanged();
                LevelChanged?.Invoke(this);
            }
        }
        private int level;

        public RelayCommand InsertImageCommand { get; set; }
        private bool IsInsertAvailable(object obj) => System.Windows.Clipboard.GetText().Contains("http");
        private void InsertImage(object obj)
        {
            string info = System.Windows.Clipboard.GetText();
            Image = info;
        }


        public NodeHierarcy(string name, string descr, int level)
        {
            Name = name;
            Description = descr;
            this.level = level;
            InsertImageCommand = new RelayCommand(InsertImage, IsInsertAvailable);
        }

        public bool Usual => Level != 0;
    }
}
