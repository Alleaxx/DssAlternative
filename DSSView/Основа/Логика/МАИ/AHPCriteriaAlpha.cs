using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    interface IAhpCriteriaAlpha
    {
        IAhpCriteriaAlpha Main { get; }
        int Level { get; }
    }

    public class AHPCriteriasAlpha : List<AHPCriteriaAlpha>
    {
        public event Action<AHPCriteriasAlpha, AHPCriteriaAlpha> Added;
        public event Action<AHPCriteriasAlpha, AHPCriteriaAlpha> Removed;

        public void Add(params AHPCriteriaAlpha[] criterias)
        {
            foreach (var newAlt in criterias)
            {
                base.Add(newAlt);
                newAlt.List = this;
                Added?.Invoke(this, newAlt);
            }
        }
        public void Remove(params AHPCriteriaAlpha[] criterias)
        {
            foreach (var oldCrit in criterias)
            {
                base.Remove(oldCrit);
                Removed?.Invoke(this, oldCrit);
            }
        }
    }
    public class AHPCriteriaAlpha : Alternative
    {
        public override string ToString() => $"Критерий уровня {Level} - {Name}";

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


        public AHPCriteriaAlpha Main { get; set; }
        public AHPCriteriasAlpha List { get; set; }
        public AHPCriteriasAlpha Inner { get; set; } = new AHPCriteriasAlpha();

        public int Level => Main == null ? 0 : 1;


        public List<AlphaRelation> Relations { get; set; } = new List<AlphaRelation>();
        public IGrouping<AHPCriteriaAlpha, AlphaRelation>[] Grouped => Relations.GroupBy(r => r.From).ToArray();
        public MatrixAHP Matrix => new MatrixAHP(Grouped);



        private void Add(AlphaRelation newRel)
        {
            Relations.Add(newRel);
        }
        public void Add(params AHPCriteriaAlpha[] newCriterias)
        {
            foreach (var newCriteria in newCriterias)
            {
                Inner.Add(newCriteria);
                foreach (var existCriteria in Inner)
                {
                    Add(new AlphaRelation(this, newCriteria, existCriteria, 1));
                    if(existCriteria != newCriteria)
                        Add(new AlphaRelation(this, existCriteria, newCriteria, 1));
                }
            }
        }
        private void Remove(AlphaRelation delRel)
        {
            Relations.Remove(delRel);
        }
        public void Remove(params AHPCriteriaAlpha[] delCriterias)
        {
            foreach (var delCriteria in delCriterias)
            {
                Inner.Remove(delCriteria);
                var deleteRelations = Relations.Where(r => r.To == delCriteria || r.From == delCriteria);
                foreach (var relation in deleteRelations)
                {
                    Remove(relation);
                }
            }
        }


        public AHPCriteriaAlpha(string name)
        {
            Name = name;
        }
        public AHPCriteriaAlpha(AHPCriteriaAlpha main,string name)
        {
            Main = main;
            Name = name;
        }
    }



    public class AlphaRelation : Relation<AHPCriteriaAlpha, AHPCriteriaAlpha>
    {
        public new event Action<AlphaRelation> Changed;
        public AlphaRelation(AHPCriteriaAlpha main, AHPCriteriaAlpha from, AHPCriteriaAlpha to, double val) : base(main, from, to, val)
        {
            base.Changed += AlphaRelation_Changed;
        }

        private void AlphaRelation_Changed(Relation<AHPCriteriaAlpha, AHPCriteriaAlpha> obj)
        {
            Changed?.Invoke(this);
        }
    }
}
