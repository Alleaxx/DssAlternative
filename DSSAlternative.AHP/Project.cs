using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    /// <summary>
    /// Проект задачи принятия решений. Имеет разделение на редактируемую и активную иерархию с отношениями
    /// </summary>
    public interface IProject
    {
        /// <summary>
        /// Возникает при изменении любого отношения в задаче
        /// </summary>
        event Action<IRelations> OnRelationChanged;

        /// <summary>
        /// Возникает при полной смене подтвержденной иерархии
        /// </summary>
        event Action<IHierarchy> OnActiveHierChanged;

        /// <summary>
        /// Возникает при полной смене редактируемой иерархии
        /// </summary>
        event Action<IHierarchy> OnEditingHierChanged;

        /// <summary>
        /// Возникает при внутренних изменениях редактируемой иерархии
        /// </summary>
        event Action<IHierarchy> OnEditingHierUpdated;


        /// <summary>
        /// Имя задачи (= главному узлу утвержденной иерархии)
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Путь к изображению задачи (равен главному узлу утвержденной иерархии)
        /// </summary>
        string ImgPath { get; }


        /// <summary>
        /// Текущий статус задачи
        /// </summary>
        string Status { get; }

        /// <summary>
        /// Признак разницы в структурах редактируемой и активной иерархии
        /// </summary>
        bool UnsavedChanged { get; }

        /// <summary>
        /// Признак доступности обновления редактируемой структурой (если есть изменения и они корректные)
        /// </summary>
        bool IsUpdateAvailable { get; }

        /// <summary>
        /// Признак наличия полноценной активной иерархии
        /// </summary>
        bool IsActiveHierCreated { get; }

        /// <summary>
        /// Признак проекта-заглушки
        /// </summary>
        bool IsEmptyProject { get; }





        /// <summary>
        /// Редактируемая иерархия задачи без отношений. Может быть утверждена
        /// </summary>
        IHierarchy HierarchyEditing { get; }

        /// <summary>
        /// Утвержденная иерархия задачи, для которой созданы отношения
        /// </summary>
        IHierarchy HierarchyActive { get; }

        /// <summary>
        /// Отношения для утвержденной иерархии задачи
        /// </summary>
        IRelations Relations { get; }


        /// <summary>
        /// Обновить активную иерархию узлами редактируемой иерархии
        /// </summary>
        void SetActiveHierarchyAsEditing();

        /// <summary>
        /// Обновить редактируемую иерархию узлами активной иерархии
        /// </summary>
        void SetEditingHierarchyAsActive();



        event Action OnSelectedRelationChanged;
        event Action OnSelectedNodeChanged;
        /// <summary>
        /// Выбранный узел. (желательно из редактируемой иерархии)
        /// </summary>
        INode NodeSelected { get; }

        /// <summary>
        /// Выбранное отношение
        /// </summary>
        INodeRelation RelationSelected { get; }

        void SelectNodeRelation(INodeRelation rel);
        void SelectNode(INode node);

    }

    /// <summary>
    /// Проект задачи задачи принятия решений. Имеет разделение на редактируемую и активную иерархию с отношениями
    /// </summary>
    public class Project : IProject
    {
        public override string ToString()
        {
            return HierarchyActive.ToString();
        }

        #region События

        public event Action<IRelations> OnRelationChanged;
        public event Action<IHierarchy> OnActiveHierChanged;
        public event Action<IHierarchy> OnEditingHierChanged;
        public event Action<IHierarchy> OnEditingHierUpdated;

        #endregion

        public IHierarchy HierarchyEditing { get; private set; }
        public IHierarchy HierarchyActive { get; private set; }   
        public IRelations Relations { get; private set; }



        public string Name => HierarchyActive?.MainGoal?.Name ?? "Неопознанная задача";
        public string ImgPath => HierarchyActive?.MainGoal?.ImgPath;
        public string Status
        {
            get
            {
                string status = "Готова, есть результат!";
                if (!IsActiveHierCreated)
                {
                    return "Нужна иерархия";
                }
                if (!Relations.Known)
                {
                    return "Есть неизвестные отношения";
                }
                if (!Relations.Consistent)
                {
                    return "Есть несогласованность в связях";
                }
                return status;
            }
        }
        public bool UnsavedChanged => !HierarchyExtensions.CompareEqual(HierarchyActive, HierarchyEditing);     
        public bool IsUpdateAvailable => UnsavedChanged && HierarchyEditing.Correctness.IsCorrect;
        public bool IsActiveHierCreated => !IsEmptyProject && HierarchyActive.Correctness.IsCorrect;  
        public bool IsEmptyProject { get; init; }



        #region Конструкторы

        public Project(ITemplateProject template)
        {
            SetEditingHierarchy(new HierarchyNodesList(template));
            SetActiveHierarchyAsEditing();
            Relations.SetFromTemplate(template);
        }
        public Project(IEnumerable<INode> nodes, bool isEmpty = false)
        {
            SetEditingHierarchy(new HierarchyNodesList(nodes));
            SetActiveHierarchy(HierarchyExamples.CreateEmptyHierarchy());
            IsEmptyProject = isEmpty;
        }
        
        #endregion

        public void SetActiveHierarchyAsEditing()
        {
            SetActiveHierarchy(HierarchyEditing.GetNodesCopy());
        }
        private void SetActiveHierarchy(IHierarchy hierarchy)
        {
            HierarchyActive = hierarchy;
            HierarchyActive.SetConnectedHierarchy(HierarchyEditing);
            HierarchyEditing.SetConnectedHierarchy(HierarchyActive);

            SelectNode(HierarchyEditing.MainGoal);
            CreateSetNewRelations();
            Relations_OnChanged(Relations);
            OnActiveHierChanged?.Invoke(HierarchyActive);


            void CreateSetNewRelations()
            {
                if (Relations != null)
                {
                    Relations.OnChanged -= Relations_OnChanged;
                }
                Relations = new Relations(HierarchyActive);
                Relations.OnChanged += Relations_OnChanged;
                SelectNodeRelation(Relations[HierarchyActive.MainGoal].FirstRequired);
            }

            void Relations_OnChanged(IRelations obj)
            {
                OnRelationChanged?.Invoke(obj);
            }
        }

        public void SetEditingHierarchyAsActive()
        {
            SetEditingHierarchy(HierarchyActive.GetNodesCopy());
        }
        private void SetEditingHierarchy(IHierarchy hierarchy)
        {
            var oldHier = HierarchyEditing;
            if (oldHier != null)
            {
                HierarchyEditing.OnChanged -= HierarchyEditing_OnUpdated;
            }

            HierarchyEditing = hierarchy;
            HierarchyEditing.OnChanged += HierarchyEditing_OnUpdated;
            OnEditingHierChanged?.Invoke(HierarchyEditing);
            if (HierarchyActive != null)
            {
                HierarchyActive.SetConnectedHierarchy(HierarchyEditing);
                HierarchyEditing.SetConnectedHierarchy(HierarchyActive);
            }
        }

        private void HierarchyEditing_OnUpdated()
        {
            OnEditingHierUpdated?.Invoke(HierarchyEditing);
        }


        #region Выбранные узлы в интерфейсе


        public event Action OnSelectedRelationChanged;
        public event Action OnSelectedNodeChanged;

        public INodeRelation RelationSelected { get; private set; }
        public INode NodeSelected { get; private set; }

        public void SelectNode(INode node)
        {
            if (!HierarchyEditing.Nodes.Contains(node) && node != null)
            {
                return;
            }

            NodeSelected = node;
            OnSelectedNodeChanged?.Invoke();
        }
        public void SelectNodeRelation(INodeRelation rel)
        {
            RelationSelected = rel;
            OnSelectedRelationChanged?.Invoke();
        }
        
        #endregion
    }

}
