using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface INode : ICloneable
    {
        public event Action<INode> OnChanged;
        public event Action<INode, IHierarchy> OnMoved;

        string Name { get; set; }
        string Description { get; set; }
        string ImgPath { get; set; }


        int Level { get; set; }
        string Group { get; set; }
        string GroupIndex { get; set; }
        double Coefficient { get; set; }

        IHierarchy Hierarchy { get; }
        void SetHierarchy(IHierarchy hier);
        void RemoveFromHierarchy();

        bool CompareValuesWith(INode node);
        string CreateNodeId();
    }
    public class Node : INode
    {
        public override string ToString()
        {
            return $"{Name} [{Level}]";
        }

        public event Action<INode> OnChanged;
        public event Action<INode, IHierarchy> OnMoved;

        public string Name { get; set; }
        public string Description { get; set; }
        public string ImgPath { get; set; }

        public int Level
        {
            get => level;
            set
            {
                level = value > 0 ? value : 0;
                OnChanged?.Invoke(this);
            }
        }                
        private int level;
        //Критерий составляет группу, которой могут принадлежать другие критерии
        public string Group
        {
            get => group;
            set
            {
                group = value;
                OnChanged?.Invoke(this);
            }
        }                
        private string group;
        //Критерий принадлежит группе критериев c этим индексом
        public string GroupIndex
        {
            get => groupIndex;
            set
            {
                groupIndex = value;
                OnChanged?.Invoke(this);
            }
        }      
        private string groupIndex;

        [JsonIgnore]
        public double Coefficient { get; set; }


        public Node()
        {

        }
        public Node(string name) : this(0, name, 0, -1)
        {

        }
        public Node(int level, string name, int group, int groupIndex)
        {
            Name = name;
            Level = level;
            Group = group.ToString();
            GroupIndex = groupIndex.ToString();
        }
        public Node(int level, string name, string group, string groupIndex)
        {
            Name = name;
            Level = level;
            Group = group;
            GroupIndex = groupIndex;
        }



        [JsonIgnore]
        public IHierarchy Hierarchy { get; private set; }

        public void SetHierarchy(IHierarchy hierarchy)
        {
            Hierarchy = hierarchy;
            OnMoved?.Invoke(this, hierarchy);
            if (hierarchy != null)
            {
                hierarchy.AddNode(this);
            }
        }
        public void RemoveFromHierarchy()
        {
            SetHierarchy(null);
        }


        public object Clone()
        {
            return CloneThis();
        }
        public Node CloneThis()
        {
            return MemberwiseClone() as Node;
        }
        public bool CompareValuesWith(INode node)
        {
            if (!node.Name.Equals(Name))
            {
                return false;
            }
            if (Level != node.Level)
            {
                return false;
            }
            if (Group != node.Group)
            {
                return false;
            }
            if (GroupIndex != node.GroupIndex)
            {
                return false;
            }
            return true;
        }

        public string CreateNodeId()
        {
            return $"{Name}{Level}{Group}{GroupIndex}";
        }
    }

    public static class NodeExtensions
    {
        //Критерии для данного узла
        public static IEnumerable<INode> Criterias(this INode node)
        {
            return node.Hierarchy.Where(n => n.Group == node.GroupIndex).ToArray();
        }
        public static IEnumerable<INode> Controlled(this INode node)
        {
            return node.Hierarchy.Where(n => n.GroupIndex == node.Group);
        }

        //Навигация по иерархии
        public static IEnumerable<INode> UpperLevel(this INode node)
        {
            return node.Hierarchy.Where(n => n.Level == node.Level - 1);
        }
        public static IEnumerable<INode> LowerLevel(this INode node)
        {
            return node.Hierarchy.Where(n => n.Level == node.Level + 1);
        }

        //Узлы на том же уровне и в тех же группах
        public static IEnumerable<INode> NeighborsLevel(this INode node)
        {
            return node.Hierarchy.Where(n => n.Level == node.Level);
        }
        public static IEnumerable<INode> NeighborsGroup(this INode node)
        {
            return node.Hierarchy.Where(n => n.Group == node.Group);
        }
        public static IEnumerable<INode> NeighborsGroupIndex(this INode node)
        {
            return node.Hierarchy.Where(n => n.GroupIndex == node.GroupIndex);
        }

        //Контролируемые узлы
        public static string LevelName(this INode node)
        {
            int maxLevel = node.Hierarchy.Select(n => n.Level).Max();
            if (node.Level == 0)
            {
                return "Цель";
            }
            else if (node.Level == maxLevel)
            {
                return "Альтернативы";
            }
            else if (node.Level == 1)
            {
                return "Критерии";
            }
            else if (node.Level < 0)
            {
                return "???";
            }
            return "Подкритерии";
        }


        //Порядковые номера в группе и на уровне
        public static int OrderIndexLevel(this INode node)
        {
            return node.NeighborsLevel().ToList().IndexOf(node);
        }
        public static int OrderIndexGroup(this INode node)
        {
            return node.NeighborsGroupIndex().ToList().IndexOf(node);
        }
    }
}
