using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSSAlternative.AHP;

namespace DSSAlternative.AHP
{
    public static class AhpExtensions
    {
        public static bool IsBest(this INode node, IProject project)
        {
            return project.ProblemActive.Best(node.Level).Contains(node);
        }
        public static bool IsSelected(this INode node, IProject project)
        {
            return project.NodeSelected == node;
        }
        public static bool IsConsistenct(this INode node, IProject project)
        {
            return project.ProblemActive.CorrectnessRels.IsNodeConsistenct(node);
        }
        public static bool IsKnown(this INode node, IProject project)
        {
            return !project.ProblemActive.CorrectnessRels.IsNodeUnknown(node);
        }
        public static INodeRelation FirstRelation(INode node, IProject project)
        {
            return project.ProblemActive.FirstRequiredRelation(node);
        }
        public static double Cr(this INode node, IProject project)
        {
            return project.ProblemActive.GetMtxRelations(node).Cr;
        }
        public static IEnumerable<INodeRelation> OwnedRelations(this INode node, IProject project)
        {
            return project.ProblemActive.RelationsAll.Where(r => r.Main == node && !r.Self);
        }
        public static bool HasOwnRelations(this INode node)
        {
            return node.LowerLevel().Any() || node.Controlled().Any();
        }



        public static bool IsSelected(this INodeRelation relation, IProject project)
        {
            return project.RelationSelected == relation;
        }
        public static bool IsConsistenct(this INodeRelation relation, IProject project)
        {
            return project.ProblemActive.CorrectnessRels.IsNodeConsistenct(relation.Main);
        }
        public static INodeRelation Prev(this INodeRelation relation, IProject project)
        {
            return project.ProblemActive.PrevRequiredRel(relation);
        }
        public static INodeRelation Next(this INodeRelation relation, IProject project)
        {
            return project.ProblemActive.NextRequiredRel(relation);
        }
        public static double Cr(this INodeRelation rel, IProject project)
        {
            return project.ProblemActive.GetMtxRelations(rel.Main).Cr;
        }



        public static IEnumerable<IGrouping<int, INode>> Grouped(this IProject project)
        {
            return project.ProblemActive.GroupedByLevel;
        }
        public static IEnumerable<INode> UnknownNodes(this IProject project)
        {
            var relationsCheck = project.ProblemActive.CorrectnessRels;
            return project.ProblemActive.Where(n => relationsCheck.IsNodeUnknown(n));
        }
        public static IEnumerable<INode> UnconsistenctNodes(this IProject project)
        {
            var relationsCheck = project.ProblemActive.CorrectnessRels;
            return project.ProblemActive.RelationNodes.Where(n => !relationsCheck.IsNodeConsistenct(n));
        }
        public static bool AreRelationsCorrect(this IProject project)
        {
            return project.ProblemActive.CorrectnessRels.IsCorrect;
        }
        public static bool AreRelationsUnknown(this IProject project)
        {
            return !project.ProblemActive.CorrectnessRels.AreKnown;
        }
        public static bool AreRelationsUnconsistect(this IProject project)
        {
            return !project.ProblemActive.CorrectnessRels.AreConsistenct;
        }
    }
}
