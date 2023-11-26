using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP.HierarchyInfo
{
    /// <summary>
    /// Расширения для узла, у которого указана иерархия
    /// </summary>
    public static class NodeExtensionsRelations
    {

        /// <summary>
        /// Является ли данный узел критерием для других узлов
        /// </summary>
        public static bool HasInnerRelations(this INode node)
        {
            return node.Relations.NodeCompares.Any();
        }

        /// <summary>
        /// Получить показатель согласованности матрицы узла (если он является критерием)
        /// </summary>
        public static double? GetCr(this INode node)
        {
            return node?.Relations?.Mtx.Cr;
        }
    }
}
