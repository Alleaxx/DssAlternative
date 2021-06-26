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

    public class HierarchyN : IHierarchy
    {
        public override string ToString() => $"{MainGoal.Name} [{GroupedByLevel.Count()}] ({Hierarchy.Count()})";
        public HierarchyN(ITemplate template)
        {
            Hierarchy = template.Nodes;
            Hierarchy.ToList().ForEach(n => n.UpdateStructure(template.Nodes, template.Groups));
        }



        public IEnumerable<INode> Hierarchy { get; set; }
        public IEnumerable<IGrouping<int, INode>> GroupedByLevel => Hierarchy.OrderBy(n => n.Level).GroupBy(h => h.Level);
        public ICorrectness Correctness => new HierarchyCorrectness(this);


        //Всего узлов
        public int NodesCount => Hierarchy.Count();
        public int LevelsCount => GroupedByLevel.Count();
        public int RelationsCount
        {
            get
            {
                var groupedLevel = GroupedByLevel;
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

            if (aNodes.Count() != bNodes.Count())
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
