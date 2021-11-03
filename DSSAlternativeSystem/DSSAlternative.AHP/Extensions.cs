using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSSAlternative.AHP;

namespace DSSAlternative.AHP
{
    public static class AhpExtensions
    {
        public static double Cr(this INode node, IProject project)
        {
            return project.Relations[node].Mtx.Cr;
        }
        public static IEnumerable<INodeRelation> OwnedRelations(this INode node, IProject project)
        {
            return project.Relations[node].Where(r => !r.Self);
        }



        public static IEnumerable<IGrouping<int, INode>> Grouped(this IProject project)
        {
            return project.HierarchyActive.GroupedByLevel;
        }
        public static IEnumerable<INode> UnknownNodes(this IProject project)
        {
            return project.Relations.Where(c => !c.Known).Select(c => c.Key);
        }
        public static IEnumerable<INode> UnconsistenctNodes(this IProject project)
        {
            return project.Relations.Where(c => !c.Consistent).Select(c => c.Key);
        }
    }
}
