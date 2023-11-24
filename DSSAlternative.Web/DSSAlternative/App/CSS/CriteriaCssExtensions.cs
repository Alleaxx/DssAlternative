using DSSAlternative.AHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.Web.AppComponents
{

    public static class CriteriaExtensions
    {
        public static double NowProgress(this IRelations Relations)
        {
            return Relations.SelectMany(c => c.Required).Where(r => !r.Unknown && !r.Self).Count();
        }
        public static double AllProgress(this IRelations Relations)
        {
            var rels = Relations.SelectMany(c => c.Required).Where(r => !r.Self).Count();
            return Math.Max(1, rels);
        }

        public static int ProgressNow(this ICriteriaRelation criteria)
        {
            return criteria.Required.Where(r => !r.Unknown).Count();
        }
        public static int ProgressNow<T>(this IGrouping<T, ICriteriaRelation> criteriaGroup)
        {
            return criteriaGroup.SelectMany(criteria => criteria.Required.Where(r => !r.Unknown)).Count();
        }
        public static int ProgressMax(this ICriteriaRelation criteria)
        {
            return Math.Max(1, criteria.Required.Where(r => !r.Self).Count());
        }
        public static int ProgressMax<T>(this IGrouping<T, ICriteriaRelation> criteriaGroup)
        {
            return Math.Max(1, criteriaGroup.SelectMany(criteria => criteria.Required.Where(r => !r.Self)).Count());
        }
        public static double Progress(this ICriteriaRelation criteria)
        {
            return Math.Round((double)criteria.ProgressNow() / criteria.ProgressMax() * 100.0);
        }
        public static double Progress<T>(this IGrouping<T, ICriteriaRelation> criteriaGroup)
        {
            return Math.Round((double)criteriaGroup.ProgressNow() / criteriaGroup.ProgressMax() * 100.0);
        }


        private const string OkColor = "lightgreen";
        private const string UnknownColor = "#e7de79";
        private const string UnconsistentColor = "lightpink";
        public static string CssColor(this ICriteriaRelation criteria)
        {
            return !criteria.Known ? UnknownColor : criteria.Consistent ? OkColor : UnconsistentColor;
        }
        public static string CssColorClass(this ICriteriaRelation criteria)
        {
            return !criteria.Known ? "unknown" : criteria.Consistent ? "safe" : "dangerous";
        }
        public static string CssColor(this IRelations Relations)
        {
            return !Relations.Known ? UnknownColor : Relations.Consistent ? OkColor : UnconsistentColor;
        }

        public static string CssSelected(this ICriteriaRelation criteria, INodeRelation relation)
        {
            if(relation == null)
            {
                return string.Empty;
            }

            return relation.Main == criteria.Key ? "selected-now" : "";
        }
        public static char Symbol(this ICriteriaRelation criteria)
        {
            if (!criteria.Known)
            {
                return '~';
            }
            if (!criteria.Consistent)
            {
                return 'X';
            }
            return '✓';

        }
        public static string SymbolTooltip(this ICriteriaRelation criteria)
        {
            if (!criteria.Known)
            {
                return "Не все отношения известны";
            }
            if (!criteria.Consistent)
            {
                return "Внимание, отношения несогласованы!";
            }
            return "Отлично, отношения заполнены и согласованы";

        }
    }
}
