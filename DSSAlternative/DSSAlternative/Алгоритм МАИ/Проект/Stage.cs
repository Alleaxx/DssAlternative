using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
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
            bool unsaved = Project.UnsavedChanged;
            bool isCorrectHierarchy = Project.ProblemEditing.Correctness.Result;

            if (unsaved)
                Add("warning");
            if (!isCorrectHierarchy)
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
            bool areRelationsKnown = Project.Problem.CorrectnessRels.AreRelationsKnown;
            bool areRelationsConsistent = Project.Problem.CorrectnessRels.AreRelationsConsistenct;

            if (!areRelationsKnown)
                Add("warning");
            if (!areRelationsConsistent)
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

        private int IndexOfRelation => Project.Problem.RelationsAll.ToList().IndexOf(Relation);
        public override string Href => $"relation-edit/{IndexOfRelation}";

        protected override void AddRules()
        {
            bool relationsUnknown = Relation.Unknown;
            bool relationConsistent = Project.Problem.GetMtxRelations(Relation.Main).Consistency.IsCorrect();

            Add("safe");
            if (relationsUnknown)
                Add("warning");
            if (!relationConsistent)
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
        public override string Href => "results";
        protected override void AddRules()
        {
            bool areRelationsCorrect = Project.Problem.CorrectnessRels.AreRelationsCorrect;

            if (!areRelationsCorrect)
                Add("none");
        }
    }
}
