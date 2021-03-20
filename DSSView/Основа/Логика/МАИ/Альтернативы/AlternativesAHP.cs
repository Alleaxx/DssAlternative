using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class AlternativesAHP : List<AlternativeAHP>
    {
        public event Action<AlternativesAHP, AlternativeAHP> Added;
        public event Action<AlternativesAHP, AlternativeAHP> Removed;
        public event Action<AlternativesAHP> CoeffsChanged;

        public CriteriaAHPGroup[] GroupCriterias { get; set; }

        public void Set(CriteriasAHP criterias)
        {
            criterias.Added += Criterias_Added;
            criterias.Removed += Criterias_Removed;
        }
        public void UpdateGroups()
        {
            if(Count > 0)
            {
                CriteriaAHP[] crits = this.First().Relations.GroupBy(r => r.Criteria).Select(gr => gr.Key).ToArray();
                GroupCriterias = crits.Select(c => new CriteriaAHPGroup(ToArray(), c)).ToArray();

                //по критериям
                double[][] test = GroupCriterias.Select(g => g.Matrix.Coeffiients).Reverse().ToArray();

                for (int a = 0; a < Count; a++)
                {
                    double coeff = 0;
                    string formula = $"alt[{a}] = ";
                    for (int c = 0; c < crits.Length; c++)
                    {
                        formula += $"{test[c][a]} * {crits[c].Coefficient} +";
                        coeff += test[c][a] * crits[c].Coefficient; 
                    }
                    this[a].Coefficient = coeff;
                }



                //foreach (AlternativeAHP alt in this)
                //{
                //    double coeff = 0;

                //    int counter = 0;
                //    foreach (CriteriaAHPGroup group in GroupCriterias)
                //    {
                //        CriteriaAHP crit = group.Criteria;
                //        coeff += crit.Coefficient * group.Matrix.RowAveragesFromNormalised[counter];
                //        counter++;
                //    }

                //    alt.Coefficient = coeff;
                //}

            }
        }

        private void Criterias_Added(CriteriasAHP crits, CriteriaAHP newCrit)
        {
            foreach (var alt in this)
            {
                alt.Add(newCrit);
            }
            UpdateGroups();
        }
        private void Criterias_Removed(CriteriasAHP crits, CriteriaAHP oldCrit)
        {
            foreach (var alt in this)
            {
                alt.Remove(oldCrit);
            }
            UpdateGroups();
        }



        public void Add(params AlternativeAHP[] alts)
        {
            foreach (var newAlt in alts)
            {
                foreach (var existAlt in this)
                {
                    existAlt.Add(newAlt);
                }
                base.Add(newAlt);
                UpdateGroups();
                Added?.Invoke(this, newAlt);
                newAlt.Changed += NewAlt_Changed;
            }
        }

        private void NewAlt_Changed(AlternativeAHPRelation obj)
        {
            UpdateGroups();
            CoeffsChanged?.Invoke(this);
        }

        public void AddSave(params AlternativeAHP[] alts)
        {
            foreach (var alt in alts)
            {
                base.Add(alt);
                Added?.Invoke(this, alt);
                alt.Changed += NewAlt_Changed;
            }
        }
        public void Remove(CriteriaAHP[] crits,params AlternativeAHP[] alts)
        {
            for (int i = 0; i < alts.Length; i++)
            {
                AlternativeAHP oldAlt = alts[i];
                foreach (var existAlt in this)
                {
                    existAlt.Remove(oldAlt);
                }
                base.Remove(oldAlt);
                UpdateGroups();
                Removed?.Invoke(this, oldAlt);
                oldAlt.Changed -= NewAlt_Changed;
            }
        }
    }

    
    public class CriteriaAHPGroup
    {
        public CriteriaAHP Criteria { get; set; }
        public List<AlternativeCriteriaGroup> Groups { get; set; } = new List<AlternativeCriteriaGroup>();
        

        public MatrixAHP Matrix { get; set; }
        public CriteriaAHPGroup(AlternativeAHP[] alts, CriteriaAHP crit)
        {
            Criteria = crit;
            foreach (var alt in alts)
            {
                Groups.Add(new AlternativeCriteriaGroup(alt, crit));
            }
            Matrix = new MatrixAHP(this);
        }
    }
}
