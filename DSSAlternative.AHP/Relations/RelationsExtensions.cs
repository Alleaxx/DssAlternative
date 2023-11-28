using DSSAlternative.AHP.HierarchyInfo;
using DSSAlternative.AHP.MatrixMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP.Relations
{
    public static class RelationsExtensions
    {
        #region Отношения иерархии

        /// <summary>
        /// Получить список всех конкретных сравнений узлов в отношениях
        /// </summary>
        public static IEnumerable<IRelationNode> GetAllNodeCompares(this IRelationsHierarchy context)
        {
            return context.RelationsCriteria.SelectMany(c => c.NodeCompares);
        }
        /// <summary>
        /// Получить минимально возможный список конкретных сравнений узлов в отношениях, необходимых для заполнения
        /// </summary>
        public static IEnumerable<IRelationNode> GetAllNodeComparesMini(this IRelationsHierarchy context)
        {
            return context.RelationsCriteria.SelectMany(c => c.NodeComparesMini);
        }

        public static IEnumerable<IRelationNode> GetAllNodeComparesMiniOrdered(this IRelationsHierarchy context)
        {
            List<IRelationNode> nodes = new List<IRelationNode>();

            var criteriaRelations = context.GetActiveNodeCriterias().GroupBy(r => r.NodeMain.Group);
            foreach (var group in criteriaRelations)
            {
                nodes.AddRange(group.SelectMany(g => g.NodeComparesMini));
            }

            return nodes;
        }
        /// <summary>
        /// Получить список узлов-критериев, по которым отношения неизвестны
        /// </summary>
        public static IEnumerable<IRelationsCriteria> GetUnknownNodeCriterias(this IRelationsHierarchy context)
        {
            return context.RelationsCriteria.Where(c => !c.Known);
        }
        /// <summary>
        /// Получить список узлов-критериев, по которым отношения несогласованы
        /// </summary>
        public static IEnumerable<IRelationsCriteria> GetUnconsistenctNodeCriterias(this IRelationsHierarchy context)
        {
            return context.RelationsCriteria.Where(c => !c.Consistent);
        }

        /// <summary>
        /// Получить список узлов-критериев, которые содержат отношения
        /// </summary>
        public static IEnumerable<IRelationsCriteria> GetActiveNodeCriterias(this IRelationsHierarchy context)
        {
            return context.RelationsCriteria.Where(c => c.HasChildCompares());
        }

        public static IRelationNode NextRequiredRel(this IRelationsHierarchy context,IRelationNode from)
        {
            var allRelations = context.GetAllNodeComparesMiniOrdered().ToList();
            int index = allRelations.IndexOf(from);
            return index < allRelations.Count - 1 ? allRelations[index + 1] : allRelations[0];
        }
        public static IRelationNode PrevRequiredRel(this IRelationsHierarchy context, IRelationNode from)
        {
            var allRelations = context.GetAllNodeComparesMiniOrdered().ToList();
            int index = allRelations.IndexOf(from);
            return index > 0 ? allRelations[index - 1] : allRelations[allRelations.Count - 1];
        }


        #endregion

        #region Отношения критерия

        /// <summary>
        /// Критерий содержит отношения
        /// </summary>
        public static bool HasChildCompares(this IRelationsCriteria relationsCriteria)
        {
            return relationsCriteria.NodeComparesMini.Any();
        }

        /// <summary>
        /// Первое отношение-сравнение, которые считается необходимым для заполнения
        /// </summary>
        public static IRelationNode FirstNodeCompareRequired(this IRelationsCriteria relationsCriteria)
        {
            return relationsCriteria.NodeComparesMini.FirstOrDefault();
        }

        public static CountInfo FilterComparesCount(this IRelationsHierarchy context, Func<IRelationNode, bool> predicate)
        {
            return new CountInfo(context.GetAllNodeComparesMini().Count(predicate), context.GetAllNodeComparesMini().Count());
        }
        public static CountInfo FilterComparesCount(this IEnumerable<IRelationsCriteria> context, Func<IRelationNode, bool> predicate)
        {
            return new CountInfo(context.SelectMany(c => c.NodeComparesMini).Count(predicate), context.SelectMany(c => c.NodeComparesMini).Count());
        }
        public static CountInfo FilterComparesCount(this IRelationsCriteria context, Func<IRelationNode, bool> predicate)
        {
            return new CountInfo(context.NodeComparesMini.Count(predicate), context.NodeComparesMini.Count());
        }

        /// <summary>
        /// В отношениях по критерию осталось последнее незаполненное отношение
        /// </summary>
        public static bool WithLastUnknownRelation(this IRelationsCriteria criteria)
        {
            return criteria.NodeComparesMini.Count(n => n.Unknown) == 1;
        }

        /// <summary>
        /// Получить матрицу с заполненным отношением сравнения
        /// </summary>
        public static IMatrix GetMatrixForRating(this IRelationNode relationNode, double value)
        {
            IMatrix source = relationNode.CriteriaContext.Mtx;
            source.Change(relationNode.From, relationNode.To, value);
            return source;
        }

        #endregion
    }
}
