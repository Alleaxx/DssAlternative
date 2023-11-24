using DSSAlternative.AHP;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DSSAlternative.Web.AppComponents
{
    /// <summary>
    /// Базовый класс компонента для элемента, который отображает конкретный узел
    /// </summary>
    public class DSSComponentParamNode : DSSComponentProject
    {
        [Parameter]
        public INode Node { get; set; }

        protected IMatrix Mtx => Relations[Node].Mtx;
    }
}
