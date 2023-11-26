using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP.HierarchyInfo
{
    /// <summary>
    /// Отчёт по сравнению узлов в двух иерархиях
    /// </summary>
    public class ComparisonNode
    {
        public CompareNodeState State { get; private set; }
        public INode First { get; private set; }
        public INode Second { get; private set; }

        public ComparisonNode(INode first, CompareNodeState state)
        {
            First = first;
            State = state;
        }
        public ComparisonNode(INode first, INode second)
        {
            First = first;
            Second = second;
            CompareSetState();
        }
        
        private void CompareSetState()
        {
            var firstHash = First.GetHashFromFields(false);
            var secondHash = Second.GetHashFromFields(false);
            if (firstHash != secondHash)
            {
                State = CompareNodeState.StructureChanges;
                return;
            }

            firstHash = First.GetHashFromFields(true);
            secondHash = Second.GetHashFromFields(true);
            if (firstHash != secondHash)
            {
                State = CompareNodeState.MinorChanges;
                return;
            }

            State = CompareNodeState.NoChanges;
        }
    }

    /// <summary>
    /// Состояние сравнения узлов в иерархиях
    /// </summary>
    public enum CompareNodeState
    {
        NoChanges,
        MinorChanges,
        StructureChanges,
        Added,
        Removed
    }
}
