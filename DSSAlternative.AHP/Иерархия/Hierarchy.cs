using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    /// <summary>
    /// Иерархия узлов - коллекция элементов INode образующая структуру
    /// </summary>
    public interface IHierarchy
    {
        IHierarchy ConnectedHierarchy { get; }
        void SetConnectedHierarchy(IHierarchy hierarchy);

        public INode MainGoal { get; }
        IEnumerable<INode> Nodes { get; }
        public ICorrectness Correctness { get; }


        public int LevelsCount { get; }
        public int RelationsCount { get; }
        TimeSpan CountEstTime();


        event Action OnChanged;
        void AddNode(INode node);
        void RemoveNode(INode node);

        IHierarchy GetNodesCopy();
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

        //Список узлов
        public IEnumerable<INode> Nodes => this;
        public INode MainGoal => this.FirstOrDefault(h => h.Level == 0);
        public ICorrectness Correctness { get; init; }

        //Общая информация об иерархии
        public int LevelsCount => this.Select(n => n.Level).Distinct().Count();
        public int RelationsCount
        {
            get
            {
                int relsAmount = 0;

                int prevAmount = 1;
                foreach (var group in this.NodesGroupedByGroup())
                {
                    int groupAmount = group.Count();
                    relsAmount += prevAmount * (groupAmount * groupAmount / 3);
                    prevAmount = groupAmount;
                }
                return relsAmount;
            }
        }
        public TimeSpan CountEstTime()
        {
            return new TimeSpan(0, 0, RelationsCount * 8);
        }



        //Конструкторы
        public HierarchyNodesList(ITemplateProject template) : this(template.Nodes.OfType<INode>().ToArray())
        {

        }
        public HierarchyNodesList(params INode[] nodes) : this(nodes.AsEnumerable())
        {

        }
        public HierarchyNodesList(IEnumerable<INode> nodes)
        {
            Correctness = new HierarchyCheck(this);
            foreach (var node in nodes)
            {
                AddNode(node);
                node.OnNodeFieldsChanged += NodeUpdated;
            }
        }



        //Изменение коллекции
        public event Action OnChanged;
        public void AddNode(INode node)
        {
            if (Contains(node))
            {
                return;
            }

            this.Add(node);
            node.SetHierarchy(this);
            node.OnNodeFieldsChanged += NodeUpdated;
            OnChanged?.Invoke();
        }
        public void RemoveNode(INode node)
        {
            bool isRemoved = Remove(node);
            if (isRemoved)
            {
                node.RemoveHierarchy();
                node.OnNodeFieldsChanged -= NodeUpdated;
                OnChanged?.Invoke();
            }
        }
        private void NodeUpdated(INode node)
        {
            OnChanged?.Invoke();
        }



        /// <summary>
        /// Связанная иерархия, куда могут переноситься изменения
        /// </summary>
        public IHierarchy ConnectedHierarchy { get; private set; }
        public void SetConnectedHierarchy(IHierarchy hierarchy)
        {
            ConnectedHierarchy = hierarchy;
        }


        //Копирование и сравнение
        public IHierarchy GetNodesCopy()
        {
            var copyProject = new TemplateProject(this).CloneThis();
            return new HierarchyNodesList(copyProject);
        }
    }

}
