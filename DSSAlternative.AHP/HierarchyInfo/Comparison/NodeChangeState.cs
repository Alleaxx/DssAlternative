using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP.HierarchyInfo
{
    /// <summary>
    /// Состояние сравнения узлов в иерархиях
    /// </summary>
    public enum NodeChangeState
    {
        NoChanges,
        MinorChanges,
        StructureChanges,
        Added,
        Removed
    }
}
