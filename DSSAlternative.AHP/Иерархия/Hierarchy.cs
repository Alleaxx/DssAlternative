using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface IHierarchy : IEnumerable<INode>
    {
        IEnumerable<IGrouping<int, INode>> GroupedByLevel { get; }
        IEnumerable<IGrouping<int, INode>> GroupedByGroup { get; }
        ILookup<int, INode> Dictionary { get; }
        IEnumerable<INode> Best(int level);


        public INode MainGoal { get; }
        public IEnumerable<INode> Criterias { get; }
        public IEnumerable<INode> Alternatives { get; }
        public IEnumerable<INode> NodesWithRels { get; }

        public ICorrectness Correctness { get; }


        public int LevelsCount { get; }
        public int RelationsCount { get; }
        TimeSpan EstTime { get; }
        public int MaxLevel { get; }


        event Action OnChanged;
        void AddNode(INode node);
        void RemoveNode(INode node);
    }

    public class HierarchyNodes : List<INode>, IHierarchy
    {
        public override string ToString()
        {
            return $"{MainGoal.Name} [{GroupedByLevel.Count()}] ({Count})";
        }
        public HierarchyNodes(ITemplate template) : base(template.Nodes.OfType<INode>().ToArray())
        {
            foreach (var node in this)
            {
                node.SetHierarchy(this);
            }
        }
        public event Action OnChanged;
        public void AddNode(INode node)
        {
            if (!Contains(node))
            {
                Add(node);
                OnChanged?.Invoke();
            }
        }
        public void RemoveNode(INode node)
        {
            bool isRemoved = Remove(node);
            if (isRemoved)
            {
                OnChanged?.Invoke();
            }
        }



        //Структура
        public IEnumerable<IGrouping<int, INode>> GroupedByLevel => this.OrderBy(n => n.Level).GroupBy(h => h.Level);
        public IEnumerable<IGrouping<int, INode>> GroupedByGroup => this.OrderBy(n => n.Group).GroupBy(h => h.Group);
        public ICorrectness Correctness => new HierarchyCheck(this);

        public ILookup<int, INode> Dictionary => this.ToLookup(n => n.Level);
        public IEnumerable<INode> Best(int level)
        {
            double maxCoefficient = Dictionary[level].Select(c => c.Coefficient).Max();
            return Dictionary[level].Where(n => n.Coefficient == maxCoefficient);
        }

        //Всего узлов
        public int LevelsCount => Dictionary.Count;
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
        public TimeSpan EstTime => new TimeSpan(0, 0, RelationsCount * 8);
        public int MaxLevel => this.Max(s => s.Level);


        //Узлы
        public INode MainGoal => this.FirstOrDefault(h => h.Level == 0);
        public IEnumerable<INode> Criterias => this.Where(h => h.Level > 0 && h.Level < LevelsCount - 1);
        public IEnumerable<INode> Alternatives => this.Where(h => h.Level == LevelsCount - 1);
        public IEnumerable<INode> NodesWithRels => Criterias.Prepend(MainGoal);
        

        public static bool CompareEqual(IHierarchy a, IHierarchy b)
        {
            if (a == null || b == null)
            {
                return false;
            }

            var aNodes = a.ToList();
            var bNodes = b.ToList();

            int aCount = aNodes.Count;
            int bCount = bNodes.Count;

            if (aCount != bCount)
            {
                return false;
            }

            string aString = string.Join(';', aNodes);
            string bString = string.Join(';', bNodes);
            if (!aString.Equals(bString))
            {
                return false;
            }

            return true;
        }
    }

}
