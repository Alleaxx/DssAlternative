using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP.HierarchyInfo
{
    /// <summary>
    /// Состояние сравнения двух иерархий
    /// </summary>
    public enum HierarchyChangeState
    {
        NoChanges,
        MinorFieldsChanges,
        StructureFieldsChanges,
        CollectionChanges
    }
}
