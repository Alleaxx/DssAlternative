using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{

    public interface IProject
    {
        event Action UpdatedHierOrRelationChanged;

        string ViewFilter { get; set; }
        bool UnsavedChanged { get; }
        bool CanTranferEditing { get; }

        ITemplate TemplateEditing { get; }
        IHierarchy ProblemEditing { get; }
        IProblem ProblemActive { get; }
        

        StageHierarchy StageHier { get; }
        StageView StageView { get; }
        StageResults StageResults { get; }
        StageRelation GetStageFromNode(INode node);
        INodeRelation GetRelFromNode(INode node);
        StageRelation GetStageFromRel(INodeRelation relation);

        INode NodeSelected { get; }
        INodeRelation RelationSelected { get; }
        void SetNow(INodeRelation rel);
        void SetNow(INode node);

        event Action OnRelationChanged;
        event Action OnNodeChanged;


        void UpdateProblemFromEditing();
        string Status { get; }
    }
    public class Project : IProject
    {
        public override string ToString()
        {
            return ProblemActive.ToString();
        }
        public event Action UpdatedHierOrRelationChanged;

        public event Action OnStructureUpdated;
        public event Action OnRelationsUpdated;

        //Редактируемое состояние
        public ITemplate TemplateEditing { get; private set; }
        public IHierarchy ProblemEditing => new HierarchySheme(TemplateEditing);

        //Текущая проблема
        public IProblem ProblemActive { get; private set; }


        public bool UnsavedChanged => !HierarchySheme.CompareEqual(ProblemActive, ProblemEditing);
        public bool CanTranferEditing => UnsavedChanged && ProblemEditing.Correctness.IsCorrect;

        public string Status
        {
            get
            {
                string status = "Готова";
                if (!ProblemActive.CorrectnessRels.AreKnown)
                {
                    return "Нужна информация";
                }
                if (!ProblemActive.CorrectnessRels.AreConsistenct)
                {
                    return "Нужна корректировка";
                }
                return status;
            }
        }
        public string ViewFilter { get; set; } = "По отношениям";


        public Project(ITemplate template, bool activeInit = true)
        {
            Console.WriteLine("Создание проекта и обновление иерархии");

            if (activeInit)
            {
                SetActiveProblem(template);
            }
            else
            {
                TemplateEditing = template;
            }
        }
        public void UpdateProblemFromEditing()
        {
            Console.WriteLine("Обновление иерархии");
            SetActiveProblem(TemplateEditing);
        }
        private void SetActiveProblem(ITemplate template)
        {
            TemplateEditing = template.CloneThis();
            IProblem old = ProblemActive;
            ProblemActive = new Problem(template);
            CreateStages();
            SetNow(ProblemActive.MainGoal);

            if (old != null)
            {
                old.RelationValueChanged -= Update;
            }
            ProblemActive.RelationValueChanged += Update;
            Update();

            void Update()
            {
                UpdatedHierOrRelationChanged?.Invoke();
            }
        }



        //Стадии решения проблемы
        private void CreateStages()
        {
            StageHier = new StageHierarchy(this);
            StageView = new StageView(this);
            StageResults = new StageResults(this);
            StageRelations = ProblemActive.RelationsAll.Select(r => new StageRelation(this, r)).ToArray();
            SetNow(StageRelations.First().Relation);
        }

        public StageHierarchy StageHier { get; private set; }
        public StageView StageView { get; private set; }
        public StageResults StageResults { get; private set; }

        public INode NodeSelected { get; set; }
        public INodeRelation RelationSelected { get; set; }
        public void SetNow(INodeRelation rel)
        {
            RelationSelected = rel;
            Console.WriteLine("Обновилось выбранное отношение");
            OnRelationChanged?.Invoke();
        }
        public void SetNow(INode node)
        {
            NodeSelected = node;
            OnNodeChanged?.Invoke();
        }

        public event Action OnRelationChanged;
        public event Action OnNodeChanged;

        private IEnumerable<StageRelation> StageRelations { get; set; }

        public StageRelation GetStageFromRel(INodeRelation relation)
        {
            var stage = StageRelations.FirstOrDefault(s => s.Relation == relation);
            if (stage != null)
            {
                return stage;
            }
            return StageRelations.First();
        }
        public StageRelation GetStageFromNode(INode node)
        {
            var relation = ProblemActive.FirstRequiredRelation(node);
            return GetStageFromRel(relation);
        }
        public INodeRelation GetRelFromNode(INode node)
        {
            return GetStageFromNode(node).Relation;
        }
    }

}
