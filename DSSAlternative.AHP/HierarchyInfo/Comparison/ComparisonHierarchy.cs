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
        public HierarchyChangeState State { get; private set; }
        public IEnumerable<ComparisonNode> Details { get; private set; }

        public ComparisonHierarchy(IEnumerable<ComparisonNode> details)
        {
            Details = details;
            SetState();
        }

        private void SetState()
        {
            State = HierarchyChangeState.NoChanges;
            if (Details.Any(c => c.State == NodeChangeState.Added || c.State == NodeChangeState.Removed))
            {
                State = HierarchyChangeState.CollectionChanges;
            }
            else
            {
                if (Details.Any(c => c.State == NodeChangeState.StructureChanges))
                {
                    State = HierarchyChangeState.StructureFieldsChanges;
                }
                else
                {
                    if (Details.Any(c => c.State == NodeChangeState.MinorChanges))
                    {
                        State = HierarchyChangeState.MinorFieldsChanges;
                    }
                }
            }
        }
    }
}
