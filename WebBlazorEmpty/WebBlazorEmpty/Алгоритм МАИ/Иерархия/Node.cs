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
        string Description { get; set; }

        int Level { get; set; }
        int Group { get; set; }
        int GroupIndex { get; set; }

        double Coefficient { get; set; }


        //Список критериев для узла
        INode[] Contents { get; }
        //Список подконтрольных критериев
        INode[] ContentsControlled { get; }


        //Список критериев узла
        //Список вышестоящих узлов
        INodeGroup Criterias { get; }
        INode[] UpperNodes { get; }

        //Список соседей узла
        //Список сравниваемых соседей узла
        INode[] Neighbors { get; }
        INode[] NeighborsComp { get; }

        //Список нижних узлов, для которых это критерий
        //Список всех нижних узлов
        INode[] LowerNodes { get; }
        INode[] LowerNodesControlled { get; }


        void UpdateStructure(IEnumerable<INode> allNodes, INodeGroup[] groups);
    }

    public class Node : INode
    {
        public override string ToString() => $"{Name} [{Level}]";

        public string Name { get; set; }
        public string Description { get; set; }

        //Уровень узла в иерархии
        public int Level { get; set; }
        //Номер группы узла
        public int Group { get; set; }
        //Номер группы критериев для узла
        public int GroupIndex { get; set; }



        [JsonIgnore]
        public double Coefficient { get; set; }

        [JsonIgnore]
        public INode[] Contents { get; set; }

        [JsonIgnore]
        public INode[] ContentsControlled { get; set; }


        [JsonIgnore]
        public INodeGroup Criterias { get; private set; }
        [JsonIgnore]
        public INode[] UpperNodes { get; private set; }
        [JsonIgnore]
        public INode[] Neighbors { get; private set; }
        [JsonIgnore]
        public INode[] NeighborsComp { get; private set; }
        [JsonIgnore]
        public INode[] LowerNodes { get; private set; }
        [JsonIgnore]
        public INode[] LowerNodesControlled { get; private set; } = new INode[0];

        public void UpdateStructure(IEnumerable<INode> allNodes, INodeGroup[] groups)
        {
            //Console.WriteLine(this);
            //for (int i = 0; i < groups.Length; i++)
            //{
            //    Console.Write($"\n{i}) ");
            //    foreach (var item in groups[i].Group)
            //    {
            //        Console.Write(item + ", ");
            //    }
            //}
            if (GroupIndex == -1)
                Criterias = new NodeGroup(-1);
            else if (GroupIndex < groups.Length)
                Criterias = groups[GroupIndex];

            UpperNodes = allNodes.Where(n => n.Level == Level - 1).ToArray();
            Neighbors = allNodes.Where(n => n.Level == Level).ToArray();
            NeighborsComp = allNodes.Where(n => n.GroupIndex == GroupIndex).ToArray();
            LowerNodes = allNodes.Where(n => n.Level == Level + 1).ToArray();

            var group = groups.ToList().Find(g => g.Group.Contains(this));
            if (group != null)
            {
                int groupIndex = groups.ToList().IndexOf(group);
                LowerNodesControlled = allNodes.Where(n => n.Group == groupIndex).ToArray();
            }

            Contents = Criterias.Group;
            ContentsControlled = LowerNodesControlled;

            //Console.Write("Критерии: ");
            //Criterias.Group.ToList().ForEach(n => Console.Write(n.Name + ", "));
            //Console.Write("\n");
        }

        public Node() { }
        public Node(int level, string name) : this(level, name, "Описание узла") { }
        public Node(int level, string name, int group, int groupIndex)
        {
            Name = name;
            Level = level;
            Group = group;
            GroupIndex = groupIndex;
        }
        public Node(int level, string name, string descr)
        {
            Name = name;
            Level = level;
            Description = descr;
            Group = level - 1;
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
