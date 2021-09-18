using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSLib;

namespace DSSView
{
    public class GameAnalysis : NotifyObj
    {
        public override string ToString() => $"Анализ статистической игры: ({string.Join(";",BestAlternatives)})";

        //Критерии для анализа
        public IEnumerable<ICriteria> Criterias { get; private set; }
        public IEnumerable<IOption> Options { get; private set; }

        //Результаты анализа
        public IEnumerable<Alternative> BestAlternatives { get; private set; }
        public IEnumerable<RankAlternative> AlternativeRanks { get; private set; }


        public GameAnalysis(IStatGame game)
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

            AlternativeRanks = GetAltRanks();
            BestAlternatives = AlternativeRanks.Where(c => c.Rating == MaxRank).Select(f => f.Alternative);
            
            UpdateProperties();
        }
        private void UpdateProperties()
        {
            OnPropertyChanged(nameof(BestAlternatives));
            OnPropertyChanged(nameof(AlternativeRanks));
        }

        private double MaxRank => AlternativeRanks.Max(a => a.Rating);
        private IEnumerable<RankAlternative> GetAltRanks()
        {
            var bestAlts = Criterias.SelectMany(c => c.BestAlternatives).Distinct();
            var ranks = new List<RankAlternative>(bestAlts.Count());
            CreateRanks();
            SetRanksRating();
            return ranks.OrderByDescending(r => r.Rating);

            void CreateRanks()
            {
                foreach (Alternative alt in bestAlts)
                {
                    var critsWhoChosedAlt = Criterias.Where(c => c.BestAlternatives.Contains(alt));
                    RankAlternative rank = new RankAlternative(alt, critsWhoChosedAlt);
                    ranks.Add(rank);
                }
            }
            void SetRanksRating()
            {
                double rankRating = ranks.Sum(r => r.Rating);
                foreach (var rank in ranks)
                {
                    rank.RatingTotal = rankRating;
                }
            }
        }
    }
}
