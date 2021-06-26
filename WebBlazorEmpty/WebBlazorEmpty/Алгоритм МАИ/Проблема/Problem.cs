using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface IProblem : IHierarchy
    {
        INodeRelation[] RelationsAll { get; }
        INodeRelation[] RelationsRequired { get; }
        IEnumerable<IGrouping<INode, INodeRelation>> RelationsGroupedMain(INode node);


        event Action RelationValueChanged;
        void SetRelationBetween(INode main, INode from, INode to, double value);
        IRelationsCorrectness CorrectnessRels { get; }


        INodeRelation FirstRequiredRelation(INode node);
        INodeRelation NextRequiredRel(INodeRelation from);
        INodeRelation PrevRequiredRel(INodeRelation from);

        void ClearRelations();
        void ClearRelations(INode node);

        IMatrixRelations GetMtxRelations(INode node);
    }
    public class Problem : HierarchySheme, IProblem
    {
        public event Action RelationValueChanged;

        public Problem(ITemplate template) : base(template)
        {
            CreateRelations();
            RecountCoeffs();
        }

        //Конструирование задачи и отношений
        private void CreateRelations()
        {
            var relations = GetRelations();
            RelationsAll = relations.ToArray();

            foreach (var rel in relations)
            {
                rel.Mirrored = relations.Find(mirr => mirr.Main == rel.Main && mirr.To == rel.From && mirr.From == rel.To);
            }
            RelationsRequired = GetRequiredRelations().ToArray();

            CorrectnessRels = new RelationsCorrectness(this);
        }
        private List<INodeRelation> GetRelations()
        {
            List<INodeRelation> relations = new List<INodeRelation>();
            foreach (var node in Hierarchy)
            {
                var neigbors = Hierarchy.Where(n => Enumerable.SequenceEqual(n.Criterias.Group, node.Criterias.Group));
                foreach (var criteria in node.Criterias.Group)
                {
                    foreach (var nodeNeighbor in neigbors)
                    {
                        bool relationExists = relations.Exists(r => r.Main == criteria && r.From == node && r.To == nodeNeighbor);
                        if (!relationExists)
                        {
                            NodeRelation relation = new NodeRelation(criteria, node, nodeNeighbor, 0);
                            relations.Add(relation);
                            relation.Changed += RelationValue_Changed;
                        }
                    }
                }
            }

            return relations;

        }
        private List<INodeRelation> GetRequiredRelations()
        {
            List<INodeRelation> onlyRequired = RelationsAll.Where(r => r.From != r.To).ToList();
            for (int i = 0; i < onlyRequired.Count; i++)
            {
                var relMirrored = onlyRequired[i].Mirrored;
                if (onlyRequired.Remove(relMirrored))
                    i--;
            }
            return onlyRequired;
        }


        private void RelationValue_Changed(Relation<INode, INode> changedRelation)
        {
            RecountCoeffs();
            RelationValueChanged?.Invoke();
        }



        public INodeRelation[] RelationsAll { get; private set; }
        public INodeRelation[] RelationsRequired { get; private set; }
        public IEnumerable<IGrouping<INode, INodeRelation>> RelationsGroupedMain(INode node) => RelationsAll.Where(g => g.Main == node).GroupBy(r => r.From);
        
        public IMatrixRelations GetMtxRelations(INode node) => new MtxRelations(this,node);


        private void RecountCoeffs()
        {
            MainGoal.Coefficient = 1;
            foreach (var levelNodes in GroupedByLevel)
            {
                int level = levelNodes.Key;
                if (level > 0)
                {
                    foreach (var node in levelNodes)
                    {
                        var coefficients = new MtxCoeffs(this, node);
                        double coeff = coefficients.Get(node);

                        node.Coefficient = coeff;
                    }
                }
            }
        }

        public void ClearRelations(INode node)
        {
            foreach (var relation in RelationsRequired.Where(r => r.Main == node))
            {
                relation.Clear();
            }
            RelationValueChanged?.Invoke();
        }
        public void ClearRelations()
        {
            foreach (var node in NodesWithRels)
            {
                ClearRelations(node);
            }
        }


        public IRelationsCorrectness CorrectnessRels { get; private set; }


        public void SetRelationBetween(INode main, INode from, INode to, double value)
        {
            var foundRelation = RelationsAll.Where(r => r.Main == main && r.From == from && r.To == to).FirstOrDefault();

            if (foundRelation != null)
            {
                foundRelation.Value = value;
            }
            else
            {
                Console.WriteLine("Отношение не найдено!");
            }
        }


        //Первое заполняемое отношение для узла
        public INodeRelation FirstRequiredRelation(INode node) => RelationsRequired.ToList().Find(r => r.Main == node);

        //Предыдущее и следующее заполняемое отношение относительно заданного
        private INodeRelation GetRequiredRel(INodeRelation from, int inc)
        {
            int index = RelationsRequired.ToList().IndexOf(from);
            int newIndex = index + inc;

            if (newIndex >= RelationsRequired.Length)
                return RelationsRequired.First();
            if (newIndex < 0)
                return RelationsRequired.Last();

            return RelationsRequired[newIndex];
        }
        public INodeRelation NextRequiredRel(INodeRelation from)
        {
            var isMtxOK = GetMtxRelations(from.Main).Consistency.IsCorrect();
            var firstRelation = RelationsRequired.ToList().Find(r => r.Main == from.Main);
            var lastRelation = RelationsRequired.ToList().FindLast(r => r.Main == from.Main);

            var nextRelation = GetRequiredRel(from, 1);
            if (isMtxOK)
                return nextRelation;
            else if (from == lastRelation)
                return firstRelation;
            else
                return nextRelation;
        }
        public INodeRelation PrevRequiredRel(INodeRelation from) => GetRequiredRel(from, -1);
    }
}
