using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DSSView
{
    public class AdviceSystemHierarchy : NotifyObj
    {
        public event Action<Problem> Finished;


        public NodeHierarcy MainNode => Nodes.Where(n => n.Level == 0).First();

        public ObservableCollection<NodeHierarcy> Nodes { get; set; } = new ObservableCollection<NodeHierarcy>();
        public CollectionViewSource NodesView { get; private set; }


        public RelayCommand AddCommand { get; set; }
        public RelayCommand RemoveCommand { get; set; }
        public RelayCommand ClearAllCommand { get; set; }
        public RelayCommand EndCommand { get; set; }


        private void Add(object obj)
        {
            if(obj == null)
            {
                Nodes.Add(new NodeHierarcy() { Name = $"Критерий {Nodes.Count}", Level = 1, Description="Описание критерия" });
            }
            else if(obj is NodeHierarcy node)
            {
                Nodes.Add(node);
            }

            Nodes.Last().LevelChanged += Node_LevelChanged;
            Node_LevelChanged(Nodes.Last());
        }
        private void Node_LevelChanged(NodeHierarcy obj)
        {
            OnPropertyChanged(nameof(RelationsFillAmount));
        }
        private void Remove(object obj)
        {
            if(obj is NodeHierarcy node)
            {
                Nodes.Remove(node);
                node.LevelChanged -= Node_LevelChanged;
                OnPropertyChanged(nameof(RelationsFillAmount));
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
            AddDefaultNode();
            CreateView();
            InitCommands();
        }
        public AdviceSystemHierarchy(Problem problem)
        {
            AddNodesFromProblem(problem);
            CreateView();
            InitCommands();
        }


        private void AddDefaultNode()
        {
            Nodes.Add(new NodeHierarcy() { Name="Проблема", Level=0, Description = "Основная задача, которую требуется решить" });
        }
        private void AddNodesFromProblem(Problem problem)
        {
            foreach (var item in problem.Dictionary)
            {
                foreach (var criteria in item.Value)
                {
                    Add(new NodeHierarcy() { Name = criteria.Name, Level = criteria.Level, Description = criteria.Description });
                }
            }
        }


        private void CreateView()
        {
            NodesView = new System.Windows.Data.CollectionViewSource();
            NodesView.Source = Nodes;
            NodesView.View.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("Level"));
        }
        private void InitCommands()
        {
            AddCommand = new RelayCommand(Add,obj => true);
            RemoveCommand = new RelayCommand(Remove, obj => obj != null && obj is NodeHierarcy node && node.Level != 0);
            EndCommand = new RelayCommand(Finish, obj => RelationsFillAmount > 1);
        }


        private void Finish(object obj)
        {
            Problem Problem = new Problem(MainNode);

            var groupedLevel = Nodes.OrderBy(n => n.Level).GroupBy(n => n.Level);
            foreach (var group in groupedLevel)
            {
                if(group.Key != 0)
                {
                    Problem.AddInner(group.Select(node => new Node(Problem, node)).ToArray());
                }
            }

            Finished?.Invoke(Problem);
        }
    }


    public class NodeHierarcy : NotifyObj
    {
        public event Action<NodeHierarcy> LevelChanged;

        public string Name { get; set; }
        public string Description { get; set; }
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
    }
}
