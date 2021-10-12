using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface IRatingRule
    {
        double FromVal { get; set; }
        double ToVal { get; set; }
        string Name { get; set; }
        string Style { get; set; }
    }

    public class RatingRule : IRatingRule
    {
        public double FromVal { get; set; }
        public double ToVal { get; set; }
        public string Name { get; set; }
        public string Style { get; set; }


        public RatingRule(double from, double to, string name, string style)
        {
            FromVal = from;
            ToVal = to;
            Name = name;
            Style = style;
        }
    }
}
