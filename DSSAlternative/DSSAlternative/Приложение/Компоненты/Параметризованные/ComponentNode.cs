using DSSAlternative.AHP;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DSSAlternative.AppComponents
{
    public class DSSComponentParamNode : DSSProject
    {
        [Parameter]
        public INode Node { get; set; }

        protected IMatrix Mtx => Relations[Node].Mtx;
    }
}
