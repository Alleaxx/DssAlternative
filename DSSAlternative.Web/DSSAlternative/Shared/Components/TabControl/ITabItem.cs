using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.Web.Shared.Components.TabControl
{
    public interface ITabItem
    {
        public RenderFragment ChildContent { get; set; }
    }
}
