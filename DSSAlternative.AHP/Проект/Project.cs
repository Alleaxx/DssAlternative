using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
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
        public IHierarchy HierarchyEditing { get; init; }

        //Текущая проблема
        public IProblem ProblemActive { get; private set; }
        public IRelations Relations { get; private set; }

        public bool UnsavedChanged => !HierarchyNodes.CompareEqual(ProblemActive, HierarchyEditing);
        public bool CanTranferEditing => UnsavedChanged && HierarchyEditing.Correctness.IsCorrect;

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


        public Project(ITemplate template)
        {
            Console.WriteLine("Создание проекта и обновление иерархии");
            HierarchyEditing = new HierarchyNodes(template);
            UpdateProblemFromEditing();
            ProblemActive.FillRelations(template);
        }
        public Project(IEnumerable<INode> nodes)
        {
            Console.WriteLine("Создание проекта и обновление иерархии");
            HierarchyEditing = new HierarchyNodes(nodes);
            UpdateProblemFromEditing();
        }
        public void UpdateProblemFromEditing()
        {
            if (CanTranferEditing)
            {
                ReplaceCurrentProblem(HierarchyEditing);
            }
            else
            {
                Console.WriteLine("Обновление иерархии недоступно");
            }
        }
        private void ReplaceCurrentProblem(IHierarchy hierarchy)
        {
            ITemplate copy = new Template(hierarchy).CloneThis();
            IProblem old = ProblemActive;

            RegisterProblem(new Problem(copy));
            if (old != null)
            {
                old.RelationValueChanged -= RelationsUpdated;
            }
        }
        private void RegisterProblem(IProblem problem)
        {
            ProblemActive = problem;
            SetNow(ProblemActive.MainGoal);

            ProblemActive.RelationValueChanged += RelationsUpdated;
            RelationsUpdated();

            Relations = new Relations(ProblemActive);
            Relations.SetFromTemplate(new Template(ProblemActive.OfType<Node>(), ProblemActive.RelationsRequired));
        }
        private void RelationsUpdated()
        {
            UpdatedHierOrRelationChanged?.Invoke();
            if(Relations != null)
            {
                Relations.SetFromTemplate(new Template(ProblemActive.OfType<Node>(), ProblemActive.RelationsRequired));

                Console.WriteLine("ИЗМЕНЕНИЕ ОТНОШЕНИЙ");
                Console.WriteLine(Relations);
                Console.WriteLine();
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
