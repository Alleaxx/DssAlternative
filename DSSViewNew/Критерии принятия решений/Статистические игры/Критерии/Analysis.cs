using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSLib;

namespace DSSView
{
    public class StatGameAnalysis
    {
        public event Action OnCriteriasUpdated;

        public IEnumerable<ICriteria> Criterias { get; private set; }
        public IEnumerable<IOption> Options { get; private set; }


        public IEnumerable<Alternative> BestAlternatives => Priorities.Where(c => c.Rating == MaxRank).Select(f => f.Alternative);
        private double MaxRank => Priorities.Max(a => a.Rating);
        public CriteriasPriorAlternative[] Priorities { get; private set; }


        public StatGameAnalysis(IStatGame game)
        {
            SetCriterias(game);
            game.OnInfoUpdated += Matrix_Changed;

            if(game is IMatrixChance<Alternative, Case, double> matrix)
            {
                matrix.Info.ChancesChanged += Matrix_Changed;
                matrix.RowChanged += a => Matrix_Changed();
                matrix.ColChanged += c => Matrix_Changed();
                matrix.ValuesChanged += c => Matrix_Changed();
            }
        }
        private void SetCriterias(IStatGame game)
        {
            Criterias = new ICriteria[]
            {
                new CriteriaWald(game),
                new CriteriaMinMax(game),
                new CriteriaMaxMax(game),
                new CriteriaLaplas(game),
                new CriteriaBaies(game),
                new CriteriaSavige(game),
                new CriteriaGurvits(game),
                new CriteriaLeman(game),
                new CriteriaMulti(game),
                new CriteriaGerr(game),
            };
            Options = Criterias.SelectMany(c => c.Options).ToList();
            Update();
        }

        private void Matrix_Changed()
        {
            Update();
            OnCriteriasUpdated?.Invoke();
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
            Priorities = Priorities.OrderByDescending(p => p.Rating).ToArray();

        }
        private Dictionary<Alternative, List<ICriteria>> GetCriteriasForAlternatives()
        {
            //Расчет приоритеттов
            Dictionary<Alternative, List<ICriteria>> PriorityAlternatives = new Dictionary<Alternative, List<ICriteria>>();
            foreach (ICriteria criteria in Criterias)
            {
                var alternatives = criteria.BestAlternatives.ToArray();
                for (int i = 0; i < alternatives.Length; i++)
                {
                    if (!PriorityAlternatives.ContainsKey(alternatives[i]))
                    {
                        PriorityAlternatives.Add(alternatives[i], new List<ICriteria>() { });
                    }

                    PriorityAlternatives[alternatives[i]].Add(criteria);
                }
            }
            return PriorityAlternatives;
        }
    }

    //Суммарный рейтинг альтернативы по всем критериям
    public class CriteriasPriorAlternative
    {
        public Alternative Alternative { get; set; }
        public ICriteria[] Criterias { get; set; }

        //Ранг для альтернативы
        public double Rating => Criterias.Sum(c => c.Rank);

        public CriteriasPriorAlternative(Alternative alternative, ICriteria[] criterias)
        {
            Alternative = alternative;
            Criterias = criterias;
        }
    }
}
