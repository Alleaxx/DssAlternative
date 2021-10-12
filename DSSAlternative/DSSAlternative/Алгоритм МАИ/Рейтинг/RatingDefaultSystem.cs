using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface IRatingSystem
    {
        int Amount { get; set; }

        string Name { get; }

        List<IRatingRule> Rules { get; }

        IRating RatingNone { get; }
        IRating RatingEqual { get; }

        IEnumerable<(IRating from, IRating to)> RatingsFor(INodeRelation relation);

        IRatingRule GetRuleForRating(IRating rating);
        IRatingRule GetRuleForRelation(INodeRelation relation);
    }

    public class RatingDefaultSystem : IRatingSystem
    {
        public string Name { get; private set; }

        public List<IRatingRule> Rules { get; set; } = new List<IRatingRule>();
        private IRatingRule DefaultRule { get; set; }


        public int Amount
        {
            get => amount;
            set
            {
                amount = value;
                Turn = (End - Start) / (double)amount;
            }
        }
        private int amount;
        private double Start { get; set; }
        private double Turn { get; set; }
        private double End { get; set; }


        public RatingDefaultSystem(double start = 3, double turn = 2, double end = 9)
        {
            Name = "Стандартная система рейтинга";
            Start = start;
            Turn = turn;
            End = end;
            amount = (int)((End - start) / Turn);
            CreateRules();
        }
        protected virtual void CreateRules()
        {
            DefaultRule = new RatingRule(double.NegativeInfinity, double.PositiveInfinity, "?? отношение ??", "color: black");
            Add(new RatingRule(0, 1, "Неизвестное отношение","color:black"));
            Add(new RatingRule(1, 2, "Одинаковы по значимости", "color:black"));
            Add(new RatingRule(2, 3, "Слегка приоритетнее", "color:green;font-size:0.9em;"));
            Add(new RatingRule(3, 4, "Немного приоритетнее", "color:green"));
            Add(new RatingRule(4, 5, "Приоритетнее", "color:#0070dd;font-size:0.9em;"));
            Add(new RatingRule(5, 6, "Весьма приоритетнее", "color:#0070dd"));
            Add(new RatingRule(6, 7, "Сильно приоритетнее", "color:#0070dd;font-size:0.9em;"));
            Add(new RatingRule(7, 8, "Значительно приоритетнее", "color:#9345ff"));
            Add(new RatingRule(8, 9, "Значительно приоритетнее", "color:#9345ff;font-size:0.9em;"));
            Add(new RatingRule(9, 11, "Абсолютно приоритетнее", "color:#ff8000"));


            void Add(IRatingRule rule)
            {
                Rules.Add(rule);
            }
        }


        public IRating RatingNone => new Rating(0);
        public IRating RatingEqual => new Rating(1);

        private IEnumerable<IRating> Ratings()
        {
            List<IRating> ratings = new List<IRating>();
            for (double i = Start; i <= End; i += Turn)
            {
                ratings.Add(new Rating(i));
            }
            return ratings;
        }
        public IEnumerable<(IRating from, IRating to)> RatingsFor(INodeRelation relation)
        {
            var rows = new List<(IRating from, IRating to)>();
            foreach (var rating in Ratings())
            {
                IRating ratingFor = new Rating(relation.From, rating.Value);
                IRating ratingTo = new Rating(relation.To, rating.Value);
                rows.Add((ratingFor, ratingTo));
            }
            return rows;
        }

        public IRatingRule GetRuleForRating(IRating rating)
        {
            var rule = Rules.Find(rule => ((rating.Value >= rule.FromVal && rating.Value < rule.ToVal) ));
            return rule ?? DefaultRule;
        }

        public IRatingRule GetRuleForRelation(INodeRelation relation)
        {
            var rule = Rules.Find(rule => relation.Value >= rule.FromVal && relation.Value < rule.ToVal);
            return rule ?? DefaultRule;
        }
    }
}
