using DSSAlternative.AHP;
using DSSAlternative.AHP.HierarchyInfo;
using DSSAlternative.AHP.MatrixMethods;
using DSSAlternative.AHP.Relations;
using DSSAlternative.Web.Shared.Components;
using DSSAlternative.Web.Shared.Components.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.Web.AppComponents
{

    public static class CssExtensions
    {
        private static string EmptyCssRule = string.Empty;

        public static List<string> AddCssRuleClass(this List<string> classes, Func<bool> func, string classTrue, string classFalse = null)
        {
            return AddCssRuleClass(classes, func.Invoke(), classTrue, classFalse);
        }
        public static List<string> AddCssRuleClass(this List<string> classes, bool res, string classTrue, string classFalse = null)
        {
            if (res && !string.IsNullOrEmpty(classTrue))
            {
                classes.Add(classTrue);
            }
            else if (!res && !string.IsNullOrEmpty(classFalse))
            {
                classes.Add(classFalse);
            }
            return classes;
        }
        public static string CreateClassList(this List<string> classes)
        {
            return string.Join(' ', classes);
        }

        #region Отношения

        //Критерии
        public static CountInfo CountFilled(this IRelationsHierarchy Relations)
        {
            return Relations.FilterComparesCount(c => !c.Unknown);
        }
        public static CountInfo CountFilled(this IRelationsCriteria Relations)
        {
            return Relations.FilterComparesCount(c => !c.Unknown);
        }
        public static CountInfo CountFilled<T>(this IGrouping<T, IRelationsCriteria> criteriaGroup)
        {
            return criteriaGroup.FilterComparesCount(c => !c.Unknown);
        }


        private const string ColorOk = "lightgreen";
        private const string ColorUnknown = "#e7de79";
        private const string ColorUnconsistent = "lightpink";
        public static string CssColor(this IUsable criteria)
        {
            return !criteria.Known ? ColorUnknown : criteria.Consistent ? ColorOk : ColorUnconsistent;
        }
        public static string CssColorClass(this IUsable criteria)
        {
            return !criteria.Known ? "unknown" : criteria.Consistent ? "safe" : "dangerous";
        }

        public static string CssSelected(this IRelationsCriteria criteria, IRelationNode relation)
        {
            if(relation == null)
            {
                return string.Empty;
            }

            return relation.Main == criteria.NodeMain ? "selected-now" : "";
        }
        public static (char symbol, string tooltip) Symbol(this IRelationsCriteria criteria)
        {
            if (!criteria.Known)
            {
                return ('?', "Не все отношения известны");
            }
            if (!criteria.Consistent)
            {
                return ('X', "Внимание, отношения несогласованы!");
            }
            return ('✓', "Отлично, отношения заполнены и согласованы");

        }

        #endregion

        #region Узел иерархии

        /// <summary>
        /// CSS для расцветки узла в схеме
        /// </summary>
        public static string GetNodeCssClasses(IProject project, INode selectedNode, INode currentNode, HierarchySchemeComponent.SchemeFilters viewSelection)
        {
            if (currentNode == null)
            {
                return EmptyCssRule;
            }

            List<string> results = new List<string>();
            if (viewSelection == HierarchySchemeComponent.SchemeFilters.Selection && selectedNode != null)
            {
                bool isOwnerForSelected = selectedNode.NodeControllers().Contains(currentNode);
                bool isOwnedBySelected = selectedNode.NodesControlled().Contains(currentNode);
                bool isNeighbor = selectedNode.NodesSameGroup().Contains(currentNode);
                bool isSelfSelected = selectedNode == currentNode;

                results.AddCssRuleClass(isOwnerForSelected, "owner-node");
                results.AddCssRuleClass(isOwnedBySelected, "owned-node");
                results.AddCssRuleClass(isNeighbor && !isSelfSelected, "neighbor-node");
                results.AddCssRuleClass(isSelfSelected, "selected-node");
            }
            if (viewSelection == HierarchySchemeComponent.SchemeFilters.Best)
            {
                bool best = project.HierarchyActive.IsNodeBestAtGroup(currentNode);

                results.AddCssRuleClass(best, "best-node");
            }
            if (viewSelection == HierarchySchemeComponent.SchemeFilters.Relations)
            {
                var relations = project.Relations[currentNode];
                bool isKnownSafe = relations.Known && relations.Consistent;
                bool isUnknown = !relations.Known;
                bool isErrored = !relations.Consistent;

                results.AddCssRuleClass(isKnownSafe, "safe-node");
                results.AddCssRuleClass(isUnknown, "unknown-node");
                results.AddCssRuleClass(isErrored, "unkonsistent-node");
            }
            return results.CreateClassList();
        }

        #endregion

        public static string GetRelationNodeCssClasses(IProject project, IRelationNode relation)
        {
            bool relationsUnknown = relation.Unknown;
            bool relationConsistent = relation.CriteriaContext.Consistent;

            List<string> classes = new List<string>();
            classes.AddCssRuleClass(() => relationsUnknown, "warning");
            classes.AddCssRuleClass(() => !relationsUnknown && !relationConsistent, "error");
            classes.AddCssRuleClass(() => !relationsUnknown && relationConsistent, "safe");
            classes.AddCssRuleClass(() => project.RelationSelected == relation, "active");
            return classes.CreateClassList();
        }
        public static string GetRelationNodeCssClasses(IProject project, IRelationNode relation, IMatrix mtx)
        {
            var selected = project.RelationSelected;
            bool mirrored = selected != null && relation == selected.Mirrored;

            List<string> classes = new List<string>();
            classes.AddCssRuleClass(() => relation == selected, "cell-selected");
            classes.AddCssRuleClass(() => mirrored, "cell-mirrored");
            classes.AddCssRuleClass(() => relation.Value == 0, "unknown");
            classes.AddCssRuleClass(() => !mtx.IsCorrect, "incorrect");
            return classes.CreateClassList();
        }
    }
}
