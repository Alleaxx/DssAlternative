using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBlazorEmpty.AHP
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
    }
    public class Problem : HierarchyNodes, IProblem
    {
        public event Action RelationValueChanged;

        public Problem(IEnumerable<INode> nodes) : base(nodes)
        {
            UpdateStructure();
            RecountCoeffsBeta();
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
                if(level != MaxLevel)
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

                                NodeRelation relationA = new NodeRelation(criteria, x, y, 1);
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
            RecountCoeffsBeta();
            RelationValueChanged?.Invoke();
        }

        public Dictionary<int, INode[]> Dictionary { get; set; }

        public INodeRelation[] RelationsAll { get; private set; }

        //Для опросника
        public INodeRelation[] RelationsRequired { get; private set; }

        //Для матриц и коэффициентов
        public IGrouping<INode, INodeRelation>[] GetGrouped(INode node) => RelationsAll.Where(g => g.Main == node).GroupBy(r => r.From).ToArray();
        public Dictionary<INode, INodeRelation[]> RelationCriteriasGrouped { get; private set; }

        //Для рейтинга критериев
        public Dictionary<INode, INode[]> CriteriasFurther { get; private set; }

        
        public IMatrix GetMatrix(INode node) => new Matrix(GetGrouped(node));

        public void RecountCoeffsBeta()
        {
            foreach (var group in GroupedByLevel)
            {
                int level = group.Key;
                if(level > 0)
                {
                    var coeffs = Matrix.GetGlobalCoeffs(this, level - 1);
                    Console.WriteLine();
                    foreach (var item in group)
                    {
                        Console.Write(item);
                    }
                    Console.WriteLine();
                    foreach (var item in coeffs)
                    {
                        Console.Write(item);
                    }
                    Console.WriteLine();

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


        public INode[] Best(int level)
        {
            var max = Dictionary[level].Select(c => c.Coefficient).Max();
            return Dictionary[level].Where(n => n.Coefficient == max).ToArray();
        }

        public IRelationsCorrecntess CorrectnessRels { get; set; }



        public void SetRelationBetween(INode main, INode from, INode to, double value)
        {
            if(RelationsAll.ToList().Find(r => r.Main == main && r.From == from && r.To == to) is INodeRelation relation)
            {
                relation.Value = value;
            }
            else
            {
                Console.WriteLine("Отношение не найдено!");
            }
        }
    }
    

    public interface IRelationsCorrecntess
    {
        double Border { get; set; }

        bool AreRelationsCorrect { get; }

        IEnumerable<INodeRelation> RelationsNotConsistent { get; }


    }
    public class RelationsCorrectness : IRelationsCorrecntess
    {
        public IProblem Pr { get; set; }
        public RelationsCorrectness(IProblem problem)
        {
            Pr = problem;
        }

        

 
        public string[] Messages
        {
            get
            {
                List<string> msgs = new List<string>();
                if (!AreRelationsConsistenct)
                    msgs.Add("Отношения не согласованы.");
                if (!AreRelationsFilled)
                    msgs.Add("Некоторые отношения не согласованы");

                if (msgs.Count == 0)
                    msgs.Add("Всё в порядке");

                return msgs.ToArray();
            }
        }

        public bool AreRelationsCorrect => AreRelationsConsistenct && AreRelationsFilled;

        public bool AreRelationsFilled => RelationsUnknown.Count() == 0;
        public bool AreRelationsConsistenct => NodesNotConsistent.Count() == 0;


        //Список матриц
        public double Border { get; set; } = 0.15;


        //Список критериев, матрицы по которым не согласованы
        public IEnumerable<INode> NodesNotConsistent
        {
            get
            {
                Dictionary<INode, IMatrix> pairs = new Dictionary<INode, IMatrix>();
                foreach (var node in Pr.NodesWithRels)
                {
                    pairs.Add(node, Pr.GetMatrix(node));
                }
                var badNodes = pairs.Where(p => !p.Value.Consistency.IsCorrect(Border)).Select(p => p.Key);
                return badNodes;
            }
        }
        public IEnumerable<INodeRelation> RelationsNotConsistent
        {
            get
            {
                var nodes = NodesNotConsistent.ToArray();
                return Pr.RelationsRequired.Where(r => nodes.Contains(r.Main));
            }
        }
        public IEnumerable<INodeRelation> RelationsUnknown => Pr.RelationsRequired.Where(r => r.Unknown);
    }
}
