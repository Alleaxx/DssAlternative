using DSSAlternative.AHP.Relations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP.HierarchyInfo
{
    /// <summary>
    /// Расширения к иерархии, которые анализируют её узлы
    /// </summary>
    public static class HierarchyExtensions
    {
        /// <summary>
        /// Узлы иерархии, сгруппированные по уровням
        /// </summary>
        public static IEnumerable<IGrouping<int, INode>> NodesGroupedByLevel(this IHierarchy hierarchy)
        {
            return hierarchy.Nodes.OrderBy(n => n.Level).GroupBy(h => h.Level);
        }

        /// <summary>
        /// Узлы иерархии, сгруппированные по своей категории
        /// </summary>
        public static IEnumerable<IGrouping<string, INode>> NodesGroupedByGroup(this IHierarchy hierarchy)
        {
            return hierarchy.Nodes.OrderBy(n => n.Level).GroupBy(h => h.Group);
        }


        /// <summary>
        /// Узлы между нулевым и последним уровнем иерархии (критерии)
        /// </summary>
        public static IEnumerable<INode> GetNodesCriterias(this IHierarchy hierarchy)
        {
            int maxLevel = hierarchy.Nodes.Max(n => n.Level);
            return hierarchy.Nodes.Where(h => h.Level > 0 && h.Level < maxLevel);
        }

        /// <summary>
        /// Узлы на последнем уровне иерархии (конечные альтернативы)
        /// </summary>
        public static IEnumerable<INode> GetNodesAlternatives(this IHierarchy hierarchy)
        {
            int maxLevel = hierarchy.Nodes.Max(n => n.Level);
            return hierarchy.Nodes.Where(h => h.Level == maxLevel);
        }

        /// <summary>
        /// Все указанные группы узлов в иерархии без дублей
        /// </summary>
        public static IEnumerable<string> GetGroupsOfNodes(this IHierarchy hierarchy)
        {
            return hierarchy.Nodes.Select(n => n.Group).Distinct();
        }

        /// <summary>
        /// Количество узлов в иерархии с указанным уровнем
        /// </summary>
        public static int CountNodesWithLevel(this IHierarchy hierarchy, int level)
        {
            return hierarchy.Nodes.Count(n => n.Level == level);
        }

        /// <summary>
        /// Максимальный уровень в иерархии узлов
        /// </summary>
        public static int GetMaxLevel(this IHierarchy hierarchy)
        {
            return hierarchy.Nodes.Max(n => n.Level);
        }


        /// <summary>
        /// Узлы с лучшим коэффициентом для указанной главной группы
        /// </summary>
        public static IEnumerable<INode> GetBestOfGroupOwner(this IHierarchy hierarchy, string group)
        {
            var nodes = hierarchy.Nodes.Where(n => n.GroupOwner == group);
            var maxCoeff = nodes.Max(n => n.Coefficient);
            return nodes.Where(n => n.Coefficient == maxCoeff);
        }
        
        /// <summary>
        /// Узлы с лучшим коэффициентом для указанного уровня
        /// </summary>
        public static IEnumerable<INode> BestOfLevel(this IHierarchy hierarchy, int level)
        {
            var nodes = hierarchy.Nodes.Where(n => n.Level == level);
            var maxCoeff = nodes.Max(n => n.Coefficient);
            return nodes.Where(n => n.Coefficient == maxCoeff);
        }


        /// <summary>
        /// Является ли узел лучшим в своей группе
        /// </summary>
        public static bool IsNodeBestAtGroup(this IHierarchy hierarchy, INode node)
        {
            var bestNodes = GetBestOfGroupOwner(hierarchy, node.GroupOwner);
            return bestNodes.Contains(node);
        }
    }
}
