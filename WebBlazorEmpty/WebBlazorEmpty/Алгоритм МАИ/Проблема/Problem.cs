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
        void SetRelationBetween(INode main, INode from, INode to, double value);
        IMatrix GetMatrix(INode node);

        //Для рейтинга критериев
        Dictionary<INode, INode[]> CriteriasFurther { get; }

        IGrouping<INode, INodeRelation>[] GetGrouped(INode node);

        INode[] Best(int level);

        IRelationsCorrecntess CorrectnessRels { get; set; }

        event Action RelationValueChanged;

        void ClearRelations();
        void ClearRelations(INode node);
        int IndexNode(INode node);

        INodeRelation FirstRequiredRelation(INode node);
        INodeRelation NextRequiredRel(INodeRelation from);
        INodeRelation PrevRequiredRel(INodeRelation from);


        Dictionary<INode, double> GetRatingFor(INode node);
        IEnumerable<IMatrix> PossibleMatrixesForRel(INodeRelation relation);
    }
    public class Problem : HierarchyN, IProblem
    {
        public event Action RelationValueChanged;

        public Problem(IEnumerable<INode> nodes) : base(nodes)
        {
            UpdateStructure();
            RecountCoeffs();
        }

        private void UpdateStructure()
        {
            SetFurtherCriterias();

            Dictionary = GetDictionary();

            CreateRelations();
        }

        private void SetFurtherCriterias()
        {
            CriteriasFurther = new Dictionary<INode, INode[]>();
            foreach (var mainNode in Hierarchy)
            {
                CriteriasFurther.Add(mainNode, Hierarchy.Where(n => n.Level == mainNode.Level + 1).ToArray());
            }
        }

        private void CreateRelations()
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

                                NodeRelation relationA = new NodeRelation(criteria, x, y, 0);
                                relations.Add(relationA);
                                relationA.Changed += RelationValue_Changed;
                            }


                        }
                    }

                }
            }
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


            RelationCriteriasGrouped = new Dictionary<INode, INodeRelation[]>();
            foreach (var node in Hierarchy)
            {
                RelationCriteriasGrouped.Add(node, RelationsAll.Where(r => r.Main == node).ToArray());
            }

            CorrectnessRels = new RelationsCorrectness(this);
        }
        private void RelationValue_Changed(Relation<INode, INode> changedRelation)
        {
            RecountCoeffs();
            RelationValueChanged?.Invoke();
        }

        public Dictionary<int, INode[]> Dictionary { get; set; }

        public INodeRelation[] RelationsAll { get; private set; }

        //Необходимые отношения для заполнения
        public INodeRelation[] RelationsRequired { get; private set; }

        //Для матриц и коэффициентов
        public IGrouping<INode, INodeRelation>[] GetGrouped(INode node) => RelationsAll.Where(g => g.Main == node).GroupBy(r => r.From).ToArray();
        public Dictionary<INode, INodeRelation[]> RelationCriteriasGrouped { get; private set; }

        //Для рейтинга критериев
        public Dictionary<INode, INode[]> CriteriasFurther { get; private set; }


        public IMatrix GetMatrix(INode node) => new Matrix(GetGrouped(node));

        public void RecountCoeffs()
        {
            foreach (var group in GroupedByLevel)
            {
                int level = group.Key;
                if (level > 0)
                {
                    var coeffs = Matrix.GetGlobalCoeffs(this, level - 1);
                    for (int i = 0; i < group.Count(); i++)
                    {
                        group.ElementAt(i).Coefficient = coeffs[i];
                    }
                }
                else
                {
                    MainGoal.Coefficient = 1;
                }
            }
        }


        public int IndexNode(INode node) => Dictionary[node.Level].ToList().IndexOf(node);
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

        public INode[] Best(int level)
        {
            var max = Dictionary[level].Select(c => c.Coefficient).Max();
            return Dictionary[level].Where(n => n.Coefficient == max).ToArray();
        }

        public IRelationsCorrecntess CorrectnessRels { get; set; }





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


        //Все возможные матрицы сравнения по указанному отношению
        public IEnumerable<IMatrix> PossibleMatrixesForRel(INodeRelation relation)
        {
            IMatrix source = GetMatrix(relation.Main);
            int x = IndexNode(relation.From);
            int y = IndexNode(relation.To);
            List<IMatrix> mtx = new List<IMatrix>();
            for (double i = -9; i <= 9; i += 2)
            {
                if (i == -1)
                    continue;

                double val = i < 0 ? 1 / Math.Abs(i) : i;
                IMatrix matrix = new Matrix(source.Array);
                matrix.Array[x, y] = val;
                matrix.Array[y, x] = 1 / val;
                mtx.Add(matrix);
            }
            return mtx;
        }




        public Dictionary<INode, double> GetRatingFor(INode node)
        {
            Dictionary<INode, double> dictionary = new Dictionary<INode, double>();
            foreach (var rel in RelationsRequired.Where(r => r.Main == node))
            {
                if(rel.Node != null)
                {
                    if (!dictionary.ContainsKey(rel.Node))
                    {
                        dictionary.Add(rel.Node, 0);
                    }
                    dictionary[rel.Node] += rel.Rating;
                }
            }
            return dictionary;
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
            if (index + inc < 0)
                return RelationsRequired.Last();

            return RelationsRequired[index + inc];
        }
        public INodeRelation NextRequiredRel(INodeRelation from)
        {
            if (GetMatrix(from.Main).Consistency.IsCorrect())
                return GetRequiredRel(from, 1);
            else if (from == RelationsRequired.ToList().FindLast(r => r.Main == from.Main))
                return RelationsRequired.ToList().Find(r => r.Main == from.Main);
            else
                return GetRequiredRel(from, 1);
        }
        public INodeRelation PrevRequiredRel(INodeRelation from) => GetRequiredRel(from, -1);
    }
}
