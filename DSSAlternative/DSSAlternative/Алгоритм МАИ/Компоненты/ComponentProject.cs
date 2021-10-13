using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
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
        protected IProblem Problem => Project.ProblemActive;

        protected IStage Hier => Project.StageHier;
        protected IStage View => Project.StageView;
        protected IStage Res => Project.StageResults;


        protected ICorrectness HierEditState => Project.ProblemEditing.Correctness;
        protected IRelationsCorrectness RelationState => Project.ProblemActive.CorrectnessRels;


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
            DSSApp.OnProjectSelected += DSS_ProjectChanged;
        }
        private void DSS_ProjectChanged(IProject obj)
        {
            StateHasChanged();
        }


        protected string FormatNumber(double num)
        {
            if (double.IsNaN(num))
            {
                return "~";
            }
            if (double.IsInfinity(num))
            {
                return "∞";
            }
            return num.ToString("0.00");
        }
    }
}
