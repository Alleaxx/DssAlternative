using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{

    public interface IProject
    {
        event Action Updated;

        string ViewFilter { get; set; }
        bool UnsavedChanged { get; }

        ITemplate Template { get; set; }
        IHierarchy ProblemEditing { get; }
        IProblem Problem { get; }
        
        IStage StageHier { get; }
        IStage StageView { get; }
        IStage StageResults { get; }

        Dictionary<INodeRelation, IStage> StageRelations { get; }
        IStage GetRelation(INode node);


        void UpdateProblem();
        string Status { get; }
    }
    public class Project : IProject
    {
        public event Action Updated;

        public override string ToString() => Problem.ToString();
        public string Status
        {
            get
            {
                string status = "Готова";
                if (!Problem.CorrectnessRels.AreRelationsKnown)
                    status = "Нужна информация";
                if (!Problem.CorrectnessRels.AreRelationsConsistenct)
                    status = "Нужна корректировка";
                return status;
            }
        }

        //Иерархия проблемы
        public ITemplate Template { get; set; }
        public IHierarchy ProblemEditing => new HierarchyN(Template);

        public IStage StageHier { get; private set; }

        public IStage GetRelation(INode node) => StageRelations[Problem.FirstRequiredRelation(node)];

        public bool UnsavedChanged => !HierarchyN.CompareEqual(Problem, ProblemEditing);

        //Текущая проблема
        public IProblem Problem { get; private set; }

        public string ViewFilter { get; set; } = "По отношениям";

        public Project(ITemplate template)
        {
            SetProblem(template);      
        }

        private void SetStages()
        {
            StageHier = new StageHierarchy(this);
            StageView = new StageView(this);
            StageResults = new StageResults(this);

            StageRelations.Clear();
            foreach (var relation in Problem.RelationsAll)
            {
                IStage relStage = new StageRelation(this, relation);
                StageRelations.Add(relation, relStage);
            }
        }


        public Dictionary<INodeRelation, IStage> StageRelations { get; private set; } = new Dictionary<INodeRelation, IStage>();
        public IStage StageView { get; private set; }
        public IStage StageResults { get; private set; }


        public void UpdateProblem()
        {
            SetProblem(Template);
            Updated?.Invoke();
        }
        public void SetProblem(ITemplate template)
        {
            Template = template;
            IProblem old = Problem;
            Problem = new Problem(template);
            SetStages();

            if(old != null)
            {
                old.RelationValueChanged -= Update;
            }
            Problem.RelationValueChanged += Update;

            void Update()
            {
                Updated?.Invoke();
            }
        }
    }

}
