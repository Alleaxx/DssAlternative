using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface IHierarchy
    {
        IEnumerable<INode> Hierarchy { get; set; }
        IEnumerable<IGrouping<int, INode>> GroupedByLevel { get; }
        Dictionary<int, INode[]> Dictionary { get; }
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
        public override string ToString() => $"{MainGoal.Name} [{GroupedByLevel.Count()}] ({Hierarchy.Count()})";
        public HierarchySheme(ITemplate template)
        {
            Hierarchy = template.Nodes.ToArray();
            foreach (var node in Hierarchy)
            {
                node.UpdateStructure(template.Nodes, template.Groups);
            }
            Dictionary = GetDictionary();
        }


        //Структура
        public IEnumerable<INode> Hierarchy { get; set; }
        public IEnumerable<IGrouping<int, INode>> GroupedByLevel => Hierarchy.OrderBy(n => n.Level).GroupBy(h => h.Level);
        public ICorrectness Correctness => new HierarchyCorrectness(this);
        public Dictionary<int, INode[]> Dictionary { get; private set; }
        private Dictionary<int, INode[]> GetDictionary()
        {
            Dictionary<int, INode[]> dictionary = new Dictionary<int, INode[]>();
            foreach (var nodeGroup in GroupedByLevel)
            {
                int level = nodeGroup.Key;
                INode[] nodes = nodeGroup.ToArray();
                dictionary.Add(level, nodes);
            }
            return dictionary;
        }
        public IEnumerable<INode> Best(int level)
        {
            double maxCoefficient = Dictionary[level].Select(c => c.Coefficient).Max();
            return Dictionary[level].Where(n => n.Coefficient == maxCoefficient);
        }



        //Всего узлов
        public int NodesCount => Hierarchy.Count();
        public int LevelsCount => GroupedByLevel.Count();
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
        public int MaxLevel => Hierarchy.Select(s => s.Level).Max();


        //Узлы
        public INode MainGoal => Hierarchy.Where(h => h.Level == 0).FirstOrDefault();
        public IEnumerable<INode> Criterias => Hierarchy.Where(h => h.Level > 0 && h.Level < LevelsCount - 1);
        public IEnumerable<INode> Alternatives => Hierarchy.Where(h => h.Level == LevelsCount - 1);
        public IEnumerable<INode> NodesWithRels => Criterias.Prepend(MainGoal);
        

        public static bool CompareEqual(IHierarchy a, IHierarchy b)
        {
            if (a == null || b == null)
                return false;

            var aNodes = a.Hierarchy.ToList();
            var bNodes = b.Hierarchy.ToList();

            int aCount = aNodes.Count;
            int bCount = bNodes.Count;

            if (aCount != bCount)
                return false;

            foreach (var aNode in a.Hierarchy)
            {
                if (!bNodes.Exists(n => n.Name == aNode.Name && n.Level == aNode.Level))
                    return false;
            }
            return true;
        }
    }

}
