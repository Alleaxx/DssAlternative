using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface ICriteriaRelation : IGrouping<INode, INodeRelation>, IUsable
    {
        event Action<ICriteriaRelation> OnChanged;

        IEnumerable<INode> Nodes { get; }
        IEnumerable<INodeRelation> Required { get; }
        INodeRelation this[INode from, INode to] { get; }
        INodeRelation FirstRequired { get; }

        void Set(INode from, INode to, double value);
        void SetUnknown();

        IEnumerable<IGrouping<INode, INodeRelation>> MtxView { get; }
        IMatrix Mtx { get; }
    }
}
