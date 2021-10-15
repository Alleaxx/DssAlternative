using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface IRatingSystem
    {
        int Amount { get; set; }

        IEnumerable<(IRating from, IRating to)> RatingsFor(INodeRelation relation);
    }

    public class RatingSystem : IRatingSystem
    {
        public override string ToString()
        {
            return "Стандартная система рейтинга";
        }

        public int Amount
        {
            get => amount;
            set
            {
                amount = value;
                Turn = (End - Start) / amount;
            }
        }
        private int amount;
        private double Start { get; set; }
        private double Turn { get; set; }
        private double End { get; set; }


        public RatingSystem(double start = 1, double turn = 2, double end = 9)
        {
            Start = start;
            Turn = turn;
            End = end;
            amount = (int)((End - start) / Turn);
        }


        private IEnumerable<IRating> Ratings()
        {
            List<IRating> ratings = new List<IRating>(10);
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
    }
}
