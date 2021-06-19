using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBlazorEmpty.AHP
{
    public interface IStage : IStyled
    {
        string Href { get; }
    }

    public class Stage : IStage
    {
        private List<string> Classes { get; set; } = new List<string>();

        public virtual string Href => "home";
        
        protected void Add(string str) => Classes.Add(str);

        public string GetClass()
        {
            Classes.Clear();
            AddRules();
            return string.Join(' ', Classes);
        }
        protected virtual void AddRules()
        {

        }
        public virtual string GetStyle() => "";
    }

    public class StageHierarchy : Stage
    {
        private IProject Project { get; set; }
        public StageHierarchy(IProject project)
        {
            Project = project;
        }

        public override string Href => "hierarchy";
        protected override void AddRules()
        {
            if (Project.UnsavedChanged)
                Add("warning");
            if (!Project.ProblemEditing.Correctness.Result)
                Add("error");
        }
    }
    public class StageView : Stage
    {
        private IProject Project { get; set; }
        public StageView(IProject problem)
        {
            Project = problem;
        }
        public override string Href => "view";
        protected override void AddRules()
        {
            if (!Project.Problem.CorrectnessRels.AreRelationsKnown)
                Add("warning");
            if (!Project.Problem.CorrectnessRels.AreRelationsConsistenct)
                Add("error");
        }
    }
    public class StageRelation : Stage
    {
        private IProject Project { get; set; }
        private INodeRelation Relation { get; set; }
        public StageRelation(IProject project,INodeRelation rel)
        {
            Project = project;
            Relation = rel;
        }
        public override string Href => $"relation-new/{Project.Problem.RelationsAll.ToList().IndexOf(Relation)}";
        protected override void AddRules()
        {
            Add("safe");
            if (Relation.Unknown)
                Add("warning");
            if (!Project.Problem.GetMatrix(Relation.Main).Consistency.IsCorrect())
                Add("error");
        }
    }
    public class StageResults : Stage
    {
        private IProject Project { get; set; }
        public StageResults(IProject project)
        {
            Project = project;
        }
        public override string Href => "/results";
        protected override void AddRules()
        {
            if (!Project.Problem.CorrectnessRels.AreRelationsCorrect)
                Add("none");
        }
    }
}
