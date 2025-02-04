using DSSAlternative.AHP.Ratings;
using DSSAlternative.AHP.Relations;
using System.Collections.Generic;

namespace DSSAlternative.Web.AppComponents
{

    public interface IRatingRules
    {
        RuleForRating GetRuleForRating(IRating rating);
        RuleForRating GetRuleForRelation(IRelationNode relation);
    }
    public class RatingCssSystem : IRatingRules
    {
        protected List<RuleForRating> Rules { get; init; }
        protected RuleForRating DefaultRule { get; set; }

        public static readonly IRatingRules DefaultSystem = new RatingCssSystem();

        public RatingCssSystem()
        {
            Rules = new List<RuleForRating>();
            CreateRules();
        }

        protected virtual void CreateRules()
        {
            DefaultRule = new RuleForRating(double.NegativeInfinity, double.PositiveInfinity, "?? отношение ??", "color: black");
            Add(new RuleForRating(0, 1, "Отношения между ними неизвестны", "color:black"));
            Add(new RuleForRating(1, 2, "Нет приоритета", "color:black"));
            Add(new RuleForRating(2, 3, "Малый приоритет", "color:green;font-size:0.9em;"));
            Add(new RuleForRating(3, 4, "Небольшой приоритет", "color:green"));
            Add(new RuleForRating(4, 5, "Некоторый приоритет", "color:#0070dd;font-size:0.9em;"));
            Add(new RuleForRating(5, 6, "Заметный приоритет", "color:#0070dd"));
            Add(new RuleForRating(6, 7, "Сильный приоритет", "color:#0070dd;font-size:0.9em;"));
            Add(new RuleForRating(7, 8, "Значительный приоритет", "color:#9345ff"));
            Add(new RuleForRating(8, 9, "Значительный приоритет", "color:#9345ff;font-size:0.9em;"));
            Add(new RuleForRating(9, 11, "Абсолютный приоритет", "color:#ff8000"));


            void Add(RuleForRating rule)
            {
                Rules.Add(rule);
            }
        }
        public RuleForRating GetRuleForRating(IRating rating)
        {
            var rule = Rules.Find(rule => ((rating.Value >= rule.FromVal && rating.Value < rule.ToVal)));
            return rule ?? DefaultRule;
        }
        public RuleForRating GetRuleForRelation(IRelationNode relation)
        {
            var rule = Rules.Find(rule => relation.Value >= rule.FromVal && relation.Value < rule.ToVal);
            return rule ?? DefaultRule;
        }
    }
    public class RatingCssSystemV2 : RatingCssSystem
    {
        protected override void CreateRules()
        {
            DefaultRule = new RuleForRating(double.NegativeInfinity, double.PositiveInfinity, "?? отношение ??", "color: black");
            Add(new RuleForRating(0, 1, "????", "color:black"));
            Add(new RuleForRating(1, 2, "Одинаковы по значимости", "color:black"));
            Add(new RuleForRating(2, 3, "Слегка приоритетнее", "color:green;font-size:0.9em;"));
            Add(new RuleForRating(3, 4, "Немного приоритетнее", "color:green"));
            Add(new RuleForRating(4, 5, "Приоритетнее", "color:#0070dd;font-size:0.9em;"));
            Add(new RuleForRating(5, 6, "Весьма приоритетнее", "color:#0070dd"));
            Add(new RuleForRating(6, 7, "Сильно приоритетнее", "color:#0070dd;font-size:0.9em;"));
            Add(new RuleForRating(7, 8, "Значительно приоритетнее", "color:#9345ff"));
            Add(new RuleForRating(8, 9, "Значительно приоритетнее", "color:#9345ff;font-size:0.9em;"));
            Add(new RuleForRating(9, 11, "Абсолютно приоритетнее", "color:#ff8000"));


            void Add(RuleForRating rule)
            {
                Rules.Add(rule);
            }
        }
    }


}
