using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP.HierarchyInfo
{
    /// <summary>
    /// Отчёт по сравнению двух иерархий
    /// </summary>
    public class ComparisonHierarchy
    {
        public CompareHierState State { get; private set; }
        public IEnumerable<ComparisonNode> Details { get; private set; }

        public ComparisonHierarchy(IEnumerable<ComparisonNode> details)
        {
            Details = details;
            SetState();
        }
        private void SetState()
        {
            State = CompareHierState.NoChanges;
            if (Details.Any(c => c.State == CompareNodeState.Added || c.State == CompareNodeState.Removed))
            {
                State = CompareHierState.CollectionChanges;
            }
            else
            {
                if (Details.Any(c => c.State == CompareNodeState.StructureChanges))
                {
                    State = CompareHierState.StructureFieldsChanges;
                }
                else
                {
                    if (Details.Any(c => c.State == CompareNodeState.MinorChanges))
                    {
                        State = CompareHierState.MinorFieldsChanges;
                    }
                }
            }
        }
    }


    /// <summary>
    /// Состояние сравнения двух иерархий
    /// </summary>
    public enum CompareHierState
    {
        NoChanges,
        MinorFieldsChanges,
        StructureFieldsChanges,
        CollectionChanges
    }
}
