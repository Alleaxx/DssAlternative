using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface IRelationsGrouped
    {
        IEnumerable<IGrouping<INode, INodeRelation>> RelationsGroupedMain(INode node);
    }

    //Отдельный элемент, воплощающий отношения для списка узлов
    //Пересоздается при изменении текущей коллекции в проекте
    //Список критериев по иерархии

    public interface IRelations : IEnumerable<ICriteriaRelation>, IRelationsGrouped, IUsable
    {
        ICriteriaRelation this[INode main] { get; }

        event Action<IRelations> OnChanged;
        void Set(INode main, INode from, INode to, double value);


        INodeRelation NextRequiredRel(INodeRelation from);
        INodeRelation PrevRequiredRel(INodeRelation from);

        void SetUnknown();

        void SetFromTemplate(ITemplate template);
    }
}
