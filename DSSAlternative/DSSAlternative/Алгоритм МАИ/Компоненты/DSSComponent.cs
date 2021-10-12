using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

namespace DSSAlternative.AHP
{
    public class DSSComponent : ComponentBase
    {
        [Inject]
        public DSS DSSApp { get; set; }
    }
}
