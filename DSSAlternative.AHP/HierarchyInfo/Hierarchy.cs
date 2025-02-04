using DSSAlternative.AHP.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DSSAlternative.AHP.HierarchyInfo
{
    /// <summary>
    /// Интерфейс иерархии узлов - коллекции элементов INode образующих структуру
    /// </summary>
    public interface IHierarchy
    {
        /// <summary>
        /// Главный узел иерархии
        /// </summary>
        public INode MainGoal { get; }

        /// <summary>
        /// Коллекция узлов иерархии
        /// </summary>
        IEnumerable<INode> Nodes { get; }
        
        /// <summary>
        /// Корректность существующей иерархии
        /// </summary>
        public ICorrectness Correctness { get; }

        /// <summary>
        /// Количество уровней в иерархии
        /// </summary>
        public int LevelsCount { get; }
        
        /// <summary>
        /// Количество потенциальных отношений для иерархии (не факт, что они будут созданы)
        /// </summary>



        /// <summary>
        /// Возникает при добавлении / удалении узлов в иерархии
        /// </summary>
        event Action<IHierarchy> OnNodesListUpdated;
        /// <summary>
        /// Возникает при изменении полей узла иерархии
        /// </summary>
        event Action<INode> OnNodeFieldsUpdated;


        
        /// <summary>
        /// Добавить несколько узлов в иерархию
        /// </summary>
        void AddNodes(params INode[] nodes);

        /// <summary>
        /// Добавить узел в иерархию
        /// </summary>
        void AddNode(INode node);

        /// <summary>
        /// Удалить узел из иерархии. 
        /// </summary>
        void RemoveNode(INode node);


        /// <summary>
        /// Признак того, что иерархия запечатан
        /// </summary>
        bool IsSealed { get; }
        /// <summary>
        /// Запретить изменение состава узлов иерархии
        /// </summary>
        void SealThisHierarchy();

        /// <summary>
        /// Создать копию иерархии
        /// </summary>
        IHierarchy CreateCopy();
    }

    /// <summary>
    /// Иерархия узлов - коллекция элементов INode образующая структуру
    /// </summary>
    public class HierarchyNodesList : List<INode>, IHierarchy
    {
        public override string ToString()
        {
            return $"{MainGoal.Name} ({Count})";
        }

        public event Action<IHierarchy> OnNodesListUpdated;
        public event Action<INode> OnNodeFieldsUpdated;

        #region Свойства

        public IEnumerable<INode> Nodes => this;
        public INode MainGoal => this.FirstOrDefault(h => h.Level == 0);
        public ICorrectness Correctness { get; init; }

        public int LevelsCount => this.Select(n => n.Level).Distinct().Count();
        public bool IsSealed { get; private set; }

        #endregion

        #region Конструкторы

        private HierarchyNodesList()
        {
            Correctness = new CorrectnessHierarchy(this);
        }
        public HierarchyNodesList(ITemplateProject template) : this()
        {
            var nodes = template.Nodes.Select(n => n.CreateNode()).ToArray();
            AddNodes(nodes);
        }
        public HierarchyNodesList(IEnumerable<INode> nodes) : this()
        {
            AddNodes(nodes.ToArray());
        }
        
        #endregion

        #region Методы

        public void AddNodes(params INode[] nodes)
        {
            foreach (var node in nodes)
            {
                AddNode(node);
            }
        }
        public void AddNode(INode node)
        {
            if (Contains(node) || IsSealed)
            {
                return;
            }

            this.Add(node);
            node.SetHierarchy(this);
            node.OnNodeFieldsChanged += OnInnerNodeUpdated;
            OnNodesListUpdated?.Invoke(this);
        }
        public void RemoveNode(INode node)
        {
            if (IsSealed)
            {
                return;
            }

            bool isRemoved = Remove(node);
            if (isRemoved)
            {
                node.SetHierarchy(null);
                node.OnNodeFieldsChanged -= OnInnerNodeUpdated;
                OnNodesListUpdated?.Invoke(this);
            }
        }
        private void OnInnerNodeUpdated(INode node)
        {
            OnNodeFieldsUpdated?.Invoke(node);
        }

        public void SealThisHierarchy()
        {
            IsSealed = true;
        }

        public IHierarchy CreateCopy()
        {
            var nodesCopy = Nodes.Select(n => new TemplateNode(n)).Select(t => t.CreateNode()).ToArray();
            return new HierarchyNodesList(nodesCopy);
        }

        #endregion
    }

}
