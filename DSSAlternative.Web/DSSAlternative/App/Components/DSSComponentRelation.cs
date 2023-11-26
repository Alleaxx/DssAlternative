using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using DSSAlternative.AHP;
using DSSAlternative.AHP.Relations;

namespace DSSAlternative.Web.AppComponents
{
    /// <summary>
    /// Базовый класс компонента для элемента, который отображает отношение
    /// </summary>
    public class DSSComponentRelationV2 : DSSComponentProjectV2
    {
        public IRelationNode Relation => RelationParam ?? Project.RelationSelected;
        [Parameter]
        public IRelationNode RelationParam { get; set; }

        //Так как Project устанавливается от каскадного параметра, то
        //каждый раз при его установке мы должны заново подписываться на событие
        //если подписываться в Initialized, то при смене параметра на событие в новом проекте подписки не произойдет
        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            Project.OnSelectedRelationChanged += StateHasChanged;
        }
    }
}
