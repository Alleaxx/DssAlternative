using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSSAlternative.AHP;
using Microsoft.AspNetCore.Components;

namespace DSSAlternative.Web.AppComponents
{
    /// <summary>
    /// Базовый класс для любого компонента системы. Содержит основные зависимости и сокращения в использовании
    /// </summary>
    public class DSSComponent : ComponentBase
    {
        [Inject]
        public IDssProjects Dss { get; set; }
        [Inject]
        public IAccount Account { get; set; }
        [Inject]
        public IRatingSystem RatingSystem { get; set; }

        public IEnumerable<IProject> ProjectsOpened => Dss.Projects;
        public IRatingRules RatingRules { get; private set; } = RatingCssSystem.DefaultSystem;

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
