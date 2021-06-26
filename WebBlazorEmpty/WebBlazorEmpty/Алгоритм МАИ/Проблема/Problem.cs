using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface IProblem : IHierarchy
    {
        Dictionary<int, INode[]> Dictionary { get; }

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

        IEnumerable<INode> Best(int level);
    }
    public class Problem : HierarchyN, IProblem
    {
        public event Action RelationValueChanged;

        public Problem(ITemplate template) : base(template)
        {
            UpdateStructure();
            RecountCoeffs();
        }

        //Конструирование задачи и отношений
        private void UpdateStructure()
        {
            Dictionary = GetDictionary();
            CreateRelations();
        }
        private Dictionary<int, INode[]> GetDictionary()
        {
            Dictionary<int, INode[]> dictionary = new Dictionary<int, INode[]>();
            foreach (var nodeGroup in GroupedByLevel)
            {
                dictionary.Add(nodeGroup.Key, nodeGroup.ToArray());
            }
            return dictionary;
        }
        private void CreateRelations()
        {
            var relations = GetRelationsNew();
            RelationsAll = relations.ToArray();

            foreach (var rel in relations)
            {
                rel.Mirrored = relations.Find(mirr => mirr.Main == rel.Main && mirr.To == rel.From && mirr.From == rel.To);
            }

            List<INodeRelation> onlyReq = RelationsAll.Where(r => r.From != r.To).ToList();
            for (int i = 0; i < onlyReq.Count; i++)
            {
                if (onlyReq.Remove(onlyReq[i].Mirrored))
                    i--;
            }
            RelationsRequired = onlyReq.ToArray();

            CorrectnessRels = new RelationsCorrectness(this);
        }
        private List<INodeRelation> GetRelationsOld()
        {
            List<INodeRelation> relations = new List<INodeRelation>();
            foreach (var level in Dictionary.Keys)
            {
                if (level != MaxLevel)
                {
                    var mainNodes = Dictionary[level];
                    var nodes = Dictionary[level + 1];
                    for (int b = 0; b < mainNodes.Length; b++)
                    {
                        var criteria = mainNodes[b];
                        for (int i = 0; i < nodes.Count(); i++)
                        {
                            bool onlyRequired = false;

                            int startIndex = onlyRequired ? i + 1 : 0;
                            for (int a = startIndex; a < nodes.Count(); a++)
                            {
                                var x = nodes[i];
                                var y = nodes[a];

                                NodeRelation relation = new NodeRelation(criteria, x, y, 0);
                                relations.Add(relation);
                                relation.Changed += RelationValue_Changed;
                            }
                        }
                    }

                }
            }

            relations.Clear();
            foreach (var node in Hierarchy)
            {
                var neigbors = Hierarchy.Where(n => Enumerable.SequenceEqual(n.Criterias.Group, node.Criterias.Group));
                foreach (var criteria in node.Criterias.Group)
                {
                    foreach (var nodeNeighbor in neigbors)
                    {
                        if(!relations.Exists(r => r.Main == criteria && r.From == node && r.To == nodeNeighbor))
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
        private List<INodeRelation> GetRelationsNew()
        {
            List<INodeRelation> relations = new List<INodeRelation>();
            foreach (var node in Hierarchy)
            {
                var neigbors = Hierarchy.Where(n => Enumerable.SequenceEqual(n.Criterias.Group, node.Criterias.Group));
                foreach (var criteria in node.Criterias.Group)
                {
                    foreach (var nodeNeighbor in neigbors)
                    {
                        if (!relations.Exists(r => r.Main == criteria && r.From == node && r.To == nodeNeighbor))
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


        private void RelationValue_Changed(Relation<INode, INode> changedRelation)
        {
            RecountCoeffs();
            RelationValueChanged?.Invoke();
        }

        public Dictionary<int, INode[]> Dictionary { get; private set; }
        public IEnumerable<INode> Best(int level)
        {
            var max = Dictionary[level].Select(c => c.Coefficient).Max();
            return Dictionary[level].Where(n => n.Coefficient == max);
        }


        public INodeRelation[] RelationsAll { get; private set; }
        public INodeRelation[] RelationsRequired { get; private set; }
        public IEnumerable<IGrouping<INode, INodeRelation>> RelationsGroupedMain(INode node) => RelationsAll.Where(g => g.Main == node).GroupBy(r => r.From).ToArray();
        
        public IMatrixRelations GetMtxRelations(INode node) => new MtxRelations(this,node);


        private void RecountCoeffs()
        {
            foreach (var group in GroupedByLevel)
            {
                int level = group.Key;
                if (level > 0)
                {
                    foreach (var node in group)
                    {
                        var coeffs = new MtxCoeffs(this, node);
                        node.Coefficient = coeffs.Get(node);
                    }
                }
                else
                {
                    MainGoal.Coefficient = 1;
                }
            }
        }

        public void ClearRelations(INode node)
        {
            foreach (var relation in RelationsRequired.Where(r => r.Main == node))
            {
                relation.Value = 0;
            }
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
            if (RelationsAll.ToList().Find(r => r.Main == main && r.From == from && r.To == to) is INodeRelation relation)
            {
                relation.Value = value;
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
        Console.WriteLine(index);

            if (index + inc >= RelationsRequired.Length)
                return RelationsRequired.First();
            if (index + inc< 0)
                return RelationsRequired.Last();

            return RelationsRequired[index + inc];
        }
        public INodeRelation NextRequiredRel(INodeRelation from)
        {
            if (GetMtxRelations(from.Main).Consistency.IsCorrect())
                return GetRequiredRel(from, 1);
            else if (from == RelationsRequired.ToList().FindLast(r => r.Main == from.Main))
                return RelationsRequired.ToList().Find(r => r.Main == from.Main);
            else
                return GetRequiredRel(from, 1);
        }
        public INodeRelation PrevRequiredRel(INodeRelation from) => GetRequiredRel(from, -1);


        private IEnumerable<IMatrixRelations> PossibleMatrixesForRel(INodeRelation relation)
        {
            List<IMatrixRelations> mtxes = new List<IMatrixRelations>();
            for (double i = -9; i <= 9; i += 2)
            {
                if (i == -1)
                    continue;

                double value = i < 0 ? 1 / Math.Abs(i) : i;

                IMatrixRelations source = GetMtxRelations(relation.Main);
                source.Change(relation.Main, relation.To, value);

                mtxes.Add(source);
            }
            return mtxes;
        }
    }
}
