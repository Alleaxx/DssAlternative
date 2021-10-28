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
        string Name { get; set; }
        string Description { get; set; }


        int Level { get; set; }
        int Group { get; set; }
        int GroupIndex { get; set; }
        double Coefficient { get; set; }

        IHierarchy Hierarchy { get; }
        void SetHierarchy(IHierarchy hier);

        bool CompareValuesWith(INode node);
    }
    public class Node : INode
    {
        public override string ToString() => $"{Name} [{Level}]";

        public string Name { get; set; }
        public string Description { get; set; }

        public int Level { get; set; }                  //Уровень узла в иерархии
        public int Group { get; set; }                  //Критерий составляет группу, которой могут принадлежать другие критерии
        public int GroupIndex { get; set; }             //Критерий принадлежит группе критериев c этим индексом

        public double Coefficient { get; set; }


        public Node()
        {

        }
        public Node(int level, string name, int group, int groupIndex)
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
    }

    public static class NodeExtensions
    {
        public static IEnumerable<INode> Criterias2(this INode node)
        {
            return node.Hierarchy.Where(n => n.Group == node.GroupIndex).ToArray();
        }
        public static IEnumerable<INode> UpperNodes(this INode node)
        {
            return node.Hierarchy.Where(n => n.Level == node.Level - 1);
        }
        public static IEnumerable<INode> LowerNodes(this INode node)
        {
            return node.Hierarchy.Where(n => n.Level == node.Level + 1);
        }

        public static IEnumerable<INode> NeighborsLevel(this INode node)
        {
            return node.Hierarchy.Where(n => n.Level == node.Level);
        }
        public static IEnumerable<INode> NeighborsGroup(this INode node)
        {
            return node.Hierarchy.Where(n => n.GroupIndex == node.GroupIndex);
        }

        public static IEnumerable<INode> LowerNodesControlled(this INode node)
        {
            return node.Hierarchy.Where(n => n.GroupIndex == node.Group);
        }
        public static string LevelName(this INode node)
        {
            int maxLevel = node.Hierarchy.Select(n => n.Level).Max();
            if (node.Level == 0)
                return "Цель";
            else if (node.Level == maxLevel)
                return "Альтернативы";
            else if (node.Level == 1)
                return "Критерии";
            else if (node.Level < 0)
                return "???";
            else
                return "Подкритерии";
        }

        //Порядковые номера в группе и на уровне
        public static int OrderIndexLevel(this INode node) => node.NeighborsLevel().ToList().IndexOf(node);
        public static int OrderIndexGroup(this INode node) => node.NeighborsGroup().ToList().IndexOf(node);

    }
}
