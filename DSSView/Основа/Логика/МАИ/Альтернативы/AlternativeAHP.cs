using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class AlternativeAHP : Alternative
    {
        public event Action<AlternativeAHPRelation> Changed;

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

        public List<AlternativeAHPRelation> Relations { get; set; } = new List<AlternativeAHPRelation>();
        public List<AlternativeCriteriaGroup> Groups
        {
            get
            {
                var list = new List<AlternativeCriteriaGroup>();
                var grouped = Relations.GroupBy(r => r.Criteria);
                foreach (var item in grouped)
                {
                    list.Add(new AlternativeCriteriaGroup(this, item.Key));
                }
                return list;
            }
        }

        private AlternativeAHP[] Alternatives => Relations.GroupBy(r => r.To).Select(g => g.Key).ToArray();
        private CriteriaAHP[] Criterias => Relations.GroupBy(r => r.Criteria).Select(g => g.Key).ToArray();


        private void Add(AlternativeAHPRelation newRel)
        {
            Relations.Add(newRel);
            newRel.Changed += NewRel_Changed;
        }

        private void NewRel_Changed(AlternativeAHPRelation obj)
        {
            AlternativeAHPRelation rel = obj as AlternativeAHPRelation;
            obj.To.Relations.Find(r => rel.Criteria == r.Criteria && r.To == this).SetValueMirror(1 / obj.Value);
            Changed?.Invoke(rel);
        }

        //Добавить в отношения новую альтернативу по всем критериям
        public void Add(AlternativeAHP newAlt)
        {
            CriteriaAHP[] crits = Criterias;
            foreach (var crit in crits)
            {
                Add(new AlternativeAHPRelation(crit, this, newAlt, 1));
            }
        }
        //Добавить в отношения новый критерий для всех альтернатив
        public void Add(CriteriaAHP newCrit)
        {
            AlternativeAHP[] alts = Alternatives;
            foreach (var alt in alts)
            {
                Add(new AlternativeAHPRelation(newCrit, this, alt, 1));
            }
        }


        private void Remove(AlternativeAHPRelation delRel)
        {
            Relations.Remove(delRel);
        }
        //Убрать из отношений выбранную альтернативу по всем критериям
        public void Remove(AlternativeAHP newRel)
        {
            Relations.Where(r => r.To == newRel).ToList().ForEach(r => r.Changed -= NewRel_Changed);
            Relations.RemoveAll(r => r.To == newRel);
        }
        //Убрать из отношений критерий для всех альтернатив
        public void Remove(CriteriaAHP criteria)
        {
            Relations.Where(r => r.Criteria == criteria).ToList().ForEach(r => r.Changed -= NewRel_Changed);
            Relations.RemoveAll(r => r.Criteria == criteria);
        }

        

        public AlternativeAHP(Problem problem,string name)
        {
            Name = name;
            for (int a = 0; a < problem.Criterias.Count; a++)
            {
                for (int i = 0; i < problem.Alternatives.Count; i++)
                {
                    Add(new AlternativeAHPRelation(problem.Criterias[a],this, problem.Alternatives[i], 1));
                }
            }
            for (int a = 0; a < problem.Criterias.Count; a++)
            {
                Add(new AlternativeAHPRelation(problem.Criterias[a],this, this, 1));
            }
        }

        public AlternativeAHP(AlternativeAHPProject from)
        {
            Name = from.Name;
            Description = from.Description;
            coefficient = from.Coefficient;

            foreach (var item in from.Relations)
            {
                Add(new AlternativeAHPRelation(item));
            }
        }
        public void InitAfterLoading(AlternativeAHP[] alts, CriteriaAHP[] crits)
        {
            int total = 0;
            for (int c = 0; c < crits.Length; c++)
            {
                for (int a = 0; a < alts.Length; a++)
                {
                    Relations[total].SetFromTo(crits[c], this, alts[a]);
                    total++;
                }
            }
            Relations.RemoveAll(r => !r.Inited);
        }
        public AlternativeAHPProject GetSaveVersion()
        {
            return new AlternativeAHPProject()
            {
                Name = Name,
                Description = Description,
                Coefficient = coefficient,
                Relations = Relations.Select(c => c.GetSaveVersion()).ToArray()
            };
        }
    }

    
    public class AlternativeCriteriaGroup
    {
        public CriteriaAHP Criteria { get; set; }
        public AlternativeAHP Alternative { get; set; }
        public List<AlternativeAHPRelation> Relations { get; set; }

        public AlternativeCriteriaGroup(AlternativeAHP alt, CriteriaAHP crit)
        {
            Criteria = crit;
            Alternative = alt;
            Relations = alt.Relations.Where(r => r.Criteria == crit).ToList();
        }
    }


    public class AlternativeAHPRelation : Relation<AlternativeAHP, CriteriaAHP>
    {
        public new event Action<AlternativeAHPRelation> Changed;

        public override string ToString()
        {
            if (Value > 1)
                return $"'{From}' лучше '{To}' в {Value} раз по {Criteria}";
            else if (Value == 1)
                return $"'{From}' и '{To}' равны по {Criteria}";
            else
                return $"'{From}' хуже '{To}' в {1 / Value} раз по {Criteria}";
        }

        public CriteriaAHP Criteria => Main;



        public AlternativeAHPRelation(CriteriaAHP criteria,AlternativeAHP from, AlternativeAHP to, double val) : base(criteria,from, to, val)
        {
            base.Changed += AlternativeAHPRelation_Changed;
        }

        private void AlternativeAHPRelation_Changed(Relation<AlternativeAHP, CriteriaAHP> obj)
        {
            Changed?.Invoke(this);
        }

        public AlternativeAHPRelation(AlternativeAHPRelationProject from) : base(null, null, null, from.Value)
        {
            base.Changed += AlternativeAHPRelation_Changed;
        }
        public AlternativeAHPRelationProject GetSaveVersion()
        {
            return new AlternativeAHPRelationProject()
            {
                Criteria = Criteria.Name,
                To = To.Name,
                Value = value
            };
        }

    }
}
