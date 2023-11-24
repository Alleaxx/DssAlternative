using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSSAlternative.AHP;
using Microsoft.AspNetCore.Components;

namespace DSSAlternative.Web.AppComponents
{
    /// <summary>
    /// Базовый класс компонента для элемента, который отображает задачу
    /// </summary>
    public class DSSComponentProject : DSSComponent
    {
        protected const string HomeLink = "home";
        protected const string HierLink = "hierarchy";
        protected const string RelationsLink = "relation-edit";
        protected const string ViewLink = "view";
        protected const string ResultsLink = "results";
        protected const string NodeLink = "node";


        protected IProject Project => Dss.Project;
        protected IHierarchy HierarchyActive => Project.HierarchyActive;
        protected IHierarchy HierarchyEditing => Project.HierarchyEditing;
        protected IRelations Relations => Project.Relations;

        [Inject]
        public NavigationManager Navigation { get; set; }


        public INodeRelation RelationSelected => Project.RelationSelected;
        protected void SelectRelation(INodeRelation rel)
        {
            if(rel == null)
            {
                return;
            }
            Project.SelectNodeRelation(rel);
        }
        protected void SelectNode(INode node)
        {
            if(node == null)
            {
                return;
            }
            Project.SelectNode(node);
        }


        protected override void OnInitialized()
        {
            base.OnInitialized();
            Dss.OnSelectedProjectChanged += DSS_ProjectChanged;
        }
        private void DSS_ProjectChanged(IProject obj)
        {
            StateHasChanged();
            if (!obj.IsActiveHierCreated && Navigation.Uri != HierLink)
            {
                Navigation.NavigateTo(HierLink);
            }
        }
    }
}
