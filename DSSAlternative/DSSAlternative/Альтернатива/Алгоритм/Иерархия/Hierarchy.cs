using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface IHierarchy
    {
        IEnumerable<INode> Hierarchy { get; }
        IEnumerable<IGrouping<int, INode>> GroupedByLevel { get; }
        ILookup<int, INode> Dictionary { get; }
        IEnumerable<INode> Best(int level);


        public INode MainGoal { get; }
        public IEnumerable<INode> Criterias { get; }
        public IEnumerable<INode> Alternatives { get; }
        public IEnumerable<INode> NodesWithRels { get; }


        public ICorrectness Correctness { get; }


        public int NodesCount { get; }
        public int LevelsCount { get; }
        public int RelationsCount { get; }
        TimeSpan EstTime { get; }
        public int MaxLevel { get; }
    }

    public class HierarchySheme : IHierarchy
    {
        public override string ToString()
        {
            return $"{MainGoal.Name} [{GroupedByLevel.Count()}] ({Hierarchy.Count()})";
        }
        public HierarchySheme(ITemplate template)
        {
            Hierarchy = template.Nodes.OfType<INode>().ToArray();
            foreach (var node in Hierarchy)
            {
                node.UpdateStructure(Hierarchy, GetGroups(Hierarchy));
            }
            Correctness = new HierarchyCheck(this);
            Dictionary = Hierarchy.ToLookup(n => n.Level);


            IEnumerable<NodeGroup> GetGroups(IEnumerable<INode> Nodes)
            {
                var groupedNodes = Nodes.GroupBy(n => n.Group).OrderBy(g => g.Key);
                var groupIndexes = groupedNodes.Select(g => g.Key);

                List<NodeGroup> groups = new List<NodeGroup>();
                foreach (var index in groupIndexes)
                {
                    groups.Add(new NodeGroup(index, Nodes.Where(n => n.Group == index).ToArray()));
                }
                return groups;
            }
        }


        //Структура
        public IEnumerable<INode> Hierarchy { get; private set; }
        public IEnumerable<IGrouping<int, INode>> GroupedByLevel => Hierarchy.OrderBy(n => n.Level).GroupBy(h => h.Level);
        public ICorrectness Correctness { get; private set; }

        public ILookup<int, INode> Dictionary { get; set; }
        public IEnumerable<INode> Best(int level)
        {
            double maxCoefficient = Dictionary[level].Select(c => c.Coefficient).Max();
            return Dictionary[level].Where(n => n.Coefficient == maxCoefficient);
        }



        //Всего узлов
        public int NodesCount => Hierarchy.Count();
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
        public int MaxLevel => Hierarchy.Max(s => s.Level);


        //Узлы
        public INode MainGoal => Hierarchy.FirstOrDefault(h => h.Level == 0);
        public IEnumerable<INode> Criterias => Hierarchy.Where(h => h.Level > 0 && h.Level < LevelsCount - 1);
        public IEnumerable<INode> Alternatives => Hierarchy.Where(h => h.Level == LevelsCount - 1);
        public IEnumerable<INode> NodesWithRels => Criterias.Prepend(MainGoal);
        

        public static bool CompareEqual(IHierarchy a, IHierarchy b)
        {
            if (a == null || b == null)
            {
                return false;
            }

            var aNodes = a.Hierarchy.ToList();
            var bNodes = b.Hierarchy.ToList();

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
