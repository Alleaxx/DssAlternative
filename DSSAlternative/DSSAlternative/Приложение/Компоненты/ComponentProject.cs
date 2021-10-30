using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSSAlternative.AHP;

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


        protected IProject Project => DSSApp.Project;
        protected IHierarchy Hierarchy => Project.HierarchyActive;
        protected IRelations Relations => Project.Relations;



        public INodeRelation RelationActive => Project.RelationSelected;
        protected void SetNow(INodeRelation rel)
        {
            Project.SetNow(rel);
        }
        protected void SetNow(INode node)
        {
            Project.SetNow(node);
        }


        protected override void OnInitialized()
        {
            base.OnInitialized();
            DSSApp.OnProjectSelectChange += DSS_ProjectChanged;
        }
        private void DSS_ProjectChanged(IProject obj)
        {
            StateHasChanged();
        }
    }
}
