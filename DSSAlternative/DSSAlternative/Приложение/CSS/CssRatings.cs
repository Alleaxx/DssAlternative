using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSSAlternative.AHP;

namespace DSSAlternative.AppComponents
{
    public class CssRating : CssCheck
    {
        public CssRating(Dictionary<IRating, IMatrix> RatingMatrix, INodeRelation relation, IRating ratingFor, IRatingRules rules = null)
        {
            if(rules == null)
            {
                rules = RatingCssSystem.DefaultSystem;
            }
            var rule = rules.GetRuleForRating(ratingFor);
            bool isSelected = relation.Rating.CheckEqual(ratingFor)
                || ((relation.Value == 1 || relation.Value == 0) && relation.Value == ratingFor.Value);

            SetSelected();
            SetSafe();

            void SetSelected()
            {
                AddRuleStyle(() => isSelected, rule.Style, "");
                AddRuleClass(() => isSelected, "selected", "selectable");
            }
            void SetSafe()
            {
                if (RatingMatrix.ContainsKey(ratingFor))
                {
                    AddRuleClass(() => RatingMatrix[ratingFor].IsCorrect, "safe", "dangerous");
                }
                else
                {
                    AddRuleClass("usual");
                }
            }
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
            Add(new RuleForRating(0, 1, "??????", "color:black"));
            Add(new RuleForRating(1, 2, "Приоритета нет", "color:black"));
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
        public RuleForRating GetRuleForRelation(INodeRelation relation)
        {
            var rule = Rules.Find(rule => relation.Value >= rule.FromVal && relation.Value < rule.ToVal);
            return rule ?? DefaultRule;
        }

        public static readonly IRatingRules DefaultSystem = new RatingCssSystem();
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
