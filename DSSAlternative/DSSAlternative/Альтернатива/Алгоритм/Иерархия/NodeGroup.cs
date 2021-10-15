using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface INodeGroup
    {
        int Index { get; }
        INode[] Group { get; }
        INode[] GroupHier { get; }
    }
    public class NodeGroup : INodeGroup
    {
        public int Index { get; private set; }
        public INode[] Group { get; } = Array.Empty<INode>();
        public INode[] GroupHier => Group.OfType<INode>().ToArray();

        public NodeGroup(int index, params INode[] nodes)
        {
            Index = index;
            Group = nodes;
        }
    }
}
