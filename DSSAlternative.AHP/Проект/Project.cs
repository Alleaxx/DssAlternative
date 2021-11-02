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
            return HierarchyActive.ToString();
        }
        public event Action UpdatedHierOrRelationChanged;

        public event Action OnStructureUpdated;
        public event Action OnRelationsUpdated;



        //Редактируемое состояние
        public IHierarchy HierarchyEditing { get; init; }
        public IHierarchy HierarchyActive { get; private set; }
        public IRelations Relations { get; private set; }


        //Текущий статус проекта
        public string Status
        {
            get
            {
                string status = "Готова";
                if (!Created)
                {
                    return "Нужна иерархия";
                }
                if (!Relations.Known)
                {
                    return "Есть незаполненные связи";
                }
                if (!Relations.Consistent)
                {
                    return "Есть несогласованность";
                }
                return status;
            }
        }
        public bool UnsavedChanged => !HierarchyNodes.CompareEqual(HierarchyActive, HierarchyEditing);
        public bool IsUpdateAvailable => UnsavedChanged && HierarchyEditing.Correctness.IsCorrect;
        public bool Created => HierarchyActive.Correctness.IsCorrect;

        //Создание проекта
        public Project(ITemplate template)
        {
            HierarchyEditing = new HierarchyNodes(template);
            UpdateHierarchy();
            Relations.SetFromTemplate(template);
        }
        public Project(IEnumerable<INode> nodes)
        {
            HierarchyEditing = new HierarchyNodes(nodes);
            UpdateHierarchy(new HierarchyNodes(new Node("???")));
        }
        public void UpdateHierarchy()
        {
            if (IsUpdateAvailable)
            {
                ITemplate copy = new Template(HierarchyEditing).CloneThis();
                UpdateHierarchy(new HierarchyNodes(copy));
            }
        }
        private void UpdateHierarchy(IHierarchy problem)
        {
            HierarchyActive = problem;
            SetNow(HierarchyActive.MainGoal);
            CreateSetNewRelations();
            Relations_OnChanged(Relations);
            void CreateSetNewRelations()
            {
                if (Relations != null)
                {
                    Relations.OnChanged -= Relations_OnChanged;
                }
                Relations = new Relations(HierarchyActive);
                Relations.OnChanged += Relations_OnChanged;
                SetNow(Relations[HierarchyActive.MainGoal].FirstRequired);
            }
            void Relations_OnChanged(IRelations obj)
            {
                UpdatedHierOrRelationChanged?.Invoke();
            }
        }




        public event Action OnRelationChanged;
        public event Action OnNodeChanged;

        public INodeRelation RelationSelected { get; private set; }
        public INode NodeSelected { get; private set; }
        
        public void SetNow(INode node)
        {
            NodeSelected = node;
            OnNodeChanged?.Invoke();
        }
        public void SetNow(INodeRelation rel)
        {
            RelationSelected = rel;
            OnRelationChanged?.Invoke();
        }
    }

}
