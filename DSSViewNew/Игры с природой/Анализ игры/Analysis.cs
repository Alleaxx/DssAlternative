using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSLib;

namespace DSSView
{
    public class StatGameAnalysis : NotifyObj
    {
        public override string ToString() => $"Анализ статистической игры: ({string.Join(";",BestAlternatives)})";

        //Критерии для анализа
        public IEnumerable<ICriteria> Criterias { get; private set; }
        public IEnumerable<IOption> Options { get; private set; }

        //Результаты анализа
        public IEnumerable<Alternative> BestAlternatives { get; private set; }
        public IEnumerable<RankAlternative> AlternativeRanks { get; private set; }


        public StatGameAnalysis(IStatGame game)
        {
            SetCriterias(game);
            game.OnInfoUpdated += UpdateSummary;
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
            Options = Criterias.SelectMany(c => c.Options);
            UpdateSummary();
        }

        //Обновить итоговые результаты
        private void UpdateSummary()
        {
            foreach (ICriteria criteria in Criterias)
            {
                criteria.Update();
            }

            AlternativeRanks = GetAlternativeRanks();
            BestAlternatives = AlternativeRanks.Where(c => c.Rating == MaxRank).Select(f => f.Alternative);

            OnPropertyChanged(nameof(BestAlternatives));
            OnPropertyChanged(nameof(AlternativeRanks));
        }
        private double MaxRank => AlternativeRanks.Max(a => a.Rating);
        private IEnumerable<RankAlternative> GetAlternativeRanks()
        {
            var alts = Criterias.SelectMany(c => c.BestAlternatives).Distinct();
            var ranks = new List<RankAlternative>(alts.Count());
            foreach (var alt in alts)
            {
                var chosenByCrits = Criterias.Where(c => c.BestAlternatives.Contains(alt));
                RankAlternative rank = new RankAlternative(alt, chosenByCrits);
                ranks.Add(rank);
            }
            double total = ranks.Sum(r => r.Rating);
            foreach (var rank in ranks)
            {
                rank.RatingTotal = total;
            }
            return ranks.OrderByDescending(r => r.Rating);
        }
    }
}
