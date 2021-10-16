using DSSAlternative.AHP;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSSAlternative.AHP;

namespace DSSAlternative.AppComponents
{
    public class DSSComponentParamNode : DSSProject
    {
        [Parameter]
        public INode Node { get; set; }

        protected IMatrix Mtx => Problem.GetMtxRelations(Node);
    }
}
