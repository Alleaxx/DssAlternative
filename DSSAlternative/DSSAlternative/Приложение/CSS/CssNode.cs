using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSSAlternative.AHP;
using DSSAlternative.Shared.Components;

namespace DSSAlternative.AppComponents
{
    public class CssNode : CssCheck
    {
        public CssNode(IProject project, INode node, HierShemeBig.ViewSelections viewSelection)
        {
            bool isBest = project.ProblemActive.Best(node.Level).Contains(node);
            bool isUnknown = project.ProblemActive.CorrectnessRels.IsNodeUnknown(node);
            bool isConsistenct = project.ProblemActive.CorrectnessRels.IsNodeConsistenct(node);
            bool isSelected = node == project.NodeSelected;
            bool filled = !isUnknown && isConsistenct;

            AddRuleClass("usual");
            AddRuleClass(() => viewSelection == HierShemeBig.ViewSelections.Best && isBest, "best");
            AddRuleClass(() => viewSelection == HierShemeBig.ViewSelections.Relations && isUnknown, "warn");
            AddRuleClass(() => viewSelection == HierShemeBig.ViewSelections.Relations && !isConsistenct, "bad");
            AddRuleClass(() => viewSelection == HierShemeBig.ViewSelections.Relations && filled, "good");
            AddRuleClass(() => viewSelection == HierShemeBig.ViewSelections.Selection && isSelected, "good");
        }
        public CssNode(INode node, INode hovered)
        {
            if (hovered != null)
            {
                AddRuleClass(hovered == node, "node-own");
                AddRuleClass(hovered.Criterias2().Contains(node), "node-main-element");
                var lower = hovered.LowerNodesControlled();
                AddRuleClass(lower != null && lower.Contains(node), "node-controlled-element");
            }
        }
    }
}
