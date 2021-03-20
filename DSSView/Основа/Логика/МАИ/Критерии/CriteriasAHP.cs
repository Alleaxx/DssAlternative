using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class CriteriasAHP : List<CriteriaAHP>
    {
        public event Action<CriteriasAHP> CoeffsChanged;
        public event Action<CriteriasAHP, CriteriaAHP> Added;
        public event Action<CriteriasAHP, CriteriaAHP> Removed;

        public MatrixAHP Matrix => new MatrixAHP(ToArray());
        

        private void Criteria_CoeffChanged(CriteriaAHPRelation obj)
        {
            UpdateCoeffs();
            CoeffsChanged?.Invoke(this);
        }
        private void UpdateCoeffs()
        {
            MatrixAHP matrix = Matrix;
            for (int i = 0; i < Count; i++)
            {
                this[i].Coefficient = matrix.Coeffiients[i];
            }
        }

        
        public void Add(Problem problem,params CriteriaAHP[] criterias)
        {
            foreach (var crit in criterias)
            {
                ForEach(existCrit => existCrit.Add(new CriteriaAHPRelation(problem, existCrit, crit, 1)));
                base.Add(crit);
                crit.CoeffChanged += Criteria_CoeffChanged;

                Added?.Invoke(this, crit);
                UpdateCoeffs();
            }
        } 
        public void AddSave(params CriteriaAHP[] criterias)
        {
            foreach (var crit in criterias)
            {
                base.Add(crit);
                crit.CoeffChanged += Criteria_CoeffChanged;
                Added?.Invoke(this, crit);
            }
        }


        public void Remove(params CriteriaAHP[] criterias)
        {
            for (int i = 0; i < criterias.Length; i++)
            {
                CriteriaAHP crit = criterias[i];
                ForEach(existCrit => existCrit.Remove(existCrit.Coeffs.Find(c => c.To == crit)));
                base.Remove(crit);

                Removed?.Invoke(this, crit);
                UpdateCoeffs();
            }
        }
    }
}
