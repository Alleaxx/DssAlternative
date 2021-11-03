using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSSAlternative.AHP;
using Microsoft.AspNetCore.Components;

namespace DSSAlternative.AppComponents
{
    public class DSSProject : DSSComponent
    {
        protected const string HomeLink = "home";
        protected const string HierLink = "hierarchy";
        protected const string RelationsLink = "relation-edit";
        protected const string ViewLink = "view";
        protected const string ResultsLink = "results";
        protected const string NodeLink = "node";


        protected IProject Project => Dss.Project;
        protected IHierarchy Hierarchy => Project.HierarchyActive;
        protected IRelations Relations => Project.Relations;

        [Inject]
        public NavigationManager Navigation { get; set; }


        public INodeRelation RelationActive => Project.RelationSelected;
        protected void SetNow(INodeRelation rel)
        {
            if(rel != null)
            {
                Project.SetNow(rel);
            }
        }
        protected void SetNow(INode node)
        {
            if(node != null)
            {
                Project.SetNow(node);
            }
        }


        protected override void OnInitialized()
        {
            base.OnInitialized();
            Dss.OnProjectSelectChange += DSS_ProjectChanged;
        }
        private void DSS_ProjectChanged(IProject obj)
        {
            StateHasChanged();
        }
    }
}
