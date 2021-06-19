using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBlazorEmpty.AHP
{

    public interface IProject
    {
        event Action Updated;

        string ViewFilter { get; set; }
        bool UnsavedChanged { get; }

        List<INode> NodesEditing { get; set; }
        IHierarchy ProblemEditing { get; }
        IProblem Problem { get; }
        
        IStage StageHier { get; }
        IStage StageView { get; }
        IStage StageResults { get; }

        Dictionary<INodeRelation, IStage> StageRelations { get; }


        void UpdateProblem();
        string Status { get; }
    }
    public class Project : IProject
    {
        public event Action Updated;

        public override string ToString() => Problem.ToString();
        public string Status => Problem.CorrectnessRels.AreRelationsCorrect ? "Готово к анализу" : "Требуется корректировка данных";

        //Иерархия проблемы
        public List<INode> NodesEditing { get; set; }
        public IHierarchy ProblemEditing => new HierarchyN(NodesEditing);

        public IStage StageHier { get; private set; }

        public bool UnsavedChanged => !HierarchyN.CompareEqual(Problem, ProblemEditing);

        //Текущая проблема
        public IProblem Problem { get; private set; }

        public string ViewFilter { get; set; } = "По отношениям";

        public Project(IEnumerable<INode> nodes)
        {
            SetProblem(nodes);      
        }

        private void SetStages()
        {
            StageHier = new StageHierarchy(this);
            StageView = new StageView(this);
            StageResults = new StageResults(this);

            StageRelations.Clear();
            foreach (var relation in Problem.RelationsRequired)
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
            SetProblem(NodesEditing);
            Updated?.Invoke();
        }
        public void SetProblem(IEnumerable<INode> nodes)
        {
            NodesEditing = nodes.ToList();
            IProblem old = Problem;
            Problem = new Problem(nodes);
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
