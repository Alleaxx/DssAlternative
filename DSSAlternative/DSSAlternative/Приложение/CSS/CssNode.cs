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
            bool isBest = project.HierarchyActive.Best(node.Level).Contains(node);
            bool isUnknown = !project.Relations[node].Known;
            bool isConsistenct = project.Relations[node].Consistent;
            bool isSelected = node == project.NodeSelected;
            bool filled = !isUnknown && isConsistenct;

            AddRuleClass("usual");
            AddRuleClass(() => viewSelection == HierShemeBig.ViewSelections.Best && isBest, "best");
            AddRuleClass(() => viewSelection == HierShemeBig.ViewSelections.Relations && isUnknown, "warn");
            AddRuleClass(() => viewSelection == HierShemeBig.ViewSelections.Relations && !isConsistenct, "bad");
            AddRuleClass(() => viewSelection == HierShemeBig.ViewSelections.Relations && filled, "good");
            AddRuleClass(() => viewSelection == HierShemeBig.ViewSelections.Selection && isSelected, "sel");
        }
        public CssNode(INode node, INode hovered)
        {
            if (hovered != null && node != null)
            {
                AddRuleClass(hovered == node, "node-own");
                var criterias = hovered.Criterias();
                AddRuleClass(criterias.Any() && hovered.Criterias().Contains(node), "node-main-element");
                var lower = hovered.Controlled();
                AddRuleClass(lower.Any() && lower.Contains(node), "node-controlled-element");
            }
        }
    }
}
