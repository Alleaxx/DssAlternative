using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSSAlternative.AHP;

namespace DSSAlternative.AppComponents
{

    public class CssHierarchy : CssCheck
    {
        public CssHierarchy(IProject project)
        {
            bool isEditHierCorrect = project.ProblemEditing.Correctness.IsCorrect;

            AddRuleClass("stage-menu-element");
            AddRuleClass(() => project.UnsavedChanged, "warning");
            AddRuleClass(() => !isEditHierCorrect, "error");
        }
    }
    public class CssView : CssCheck
    {
        public CssView(IProject project)
        {
            bool areRelationsKnown = project.ProblemActive.CorrectnessRels.AreKnown;
            bool areRelationsConsistent = project.ProblemActive.CorrectnessRels.AreConsistenct;

            AddRuleClass("stage-menu-element");
            AddRuleClass(() => !areRelationsKnown, "warning");
            AddRuleClass(() => !areRelationsConsistent, "error");
        }
    }
    public class CssResults : CssCheck
    {
        public CssResults(IProject project)
        {
            bool areRelationsCorrect = project.ProblemActive.CorrectnessRels.IsCorrect;

            AddRuleClass("stage-menu-element");
            AddRuleClass(() => !areRelationsCorrect, "none");
        }
    }
    public class CssRelation : CssCheck
    {
        public CssRelation(IProject project, INodeRelation relation)
        {
            bool relationsUnknown = relation.Unknown;
            bool relationConsistent = project.ProblemActive.GetMtxRelations(relation.Main).Consistency.IsCorrect();

            AddRuleClass("safe");
            AddRuleClass(() => relationsUnknown, "warning");
            AddRuleClass(() => !relationConsistent, "error");
            AddRuleClass(() => project.RelationSelected == relation, "active");
            AddRuleStyle($"margin-left:{relation.Main.Level * 2}em");
        }
        public CssRelation(IMatrix mtx, INodeRelation selected,INodeRelation relation)
        {
            bool mirrored = selected != null && relation == selected.Mirrored;
            AddRuleClass("cell-value");
            AddRuleClass(() => relation == selected, "cell-selected");
            AddRuleClass(() => mirrored, "cell-mirrored");
            AddRuleClass(() => relation.Value == 0, "unknown");
            AddRuleClass(() => !mtx.Consistency.IsCorrect(), "incorrect");
        }
    }
}
