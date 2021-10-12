using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public class DSSComponentRelation : DSSProject
    {
        [Parameter]
        public INodeRelation Relation { get; set; }
    }
}
