using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSSAlternative.AHP;
using DSSAlternative.Web.Shared.Components;

namespace DSSAlternative.Web.AppComponents
{
    public class CssNode : CssCheck
    {
        public static string GetCssClasses(IProject project, INode selectedNode, INode node, HierarchySchemeComponent.SchemeFilters viewSelection)
        {
            if (node == null)
            {
                return EmptyCssRule;
            }

            List<string> results = new List<string>();
            if(viewSelection == HierarchySchemeComponent.SchemeFilters.Selection)
            {
                results.Add(GetCssNodeOwnship(selectedNode, node));
            }
            if (viewSelection == HierarchySchemeComponent.SchemeFilters.Best)
            {
                results.Add(GetCssNodeBest(project, node));
            }
            if (viewSelection == HierarchySchemeComponent.SchemeFilters.Relations)
            {
                results.Add(GetCssNodeRelations(project, node));
            }

            return string.Join(' ', results);
        }
        private static string GetCssNodeOwnship(INode selectedNode, INode node)
        {
            if (selectedNode == null)
            {
                return EmptyCssRule;
            }

            bool isOwnerForSelected = selectedNode.NodeControllers().Contains(node);
            bool isOwnedBySelected = selectedNode.NodesControlled().Contains(node);
            bool isNeighbor = selectedNode.NodesSameGroup().Contains(node);
            bool isSelf = selectedNode == node;

            if (isOwnerForSelected)
            {
                return "owner-node";
            }
            if (isOwnedBySelected)
            {
                return "owned-node";
            }
            if (isNeighbor && selectedNode != node)
            {
                return "neighbor-node";
            }
            if (isSelf)
            {
                return "selected-node";
            }    
            return EmptyCssRule;
        }
        private static string GetCssNodeBest(IProject project, INode node)
        {
            var bestList = project.HierarchyActive.BestOfGroupOwner(node.GroupOwner);
            bool best = bestList.Contains(node);
            if (best)
            {
                return "best-node";
            }
            return EmptyCssRule;
        }
        private static string GetCssNodeRelations(IProject project, INode node)
        {
            var relations = project.Relations[node];
            bool isKnownSafe = relations.Known && relations.Consistent;
            bool isUnknown = !relations.Known;
            bool isErrored = !relations.Consistent;

            if (isKnownSafe)
            {
                return "safe-node";
            }
            else if (isUnknown)
            {
                return "unknown-node";
            }
            else if (isErrored)
            {
                return "unkonsistent-node";
            }


            return EmptyCssRule;
        }
    }
}
