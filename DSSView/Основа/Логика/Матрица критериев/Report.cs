using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class ReportCriterias
    {
        public event Action CriteriasUpdated;

        public ICriteria[] Criterias { get; set; }
        public IOption[] Options { get; set; }


        public Alternative[] BestAlternatives => Priorities.Where(c => c.Rank == Priorities.Select(a => a.Rank).Max()).Select(f => f.Alternative).ToArray();
        public CriteriasPriorAlternative[] Priorities { get; set; }


        public ReportCriterias(IMatrixChance<Alternative,Case,double> matrix)
        {
            SetCriterias(matrix);
            matrix.Info.ChancesChanged += Matrix_Changed;
            matrix.RowChanged += a => Matrix_Changed();
            matrix.ColChanged += c => Matrix_Changed();
            matrix.ValuesChanged += c => Matrix_Changed();
        }
        private void SetCriterias(IMatrixChance<Alternative, Case, double> matrix)
        {
            Criterias = new ICriteria[]
            {
                new CriteriaWald(matrix),
                new CriteriaMinMax(matrix),
                new CriteriaMaxMax(matrix),
                new CriteriaLaplas(matrix),
                new CriteriaBaies(matrix),
                new CriteriaSavige(matrix),
                new CriteriaGurvits(matrix),
                new CriteriaLeman(matrix),
                new CriteriaMulti(matrix),
                new CriteriaGerr(matrix),
            };

            List<IOption> options = new List<IOption>();
            for (int i = 0; i < Criterias.Length; i++)
            {
                options.AddRange(Criterias[i].Options);
            }
            Options = options.ToArray();

            Update();
        }

        private void Matrix_Changed()
        {
            Update();
            CriteriasUpdated?.Invoke();
        }
        private void Update()
        {
            foreach (ICriteria criteria in Criterias)
            {
                criteria.Update();
            }
            SetAltCritPriorities();
        }
        private void SetAltCritPriorities()
        {
            Dictionary<Alternative, List<ICriteria>> dictionary = GetCriteriasForAlternatives();
            Priorities = new CriteriasPriorAlternative[dictionary.Count];
            for (int i = 0; i < dictionary.Count; i++)
            {
                Priorities[i] = new CriteriasPriorAlternative(dictionary.ElementAt(i).Key, dictionary.ElementAt(i).Value.ToArray());
            }
            Priorities = Priorities.OrderBy(p => p.Rank).Reverse().ToArray();

        }
        private Dictionary<Alternative, List<ICriteria>> GetCriteriasForAlternatives()
        {
            //Расчет приоритеттов
            Dictionary<Alternative, List<ICriteria>> PriorityAlternatives = new Dictionary<Alternative, List<ICriteria>>();
            foreach (ICriteria criteria in Criterias)
            {
                Alternative[] alternatives = criteria.BestAlts;
                for (int i = 0; i < alternatives.Length; i++)
                {
                    if (!PriorityAlternatives.ContainsKey(alternatives[i]))
                        PriorityAlternatives.Add(alternatives[i], new List<ICriteria>() { criteria });
                    else
                        PriorityAlternatives[alternatives[i]].Add(criteria);
                }
            }
            return PriorityAlternatives;
        }
    }

    public class CriteriasPriorAlternative
    {
        public Alternative Alternative { get; set; }
        public ICriteria[] Criterias { get; set; }

        //Ранг для альтернативы
        public double Rank => Criterias.Select(c => c.Rank).Sum();

        public CriteriasPriorAlternative(Alternative alternative, ICriteria[] criterias)
        {
            Alternative = alternative;
            Criterias = criterias;
        }
    }

}
