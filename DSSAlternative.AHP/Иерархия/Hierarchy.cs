using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface IHierarchy : IEnumerable<INode>
    {
        IEnumerable<IGrouping<int, INode>> GroupedByLevel { get; }
        IEnumerable<INode> Best(int level);


        public INode MainGoal { get; }
        public IEnumerable<INode> Criterias { get; }
        public IEnumerable<INode> Alternatives { get; }
        public IEnumerable<INode> RelationNodes { get; }

        public ICorrectness Correctness { get; }

        public int LevelsCount { get; }
        public int RelationsCount { get; }
        TimeSpan CountEstTime();
        public int MaxLevel { get; }
        IEnumerable<string> ExistingGroups { get; }


        event Action OnChanged;
        void AddNode(INode node);
    }

    public class HierarchyNodes : List<INode>, IHierarchy
    {
        public override string ToString()
        {
            return $"{MainGoal.Name} [{GroupedByLevel.Count()}] ({Count})";
        }
        
        public ICorrectness Correctness { get; init; }
        public HierarchyNodes(ITemplate template) : this(template.Nodes.OfType<INode>().ToArray())
        {

        }
        public HierarchyNodes(params INode[] nodes) : this(nodes.AsEnumerable())
        {

        }
        public HierarchyNodes(IEnumerable<INode> nodes) : base(nodes)
        {
            Correctness = new HierarchyCheck(this);
            foreach (var node in this)
            {
                node.SetHierarchy(this);
                node.OnChanged += NodeUpdated;
                node.OnMoved += NodeMoved;
            }
        }


        //Изменение коллекции
        public event Action OnChanged;
        public void AddNode(INode node)
        {
            if (!Contains(node))
            {
                Add(node);
                if(node.Hierarchy != this)
                {
                    node.SetHierarchy(this);
                }
                node.OnChanged += NodeUpdated;
                node.OnMoved += NodeMoved;
                OnChanged?.Invoke();
            }
        }
        private void RemoveNode(INode node)
        {
            bool isRemoved = Remove(node);
            if (isRemoved)
            {
                node.OnChanged -= NodeUpdated;
                node.OnMoved -= NodeMoved;
                OnChanged?.Invoke();
            }
        }
        private void NodeUpdated(INode node)
        {
            OnChanged?.Invoke();
        }
        private void NodeMoved(INode node, IHierarchy newHier)
        {
            RemoveNode(node);
        }


        //Структура
        public IEnumerable<IGrouping<int, INode>> GroupedByLevel => this.OrderBy(n => n.Level).GroupBy(h => h.Level);

        public ILookup<int, INode> Dictionary => this.ToLookup(n => n.Level);
        public IEnumerable<INode> Best(int level)
        {
            double maxCoefficient = Dictionary[level].Select(c => c.Coefficient).Max();
            return Dictionary[level].Where(n => n.Coefficient == maxCoefficient);
        }

        //Общая информация об иерархии
        public int LevelsCount => this.Select(n => n.Level).Distinct().Count();
        public int RelationsCount
        {
            get
            {
                int relsAmount = 0;

                int prevAmount = 1;
                foreach (var group in GroupedByLevel)
                {
                    int groupAmount = group.Count();
                    relsAmount += prevAmount * (groupAmount * groupAmount / 3 );
                    prevAmount = groupAmount;
                }
                return relsAmount;
            }
        }
        public TimeSpan CountEstTime()
        {
            return new TimeSpan(0, 0, RelationsCount * 8);
        }
        public int MaxLevel => this.Max(s => s.Level);
        public IEnumerable<string> ExistingGroups => this.Select(n => n.Group).Distinct();

        //Узлы
        public INode MainGoal => this.FirstOrDefault(h => h.Level == 0);
        public IEnumerable<INode> Criterias => this.Where(h => h.Level > 0 && h.Level < MaxLevel);
        public IEnumerable<INode> Alternatives => this.Where(h => h.Level == MaxLevel);

        //Все кроме последнего уровня
        public IEnumerable<INode> RelationNodes => this.Where(n => n.Level != MaxLevel);

        public static bool CompareEqual(IHierarchy a, IHierarchy b)
        {
            if (a == null || b == null)
            {
                return false;
            }
            if (a.Count() != b.Count())
            {
                return false;
            }
            var dictionary = a.ToDictionary(n => n.CreateNodeId());
            foreach (var node in b)
            {
                string id = node.CreateNodeId();
                if (!dictionary.ContainsKey(id))
                {
                    return false;
                }
            }

            return true;
        }
    }

}
