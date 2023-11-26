namespace DSSAlternative.Web.AppComponents
{
    /// <summary>
    /// 
    /// </summary>
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
