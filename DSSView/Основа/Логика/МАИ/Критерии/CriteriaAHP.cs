using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DSSView
{
    public interface ICriteriaAHP
    {

    }
    public class CriteriaAHP : Alternative, ICriteriaAHP
    {
        public event Action<CriteriaAHPRelation> CoeffChanged;
        public override string ToString() => Name;

        public double Coefficient
        {
            get => coefficient;
            set
            {
                coefficient = value;
                OnPropertyChanged();
            }
        }
        private double coefficient;


        public List<CriteriaAHPRelation> Coeffs { get; set; } = new List<CriteriaAHPRelation>();
        public void Add(CriteriaAHPRelation newRel)
        {
            Coeffs.Add(newRel);
            newRel.Changed += NewRel_Changed;
        }
        private void NewRel_Changed(CriteriaAHPRelation obj)
        {
            CriteriaAHPRelation other = obj.To.Coeffs.Find(c => c.To == this);
            other.SetValueMirror(1 / obj.Value);
            CoeffChanged?.Invoke(obj);
        }
        public void Remove(CriteriaAHPRelation delRel)
        {
            Coeffs.Remove(delRel);
            delRel.Changed -= NewRel_Changed;
        }


        public CriteriaAHP(Problem problem,string name)
        {
            Name = name;
            for (int i = 0; i < problem.Criterias.Count; i++)
            {
                Add(new CriteriaAHPRelation(problem,this, problem.Criterias[i], 1));
            }
            Add(new CriteriaAHPRelation(problem,this, this, 1));
        }
        public CriteriaAHP(CriteriaAHPProject from)
        {
            Name = from.Name;
            Description = from.Description;
            coefficient = from.Coefficient;
            foreach (var item in from.Coeffs)
            {
                Add(new CriteriaAHPRelation(item));
            }
        }
        public void InitAfterLoading(Problem problem,CriteriaAHP[] crits)
        {
            for (int i = 0; i < Coeffs.Count; i++)
            {
                Coeffs[i].SetFromTo(problem, this, crits[i]);
            }
        }
        public CriteriaAHPProject GetSaveVersion()
        {
            return new CriteriaAHPProject()
            {
                Name = Name,
                Description = Description,
                Coefficient = coefficient,
                Coeffs = Coeffs.Select(c => c.GetSaveVersion()).ToArray()
            };
        }
    }



    public class CriteriaAHPRelation : Relation<CriteriaAHP, Problem>
    {
        public new event Action<CriteriaAHPRelation> Changed;

        public override string ToString()
        {
            if (Value > 1)
                return $"'{From}' лучше '{To}' в {Value} раз";
            else if (Value == 1)
                return $"'{From}' и '{To}' равны";
            else
                return $"'{From}' хуже '{To}' в {1 / Value} раз";
        }

        public CriteriaAHPRelation(Problem problem,CriteriaAHP from, CriteriaAHP to, double val) : base(problem,from, to, val)
        {
            base.Changed += CriteriaAHPRelation_Changed;
        }

        private void CriteriaAHPRelation_Changed(Relation<CriteriaAHP, Problem> obj)
        {
            Changed?.Invoke(this);
        }

        public CriteriaAHPRelation(CriteriaAHPRelationProject from) : base(null,null,null, from.Value)
        {
            base.Changed += CriteriaAHPRelation_Changed;
        }
        public CriteriaAHPRelationProject GetSaveVersion()
        {
            return new CriteriaAHPRelationProject()
            {
                To = To.Name,
                Value = value
            };
        }
    }
}
