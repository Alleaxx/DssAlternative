using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface INode
    {
        string Name { get; set; }
        string LevelName { get; }
        string Description { get; set; }


        int Level { get; set; }
        int Group { get; set; }
        int GroupIndex { get; set; }

        double Coefficient { get; set; }
        string GetLevelName(IEnumerable<INode> nodes);

        public bool CompareValuesWith(INode node);



        int OrderIndexLevel { get; }
        int OrderIndexGroup { get; }


        INodeGroup Criterias { get; }
        INode[] UpperNodes { get; }

        INode[] Neighbors { get; }
        INode[] NeighborsGroup { get; }

        INode[] LowerNodes { get; }
        INode[] LowerNodesControlled { get; }

        void UpdateStructure(IEnumerable<INode> allNodes, IEnumerable<INodeGroup> groups);

        IProblem Owner { get; set; }
    }
    public class Node : INode, ICloneable
    {
        public override string ToString() => $"{Name} [{Level}]";

        public string Name { get; set; }
        public string LevelName { get; protected set; }
        public string Description { get; set; }

        //Уровень узла в иерархии
        public int Level { get; set; }                  //Представление уровня иерархии
        //Номер группы узла
        public int Group { get; set; }                  //Критерий составляет группу, которой могут принадлежать другие критерии
        //Номер группы критериев для узла
        public int GroupIndex { get; set; }             //Критерий принадлежит группе данных критериев

        [JsonIgnore]
        public double Coefficient { get; set; }

        public string GetLevelName(IEnumerable<INode> nodes)
        {
            int maxLevel = nodes.Select(n => n.Level).Max();
            if (Level == 0)
                return "Цель";
            else if (Level == maxLevel)
                return "Альтернативы";
            else if (Level == 1)
                return "Критерии";
            else if (Level < 0)
                return "???";
            else
                return "Подкритерии";
        }
        [JsonIgnore]
        public IProblem Owner { get; set; }
        public Node() { }
        public Node(int level, string name, int group, int groupIndex)
        {
            Name = name;
            Level = level;
            Group = group;
            GroupIndex = groupIndex;
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


        //Порядковые номера в группе и на уровне
        [JsonIgnore]
        public int OrderIndexLevel => Neighbors.ToList().IndexOf(this);
        [JsonIgnore]
        public int OrderIndexGroup => NeighborsGroup.ToList().IndexOf(this);


        [JsonIgnore]
        public INodeGroup Criterias { get; private set; }
        [JsonIgnore]
        public INode[] UpperNodes { get; private set; }
        [JsonIgnore]
        public INode[] Neighbors { get; private set; }
        [JsonIgnore]
        public INode[] NeighborsGroup { get; private set; }
        [JsonIgnore]
        public INode[] LowerNodes { get; private set; }
        [JsonIgnore]
        public INode[] LowerNodesControlled { get; private set; }


        public void UpdateStructure(IEnumerable<INode> allNodes, IEnumerable<INodeGroup> groups)
        {
            LevelName = GetLevelName(allNodes);

            if (GroupIndex == -1)
            {
                Criterias = new NodeGroup(-1);
            }
            else if (GroupIndex < groups.Count())
            {
                Criterias = groups.ElementAt(GroupIndex);
            }

            UpperNodes = allNodes.Where(n => n.Level == Level - 1).ToArray();
            Neighbors = allNodes.Where(n => n.Level == Level).ToArray();
            NeighborsGroup = allNodes.Where(n => n.GroupIndex == GroupIndex).ToArray();
            LowerNodes = allNodes.Where(n => n.Level == Level + 1).ToArray();
            LowerNodesControlled = allNodes.Where(n => n.GroupIndex == Group).ToArray();
        }


        public object Clone()
        {
            return CloneThis();
        }
        public Node CloneThis()
        {
            return MemberwiseClone() as Node;
        }
    }
}
