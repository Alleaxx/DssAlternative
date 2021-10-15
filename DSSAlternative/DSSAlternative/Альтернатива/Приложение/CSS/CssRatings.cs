using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public class CssRating : CssCheck
    {
        public static IRatingRules Rules { get; set; } = new RatingCssSystem();
        public CssRating(INodeRelation relation, IRating ratingFor)
        {
            var rule = Rules.GetRuleForRating(ratingFor);
            AddRuleStyle(rule.Style);
            bool isSelected = relation.Rating.CheckEqual(ratingFor)
                || ( (relation.Value == 1 || relation.Value == 0) && relation.Value == ratingFor.Value);

            AddRuleClass(() => isSelected, "selected", "selectable");
        }
    }

    public interface IRatingRules
    {
        List<RuleForRating> Rules { get; }
        RuleForRating GetRuleForRating(IRating rating);
        RuleForRating GetRuleForRelation(INodeRelation relation);
    }
    public class RatingCssSystem : IRatingRules
    {
        public List<RuleForRating> Rules { get; set; } = new List<RuleForRating>();
        protected RuleForRating DefaultRule { get; set; }

        public RatingCssSystem()
        {
            CreateRules();
        }

        protected virtual void CreateRules()
        {
            DefaultRule = new RuleForRating(double.NegativeInfinity, double.PositiveInfinity, "?? отношение ??", "color: black");
            Add(new RuleForRating(0, 1, "????", "color:black"));
            Add(new RuleForRating(1, 2, "Одинаковый приоритет", "color:black"));
            Add(new RuleForRating(2, 3, "Незначительный", "color:green;font-size:0.9em;"));
            Add(new RuleForRating(3, 4, "Небольшой", "color:green"));
            Add(new RuleForRating(4, 5, "Обычный", "color:#0070dd;font-size:0.9em;"));
            Add(new RuleForRating(5, 6, "Заметный", "color:#0070dd"));
            Add(new RuleForRating(6, 7, "Сильный", "color:#0070dd;font-size:0.9em;"));
            Add(new RuleForRating(7, 8, "Значительный", "color:#9345ff"));
            Add(new RuleForRating(8, 9, "Значительный", "color:#9345ff;font-size:0.9em;"));
            Add(new RuleForRating(9, 11, "Абсолютный", "color:#ff8000"));


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
        public RuleForRating GetRuleForRelation(INodeRelation relation)
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


    public class RuleForRating
    {
        public double FromVal { get; set; }
        public double ToVal { get; set; }
        public string Name { get; set; }
        public string Style { get; set; }


        public RuleForRating(double from, double to, string name, string style)
        {
            FromVal = from;
            ToVal = to;
            Name = name;
            Style = style;
        }
    }
}
