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
        private readonly IRatingSystem RatingSystem = new RatingDefaultSystem();
        public INode Node { get; set; }
        private IRatingRule Rule => RatingSystem.GetRuleForRating(this);


        public double Value { get; private set; }
        public string Name => Rule.Name;


        public Rating(double val) : this(null, val) { }
        public Rating(INode node, double val, IRatingSystem system = null)
        {
            Node = node;
            Value = val;

            if(system != null)
            {
                RatingSystem = system;
            }
        }


        public bool CheckEqual(IRating rating) => rating.Node == Node && rating.Value == Value;

        public string CssClass() => "";
        public string CssStyle() => Rule.Style;
    }
}
