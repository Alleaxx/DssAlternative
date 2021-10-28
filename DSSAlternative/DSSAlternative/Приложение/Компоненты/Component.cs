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
        public DSS DSSApp { get; set; }
        [Inject]
        public Account Account { get; set; }

        public IEnumerable<IProject> Projects => DSSApp.Projects;
        public IEnumerable<ITemplate> Templates => DSSApp.Templates;
        public IRatingSystem RatingSystem => DSSApp.RatingSystem;
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
