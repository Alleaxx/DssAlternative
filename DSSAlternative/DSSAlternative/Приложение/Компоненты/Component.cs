using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSSAlternative.AHP;
using Microsoft.AspNetCore.Components;

namespace DSSAlternative.AppComponents
{
    public class DSSComponent : ComponentBase
    {
        [Inject]
        public IDssProjects Dss { get; set; }
        [Inject]
        public IAccount Account { get; set; }
        [Inject]
        public IRatingSystem RatingSystem { get; set; }

        public IEnumerable<IProject> Projects => Dss.Projects;
        public IRatingRules RatingRules { get; set; } = RatingCssSystem.DefaultSystem;

        protected const string OptionsLink = "Options";
        protected static string FormatNumber(double num)
        {
            if (double.IsNaN(num))
            {
                return "~";
            }
            if (double.IsInfinity(num))
            {
                return "∞";
            }
            return num.ToString("0.00");
        }
    }
}
