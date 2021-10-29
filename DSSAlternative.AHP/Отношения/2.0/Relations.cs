using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface IUsable
    {
        bool Correct { get; }
        bool Consistent { get; }
        bool Known { get; }
    }
    //Отдельный элемент, воплощающий отношения для списка узлов
    //Пересоздается при изменении текущей коллекции в проекте
    public interface IRelations : IEnumerable<ICriteriaRelation>, IRelationsGrouped, IUsable
    {
        ICriteriaRelation this[INode main] { get; }

        event Action<IRelations> OnChanged;
        void Set(INode main, INode from, INode to, double value);
        IRelationsCorrectness CorrectnessRels { get; }


        INodeRelation NextRequiredRel(INodeRelation from);
        INodeRelation PrevRequiredRel(INodeRelation from);

        void SetUnknown();

        void SetFromTemplate(ITemplate template);
    }

    //Список критериев по иерархии
    public class Relations : List<ICriteriaRelation>, IRelations
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Отношения из {Count} критериев | корректность: {Correct}, согласованность {Consistent}, известность {Known}");
            foreach (var criteria in this)
            {
                sb.Append($"\n{criteria}");
            }
            return sb.ToString();
        }


        public event Action<IRelations> OnChanged;

        public readonly IHierarchy Hierarchy;
        private Dictionary<INode, ICriteriaRelation> Criterias { get; set; }

        public Relations(IHierarchy hierarchy)
        {
            Hierarchy = hierarchy;
            CreateCriterias();
            RecountCoefficients();
        }

        //Создание критериев и отношений
        private void CreateCriterias()
        {
            foreach (var node in Hierarchy.RelationNodes)
            {
                var criteria = new CriteriaRelation(this, node);
                Add(criteria);
                criteria.OnChanged += InnerCriteriaChanged;
            }
            Criterias = this.ToDictionary(c => c.Key);
        }


        private void InnerCriteriaChanged(ICriteriaRelation relation)
        {
            RecountCoefficients();
            OnChanged?.Invoke(this);
        }
        private void RecountCoefficients()
        {
            Hierarchy.MainGoal.Coefficient = 1;
            foreach (var levelNodes in Hierarchy.GroupedByLevel)
            {
                int level = levelNodes.Key;
                if (level > 0)
                {
                    foreach (var node in levelNodes)
                    {
                        var coefficients = VectorMtx.CreateCoeffs(this, node);
                        double coeff = coefficients[node];

                        node.Coefficient = coeff;
                    }
                }
            }
        }



        //Получение отношений
        public ICriteriaRelation this[INode main]
        {
            get => Criterias[main];
        }
        private IEnumerable<INodeRelation> AllRelations => this.SelectMany(criteria => criteria);
        public IEnumerable<IGrouping<INode, INodeRelation>> RelationsGroupedMain(INode node)
        {
            return Criterias[node].MtxView;
        }
        public INodeRelation NextRequiredRel(INodeRelation from)
        {
            var criteria = this[from.Main];
            if (criteria.Correct)
            {

            }
            else
            {
                
            }
            throw new NotImplementedException();
        }
        public INodeRelation PrevRequiredRel(INodeRelation from)
        {
            var criteria = this[from.Main];
            if (criteria.Correct)
            {

            }
            else
            {

            }
            throw new NotImplementedException();
        }


        //Состояние пригодности
        public bool Correct => this.All(c => c.Correct);
        public bool Consistent => this.All(c => c.Consistent);
        public bool Known => this.All(c => c.Known);

        public IRelationsCorrectness CorrectnessRels => throw new NotImplementedException();


        //Установка значений
        public void Set(INode main, INode from, INode to, double value)
        {
            Criterias[main].Set(from, to, value);
        }
        public void SetUnknown()
        {
            foreach (var criteria in this)
            {
                criteria.SetUnknown();
            }
        }
        public void SetFromTemplate(ITemplate template)
        {
            SetUnknown();
            foreach (var rel in template.Relations)
            {
                INode main = Hierarchy.FirstOrDefault(n => n.Name == rel.Main);
                INode from = Hierarchy.FirstOrDefault(n => n.Name == rel.From);
                INode to = Hierarchy.FirstOrDefault(n => n.Name == rel.To);
                if (main != null && from != null && to != null)
                {
                    Set(main, from, to, rel.Value);
                }
            }
        }
    }
}
