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
        public IHierarchy ProblemEditing => new HierarchyNodes(NodesEditing);

        public IStage StageHier { get; private set; }

        public bool UnsavedChanged => !HierarchyNodes.CompareEqual(Problem, ProblemEditing);

        //Текущая проблема
        public IProblem Problem { get; private set; }

        public string ViewFilter { get; set; } = "По согласованности отношений";

        public Project(IEnumerable<INode> nodes)
        {
            SetProblem(nodes);      
        }

        private void SetStages()
        {
            StageHier = new Stage("Формирование иерархии");
            StageView = new Stage("Обзор проблемы");
            StageResults = new Stage("Анализ результатов");

            if (StageHier is Stage stH)
            {
                stH.GetWarning = () => UnsavedChanged || Problem == null;
                stH.GetError = () => ProblemEditing.Correctness.Result;
            }
            if (StageView is Stage st)
            {
                st.GetWarning = WarnView;
                st.GetHidden = () => Problem == null;
            }
            if (StageResults is Stage stR)
            {
                stR.GetHidden = WarnView;
            }

            bool WarnView()
            {
                return !Problem.CorrectnessRels.AreRelationsCorrect;
            }

            StageRelations.Clear();
            int counter = 0;
            foreach (var relation in Problem.RelationsRequired)
            {
                Stage relStage = new Stage($"{counter}-е определение связей ");
                counter++;
                StageRelations.Add(relation, relStage);

                relStage.GetWarning = IsWarned;
                relStage.GetError = IsErrored;

                bool IsErrored()
                {
                    return !Problem.GetMatrix(relation.Main).Consistency.IsCorrect();
                }
                bool IsWarned()
                {
                    return relation.Unknown;
                }
            }
            StageHier = new Stage("");
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
