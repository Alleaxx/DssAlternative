using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface IRating
    {
        INode Node { get; set; }
        double Value { get; }

        bool CheckEqual(IRating rating);
    }
    public class Rating : IRating
    {
        public INode Node { get; set; }
        public double Value { get; private set; }

        public Rating(double val) : this(null, val) { }
        public Rating(INode node, double val)
        {
            Node = node;
            Value = val;
        }

        public bool CheckEqual(IRating rating)
        {
            return rating.Node == Node && rating.Value == Value;
        }
    }
}
