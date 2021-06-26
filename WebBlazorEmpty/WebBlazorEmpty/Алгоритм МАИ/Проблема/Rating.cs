using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface IRating : IStyled
    {
        INode Node { get; set; }
        string Name { get; }
        double Value { get; }

        bool CheckEqual(IRating rating);
    }
    public class Rating : IRating
    {
        public INode Node { get; set; }
        private IRatingRule Rule => DSS.Ex.RatingSystem.GetRuleForRating(this);


        public double Value { get; private set; }
        public string Name => Rule.Name;


        public Rating(double val) : this(null, val) { }
        public Rating(INode node, double val)
        {
            Node = node;
            Value = val;
        }


        public bool CheckEqual(IRating rating) => rating.Node == Node && rating.Value == Value;

        public string GetClass() => "";
        public string GetStyle() => Rule.Style;
    }
}
