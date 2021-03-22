using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    interface INode
    {
        INode Main { get; }
        int Level { get; }
    }


    public class NodeList : List<Node>
    {
        public event Action<NodeList, Node> Added;
        public event Action<NodeList, Node> Removed;

        public void Add(params Node[] criterias)
        {
            foreach (var newAlt in criterias)
            {
                base.Add(newAlt);
                Added?.Invoke(this, newAlt);
            }
        }
        public void Remove(params Node[] criterias)
        {
            foreach (var oldCrit in criterias)
            {
                base.Remove(oldCrit);
                Removed?.Invoke(this, oldCrit);
            }
        }
    }

    public class Node : Alternative
    {
        public event Action<Node> StructureChanged;

        public event Action<Node> Removed;
        public event Action<Node> Added;

        public event Action<Node> CoefficientUpdated;


        public override string ToString() => $"{Name} - {Level} ур";

        public double Coefficient
        {
            get => coefficient;
            set
            {
                coefficient = value;
                OnPropertyChanged();
            }
        }
        protected double coefficient;


        public virtual Problem Main { get; private set; }

        public NodeList Inner { get; private set; } = new NodeList();
        public Dictionary<int, NodeList> Dictionary { get; private set; } = new Dictionary<int, NodeList>();
        public Node[] AllCriterias
        {
            get
            {
                List<Node> allNodes = new List<Node>();
                foreach (var item in Dictionary)
                {
                    allNodes.AddRange(item.Value);
                }
                return allNodes.ToArray();
            }
        }


        public int Level { get; private set; }
        public bool Deepest => Inner.Count == 0;


        public List<NodeRelation> GetReqRelationsThis()
        {
            List<NodeRelation> relations = new List<NodeRelation>();
            for (int i = 0; i < Inner.Count; i++)
            {
                for (int a = i + 1; a < Inner.Count; a++)
                {
                    relations.Add(Relations.Find(r => r.From == Inner[i] && r.To == Inner[a]));
                }
            }
            return relations;
        }
        public List<NodeRelation> FillRelations => GetReqRelationsThis();
        public List<NodeRelation> FillRelationsAll => GetReqRelationsInner();

        public List<NodeRelation> GetReqRelationsInner()
        {
            List<NodeRelation> relations = new List<NodeRelation>();
            relations.AddRange(GetReqRelationsThis());
            foreach (var critGroup in Dictionary)
            {
                if(critGroup.Key != 0)
                {
                    foreach (var item in critGroup.Value)
                    {
                        relations.AddRange(item.GetReqRelationsThis());
                    }
                }
            }
            return relations;
        }

        public int ReqRelationsCount => (int)Math.Ceiling((double)(Inner.Count * Inner.Count / 3));




        public List<NodeRelation> Relations { get; private set; } = new List<NodeRelation>();
        public IGrouping<Node, NodeRelation>[] Grouped => Relations.GroupBy(r => r.From).ToArray();
        public MatrixAHP Matrix => new MatrixAHP(Grouped);


        protected void UpdateCoefficientEvent()
        {
            OnPropertyChanged(nameof(Matrix));
            CoefficientUpdated?.Invoke(this);
        }


        public virtual void UpdateCoeffs()
        {
            MatrixAHP matrix = Matrix;
            for (int i = 0; i < Inner.Count; i++)
            {
                Inner[i].Coefficient = matrix.Coeffiients[i];
            }
            CoefficientUpdated?.Invoke(this);
        }
        private void Relation_ValueChanged(NodeRelation obj)
        {
            Main.UpdateCoeffs();
        }

        
        public void AddInner(params Node[] criteriasAlpha)
        {
            foreach (var criteriaAlpha in criteriasAlpha)
            {
                foreach (var thisLevelCriteria in Main.Dictionary[criteriaAlpha.Level - 1])
                {
                    thisLevelCriteria.Add(criteriaAlpha);
                }

                //if(Main.Dictionary.ContainsKey(criteriaAlpha.Level + 1))
                //{
                //    foreach (var lowerLevelCriteria in Main.Dictionary[criteriaAlpha.Level + 1])
                //    {
                //        criteriaAlpha.Add(lowerLevelCriteria);
                //    }
                //}


                //foreach (var thisLevelCriteria in Main.Dictionary[criteriaAlpha.Level])
                //{
                //    foreach (var crAlpha in criteriasAlpha)
                //    {
                //        crAlpha.AddToStructureDictionary(thisLevelCriteria);
                //    }
                //    thisLevelCriteria.AddToStructureDictionary(criteriaAlpha);
                //}
            }
        }
        public void RemoveThis()
        {
            int level = Level;
            for (int i = 0; i < Level; i++)
            {
                var currentList = Main.Dictionary[i];
                foreach (var criteria in currentList)
                {
                    criteria.Dictionary[level - i].Remove(this);
                    criteria.Remove(this);
                    criteria.StructureChanged?.Invoke(criteria);
                    if (criteria.Dictionary[level - i].Count == 0)
                        criteria.Dictionary.Remove(level - i);
                }
            }
            Removed?.Invoke(this);
        }
        


        private void Add(NodeRelation newRel)
        {
            Relations.Add(newRel);
            newRel.Changed += Relation_ValueChanged;
        }
        private void Add(params Node[] newCriterias)
        {
            AddToStructureDictionary(newCriterias);

            foreach (var newCriteria in newCriterias)
            {
                Inner.Add(newCriteria);
                foreach (var existCriteria in Inner)
                {
                    Add(new NodeRelation(this, newCriteria, existCriteria, 1));
                    if (existCriteria != newCriteria)
                    {
                        Add(new NodeRelation(this, existCriteria, newCriteria, 1));
                        Relations[Relations.Count - 1].Mirrored = Relations[Relations.Count - 2];
                        Relations[Relations.Count - 2].Mirrored = Relations[Relations.Count - 1];
                    }
                }
                newCriteria.Added?.Invoke(newCriteria);
            }
            Main.UpdateCoeffs();
        }

        private void Remove(NodeRelation delRel)
        {
            Relations.Remove(delRel);
            delRel.Changed -= Relation_ValueChanged;
        }
        private void Remove(params Node[] delCriterias)
        {
            foreach (var delCriteria in delCriterias)
            {
                Inner.Remove(delCriteria);
                var deleteRelations = Relations.Where(r => r.To == delCriteria || r.From == delCriteria).ToArray();
                foreach (var item in deleteRelations)
                {
                    Remove(item);
                }
            }
            Main.UpdateCoeffs();
        }


        private void AddToStructureDictionary(params Node[] newCriterias)
        {
            var groupedLevel = newCriterias.GroupBy(c => c.Level);
            foreach (var group in groupedLevel)
            {
                int level = group.Key;
                for (int i = 0; i <= Level; i++)
                {
                    var currentList = Main.Dictionary[i];
                    foreach (var criteria in currentList)
                    {
                        if (!criteria.Dictionary.ContainsKey(level - i))
                            criteria.Dictionary.Add(level - i, new NodeList());

                        foreach (var criteriaAdd in group)
                        {
                            if (!criteria.Dictionary[level - i].Contains(criteriaAdd))
                            {
                                criteria.Dictionary[level - i].Add(criteriaAdd);
                                criteria.StructureChanged?.Invoke(criteria);
                            }
                        }
                    }
                }
            }
        }



        protected Node(int level, string name) : this(null, level, name)
        {

        }
        public Node(Problem main, NodeProject project) : this(main, project.Level, project.Name)
        {
            Description = project.Description;
            Coefficient = project.Coefficient;
        }
        public Node(Problem main, NodeHierarcy project) : this(main, project.Level, project.Name)
        {
            Description = project.Description;
        }
        public Node(Problem main,int level,string name)
        {
            Main = main;
            Level = level;
            Name = name;

            
            Dictionary.Add(0, new NodeList() { });
        }

        public virtual NodeProject GetSaveVersionAlpha()
        {
            MatrixAHP matrix = Matrix;
            int sizeX = matrix.Array.GetLength(0);
            int sizeY = matrix.Array.GetLength(1);

            double[][] values2 = new double[sizeX][];
            for (int i = 0; i < values2.Length; i++)
            {
                values2[i] = new double[sizeY];
            }
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    values2[x][y] = matrix.Array[x, y];
                }
            }

            return new NodeProject()
            {
                Name = Name,
                Description = Description,
                Coefficient = Coefficient,
                Level = Level,
                Values = values2
            };
        }

        public void LoadValues(double[][] values)
        {
            for (int x = 0; x < Grouped.Length; x++)
            {
                for (int y = 0; y < Grouped[x].Count(); y++)
                {
                    Grouped[x].ElementAt(y).Value = values[x][y];
                }
            }
        }

        


        public static void SetRelationBetween(Node main, Node from, Node to, double value)
        {
            if(main.Relations.Find(r => r.From == from && r.To == to) is NodeRelation relation)
            {
                relation.Value = value;
            }
        }
    }
}
