using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSSAlternative.AHP;
using DSSAlternative.AHP.HierarchyInfo;
using DSSAlternative.AHP.Relations;
using DSSAlternative.Web.Shared.Pages;
using Microsoft.AspNetCore.Components;

namespace DSSAlternative.Web.Shared
{
    /// <summary>
    /// Базовый класс компонента для элемента, который отображает задачу
    /// </summary>
    public class DSSComponentProject : DSSComponent, IAsyncDisposable
    {
        [Inject]
        public NavigationManager Navigation { get; set; }

        protected virtual IProject Project => ProjectsCollection.SelectedProject;
        protected IHierarchy HierarchyActive => Project.HierarchyActive;
        protected IHierarchy HierarchyEditing => Project.HierarchyEditing;
        protected IRelationsHierarchy Relations => Project.Relations;

        [CascadingParameter(Name = "Project")]
        protected IProject ProjectCascading { get; set; }

        public IRelationNode RelationSelected => Project.RelationSelected;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            ProjectsCollection.OnSelectedProjectChanged += DSS_ProjectChanged;
        }
        public virtual async ValueTask DisposeAsync()
        {
            ProjectsCollection.OnSelectedProjectChanged -= DSS_ProjectChanged;
        }

        protected void SelectRelation(IRelationNode rel)
        {
            if (rel == null)
            {
                return;
            }
            Project.SelectNodeRelation(rel);
        }
        protected void SelectNode(INode node)
        {
            if (node == null)
            {
                return;
            }
            Project.SelectNode(node);
        }
        private void DSS_ProjectChanged(IProject obj)
        {
            StateHasChanged();
            if (!obj.IsActiveHierCreated && Navigation.Uri != LinksEnum.Hierarchy)
            {
                Navigation.NavigateTo(LinksEnum.Hierarchy);
            }
        }


    }
    public class DSSComponentProjectV2 : DSSComponentProject
    {
        protected override IProject Project => ProjectCascading;

    }
}
