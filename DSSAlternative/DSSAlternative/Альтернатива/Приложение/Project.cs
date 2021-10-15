using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{

    public interface IProject
    {
        event Action UpdatedHierOrRelationChanged;

        bool UnsavedChanged { get; }
        bool CanTranferEditing { get; }


        ITemplate TemplateEditing { get; }
        IHierarchy ProblemEditing { get; }
        IProblem ProblemActive { get; }
        

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
            if (CanTranferEditing)
            {
                SetActiveProblem(TemplateEditing);
            }
            else
            {
                Console.WriteLine("Обновление иерархии недоступно");
            }
        }
        private void SetActiveProblem(ITemplate template)
        {
            TemplateEditing = template.CloneThis();
            IProblem old = ProblemActive;
            ProblemActive = new Problem(template);
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



        public INodeRelation RelationSelected { get; set; }
        public void SetNow(INodeRelation rel)
        {
            RelationSelected = rel;
            OnRelationChanged?.Invoke();
        }

        public INode NodeSelected { get; set; }
        public void SetNow(INode node)
        {
            NodeSelected = node;
            OnNodeChanged?.Invoke();
        }

        public event Action OnRelationChanged;
        public event Action OnNodeChanged;
    }

}
