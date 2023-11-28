using DSSAlternative.AHP.HierarchyInfo;
using DSSAlternative.AHP.Logs;
using DSSAlternative.AHP.Relations;
using DSSAlternative.AHP.Templates;
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
        event Action<IRelationsHierarchy> OnRelationChanged;

        /// <summary>
        /// Возникает при полной смене подтвержденной иерархии
        /// </summary>
        event Action<IHierarchy> OnActiveHierChanged;

        /// <summary>
        /// Возникает при полной смене редактируемой иерархии или её внутренних изменениях
        /// </summary>
        event Action<IHierarchy> OnEditingHierChanged;

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
        /// Карта изменений узлов текущей редактируемой и прошлой утвержденной иерархии
        /// </summary>
        INodeMap NodeMap { get; }





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
        IRelationsHierarchy Relations { get; }


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
        IRelationNode RelationSelected { get; }

        void SelectNodeRelation(IRelationNode rel);
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

        public event Action<IRelationsHierarchy> OnRelationChanged;
        public event Action<IHierarchy> OnActiveHierChanged;
        public event Action<IHierarchy> OnEditingHierChanged;

        #endregion

        public IHierarchy HierarchyEditing { get; private set; }
        public IHierarchy HierarchyActive { get; private set; }
        public IRelationsHierarchy Relations { get; private set; }
        public INodeMap NodeMap { get; private set; }



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
        public bool UnsavedChanged => NodeMap.GetState(HierarchyEditing).State != CompareHierState.NoChanges;     
        public bool IsUpdateAvailable => UnsavedChanged && HierarchyEditing.Correctness.IsCorrect;
        public bool IsActiveHierCreated => !IsEmptyProject && HierarchyActive.Correctness.IsCorrect;  
        public bool IsEmptyProject { get; init; }



        #region Конструкторы

        public Project(ITemplateProject template)
        {
            NodeMap = new NodeMap(this);
            SetEditingHierarchy(new HierarchyNodesList(template));
            SetActiveHierarchyAsEditing();
            Relations.SetValuesFromTemplate(template);
            Logger.Default.AddInfo(this, "Создание проекта из шаблона", Name, cate: LogCategory.Projects);
        }
        public Project(IEnumerable<INode> nodes, bool isEmpty = false)
        {
            NodeMap = new NodeMap(this);
            SetEditingHierarchy(new HierarchyNodesList(nodes));
            SetActiveHierarchy(HierarchyExamples.CreateEmptyHierarchy());
            IsEmptyProject = isEmpty;
            Logger.Default.AddInfo(this, "Создание проект по узлам", Name, cate: LogCategory.Projects);
        }
        
        #endregion

        public void SetActiveHierarchyAsEditing()
        {
            var state = NodeMap.GetState(HierarchyEditing);
            if (state.State == CompareHierState.MinorFieldsChanges)
            {
                UpdateActiveHierarchy();
            }
            else if (state.State == CompareHierState.StructureFieldsChanges || state.State == CompareHierState.CollectionChanges)
            {
                SetActiveHierarchy(HierarchyEditing.CreateCopy());
            }
        }
        private void SetActiveHierarchy(IHierarchy hierarchy)
        {
            HierarchyActive = hierarchy;

            HierarchyActive.SealThisHierarchy();
            CreateSetNewRelations();
            Relations_OnChanged(Relations, Relations.RelationsCriteria.FirstOrDefault(), Relations.GetAllNodeComparesMini().FirstOrDefault());
            OnActiveHierChanged?.Invoke(HierarchyActive);
            NodeMap = new NodeMap(HierarchyEditing, HierarchyActive);


            void CreateSetNewRelations()
            {
                if (Relations != null)
                {
                    Relations.OnInnerRelationValueChanged -= Relations_OnChanged;
                }
                Relations = new RelationsHierarchy(HierarchyActive);
                Relations.OnInnerRelationValueChanged += Relations_OnChanged;
                SelectNodeRelation(Relations[HierarchyActive.MainGoal].FirstNodeCompareRequired());
            }

            void Relations_OnChanged(IRelationsHierarchy obj, IRelationsCriteria criteria, IRelationNode node)
            {
                OnRelationChanged?.Invoke(obj);
            }
        }
        private void UpdateActiveHierarchy()
        {
            foreach (var pair in NodeMap.ConfirmMap)
            {
                var editNode = pair.Key;
                var activeNode = pair.Value;

                activeNode.CopyMinorFieldsFrom(editNode);
            }
            OnActiveHierChanged?.Invoke(HierarchyActive);
        }

        public void SetEditingHierarchyAsActive()
        {
            SetEditingHierarchy(HierarchyActive.CreateCopy());
        }
        private void SetEditingHierarchy(IHierarchy hierarchy)
        {
            var oldHier = HierarchyEditing;
            if (oldHier != null)
            {
                HierarchyEditing.OnNodeFieldsUpdated -= Hierarchy_OnNodeFieldsUpdated;
                HierarchyEditing.OnNodesListUpdated -= Hierarchy_OnNodesListUpdated;
            }

            HierarchyEditing = hierarchy;
            HierarchyEditing.OnNodeFieldsUpdated += Hierarchy_OnNodeFieldsUpdated;
            HierarchyEditing.OnNodesListUpdated += Hierarchy_OnNodesListUpdated;
            OnEditingHierChanged?.Invoke(HierarchyEditing);
            if (HierarchyActive != null)
            {
                NodeMap = new NodeMap(HierarchyEditing, HierarchyActive);
            }
            SelectNode(HierarchyEditing.MainGoal);
        }

        private void Hierarchy_OnNodesListUpdated(IHierarchy obj)
        {
            OnEditingHierChanged?.Invoke(obj);
        }
        private void Hierarchy_OnNodeFieldsUpdated(INode obj)
        {
            OnEditingHierChanged?.Invoke(obj.Hierarchy);
        }




        #region Выбранные узлы в интерфейсе


        public event Action OnSelectedRelationChanged;
        public event Action OnSelectedNodeChanged;

        public IRelationNode RelationSelected { get; private set; }
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
        public void SelectNodeRelation(IRelationNode rel)
        {
            RelationSelected = rel;
            OnSelectedRelationChanged?.Invoke();
        }
        
        #endregion
    }

}
