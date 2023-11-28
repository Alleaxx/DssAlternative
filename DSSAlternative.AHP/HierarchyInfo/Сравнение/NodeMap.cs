using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP.HierarchyInfo
{
    /// <summary>
    /// Интерфейс сверки двух иерархий на предмет изменений
    /// </summary>
    public interface INodeMap
    {
        /// <summary>
        /// Получить отчет о разнице текущей редактируемой иерархией с прошлой утвержденной для проекта
        /// </summary>
        ComparisonHierarchy GetState(IProject project);

        /// <summary>
        /// Получить отчет о разнице с указанной иерархией
        /// </summary>
        ComparisonHierarchy GetState(IHierarchy editCurrent);

        /// <summary>
        /// Карта соответствия узлов иерархий
        /// </summary>
        Dictionary<INode, INode> ConfirmMap { get; }
    }


    /// <summary>
    /// Сверка двух иерархий на предмет изменений
    /// </summary>
    public class NodeMap : INodeMap
    {
        public Dictionary<INode, INode> ConfirmMap { get; init; }


        public NodeMap(IProject project) : this(project.HierarchyEditing, project.HierarchyActive)
        {

        }
        public NodeMap(IHierarchy hierarchyEdit, IHierarchy hierarchyActive)
        {
            ConfirmMap = new Dictionary<INode, INode>();
            UpdateMap(hierarchyEdit, hierarchyActive);
        }
        
        
        //Момент синхронизации
        private void UpdateMap(IHierarchy hierarchyEdit, IHierarchy hierarchyActive)
        {
            if(hierarchyEdit == null || hierarchyActive == null)
            {
                return;
            }
            if (hierarchyEdit.Nodes.Count() != hierarchyActive.Nodes.Count())
            {
                return;
            }

            ConfirmMap.Clear();
            for (int i = 0; i < hierarchyEdit.Nodes.Count(); i++)
            {
                var nodeEdit = hierarchyEdit.Nodes.ElementAt(i);
                var nodeActive = hierarchyActive.Nodes.ElementAt(i);
                ConfirmMap.Add(nodeEdit, nodeActive);
            }
        }
        public ComparisonHierarchy GetState(IProject project)
        {
            return GetState(project.HierarchyEditing);
        }
        public ComparisonHierarchy GetState(IHierarchy hierarchyCurrent)
        {
            var nodesCurrent = hierarchyCurrent.Nodes;
            var nodesOld = ConfirmMap.Keys;

            List<ComparisonNode> comparisons= new List<ComparisonNode>();

            foreach (var nodeOld in nodesOld)
            {
                if (!nodesCurrent.Contains(nodeOld))
                {
                    comparisons.Add(new ComparisonNode(nodeOld, CompareNodeState.Removed));
                }
                else
                {
                    comparisons.Add(new ComparisonNode(nodeOld, ConfirmMap[nodeOld]));
                }
            }
            foreach (var nodeCurrent in nodesCurrent)
            {
                if (!nodesOld.Contains(nodeCurrent))
                {
                    comparisons.Add(new ComparisonNode(nodeCurrent, CompareNodeState.Added));
                }
            }

            return new ComparisonHierarchy(comparisons);
        }
    }
}
