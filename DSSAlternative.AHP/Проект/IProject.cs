using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface IProject
    {
        event Action UpdatedHierOrRelationChanged;

        bool UnsavedChanged { get; }
        bool CanTranferEditing { get; }


        IHierarchy HierarchyEditing { get; }
        IProblem ProblemActive { get; }
        IRelations Relations { get; set; }

        INode NodeSelected { get; }
        INodeRelation RelationSelected { get; }
        void SetNow(INodeRelation rel);
        void SetNow(INode node);

        event Action OnRelationChanged;
        event Action OnNodeChanged;

        void UpdateProblemFromEditing();
        string Status { get; }
    }
}
