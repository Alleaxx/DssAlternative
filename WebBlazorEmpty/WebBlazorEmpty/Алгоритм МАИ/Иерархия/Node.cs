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

        int OrderIndexLevel { get; }
        int OrderIndexGroup { get; }

        double Coefficient { get; set; }


        INodeGroup Criterias { get; }
        INode[] UpperNodes { get; }

        INode[] Neighbors { get; }
        INode[] NeighborsGroup { get; }

        INode[] LowerNodes { get; }
        INode[] LowerNodesControlled { get; }

        void UpdateStructure(IEnumerable<INode> allNodes, INodeGroup[] groups);
    }

    public class Node : INode
    {
        public override string ToString() => $"{Name} [{Level}]";

        public string Name { get; set; }
        public string LevelName { get; private set; }
        public string Description { get; set; }

        //Уровень узла в иерархии
        public int Level { get; set; }
        //Номер группы узла
        public int Group { get; set; }
        //Номер группы критериев для узла
        public int GroupIndex { get; set; }

        //Порядковые номера в группе и на уровне
        public int OrderIndexLevel => Neighbors.ToList().IndexOf(this);
        public int OrderIndexGroup => NeighborsGroup.ToList().IndexOf(this);



        [JsonIgnore]
        public double Coefficient { get; set; }


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



        public void UpdateStructure(IEnumerable<INode> allNodes, INodeGroup[] groups)
        {
            LevelName = GetLevelName(allNodes);

            if (GroupIndex == -1)
                Criterias = new NodeGroup(-1);
            else if (GroupIndex < groups.Length)
                Criterias = groups[GroupIndex];

            UpperNodes = allNodes.Where(n => n.Level == Level - 1).ToArray();
            Neighbors = allNodes.Where(n => n.Level == Level).ToArray();
            NeighborsGroup = allNodes.Where(n => n.GroupIndex == GroupIndex).ToArray();
            LowerNodes = allNodes.Where(n => n.Level == Level + 1).ToArray();
            LowerNodesControlled = allNodes.Where(n => n.GroupIndex == Group).ToArray();
        }
        private string GetLevelName(IEnumerable<INode> nodes)
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


        public Node() { }
        public Node(int level, string name, int group, int groupIndex)
        {
            Name = name;
            Level = level;
            Group = group;
            GroupIndex = groupIndex;
        }
    }


    public interface INodeGroup
    {
        int Index { get; }
        INode[] Group { get; }
    }
    public class NodeGroup : INodeGroup
    {
        public int Index { get; private set; }
        public INode[] Group { get; } = new INode[0];

        public NodeGroup(int index,params INode[] nodes)
        {
            Index = index;
            Group = nodes;
        }
    }
}
