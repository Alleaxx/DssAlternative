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
        double Coefficient { get; set; }
    }

    public class Node : INode
    {
        public override string ToString() => $"{Name} [{Level}]";

        public string Name { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }

        [JsonIgnore]
        public double Coefficient { get; set; }

        public Node() { }
        public Node(int level, string name) : this(level, name, "Описание узла") { }
        public Node(int level, string name, string descr)
        {
            Name = name;
            Level = level;
            Description = descr;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
