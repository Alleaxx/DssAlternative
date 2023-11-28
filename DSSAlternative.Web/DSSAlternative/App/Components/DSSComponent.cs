using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSSAlternative.AHP;
using DSSAlternative.AHP.Logs;
using DSSAlternative.AHP.Ratings;
using Microsoft.AspNetCore.Components;

namespace DSSAlternative.Web.AppComponents
{
    /// <summary>
    /// Базовый класс для любого компонента системы. Содержит основные зависимости и сокращения в использовании
    /// </summary>
    public class DSSComponent : ComponentBase
    {
        [Inject]
        public ILogger Logger { get; set; }
        [Inject]
        public IProjectsCollection ProjectsCollection { get; set; }
        [Inject]
        public IAccount Account { get; set; }
        [Inject]
        public IRatingSystem RatingSystem { get; set; }

        protected static string FormatNumber(double? num)
        {
            if (!num.HasValue)
            {
                return "?";
            }
            if (double.IsNaN(num.Value))
            {
                return "~";
            }
            if (double.IsInfinity(num.Value))
            {
                return "∞";
            }
            return num.Value.ToString("0.00");
        }


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (!Account.IsLoaded.HasValue)
            {
                await Account.LoadAll();
            }
        }
    }
}
