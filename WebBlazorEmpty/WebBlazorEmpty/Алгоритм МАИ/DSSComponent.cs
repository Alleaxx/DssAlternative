using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

namespace DSSAlternative.AHP
{
    public interface IStyled
    {
        string GetClass();
        string GetStyle();
    }
    public class DSSComponent : ComponentBase
    {
        public DSS DSS => DSS.Ex;
    }
    public class DSSProject : DSSComponent
    {
        protected IProject Project => DSS.Project;
        protected IProblem Problem => Project.Problem;

        protected IStage Hier => Project.StageHier;
        protected IStage View => Project.StageView;
        protected IStage Res => Project.StageResults;


        protected IProblemNode[] Nodes => Project.Problem.Hierarchy.Select(n => new ProblemNode(Project.Problem, n)).ToArray();


        protected ICorrectness HierEditState => Project.ProblemEditing.Correctness;
        protected IRelationsCorrecntess RelState => Project.Problem.CorrectnessRels;
    }

    public class DSSComponentNode : DSSProject
    {
        [Parameter]
        public INode Node { get; set; }

        [Parameter]
        public int Level { get; set; }
        [Parameter]
        public string NodeName { get; set; }


        protected IMatrix Mtx => Problem.GetMatrix(Node);


        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if(NodeName != null)
            {
                Node = Problem.Dictionary[Level].Where(n => n.Name == NodeName).FirstOrDefault();
            }

            if(Node == null)
            {
                Node = Problem.Dictionary[Level].Where(n => n.Name == NodeName).FirstOrDefault();
            }

        }
    }

    public class DSSComponentRelation : DSSProject
    {
        
        [Parameter]
        public int RelIndex { get; set; } = -1;

        [Parameter]
        public INodeRelation Relation { get; set; }


        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (RelIndex > -1 && RelIndex < Problem.RelationsAll.Count())
            {
                Relation = Problem.RelationsAll.ElementAt(RelIndex);
            }
        }
    }

}
