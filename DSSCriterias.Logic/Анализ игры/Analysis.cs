using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSLib;
using DSSCriterias.Logic.Criterias;

namespace DSSCriterias.Logic
{
    public class GameAnalysis : NotifyObj
    {
        public override string ToString()
        {
            return $"Анализ статистической игры: ({string.Join(";", BestAlternatives)})";
        }

        public bool IgnoreUsage
        {
            get => ignoreUsage;
            set
            {
                ignoreUsage = value;
                OnPropertyChanged();
                UpdateSummary();
            }
        }
        private bool ignoreUsage = false;
        //Критерии для анализа
        private IEnumerable<ICriteria> Criterias { get; set; }
        public IEnumerable<ICriteria> CriteriasConsider
        {
            get
            {
                if (!IgnoreUsage)
                {
                    return Criterias.Where(c => c.Rank.Rating > 0).OrderByDescending(c => c.Rank.Rating);
                }
                else
                {
                    return Criterias;
                }
            }
        }
        public ICriteria GetCriteria<T>() where T:ICriteria
        {
            return CriteriasConsider.First(c => c is T);
        }
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
            foreach (var option in Options)
            {
                option.OnValueChanged += Option_OnValueChanged;
            }
            UpdateSummary();
        }

        private void Option_OnValueChanged(double arg1, double arg2)
        {
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
            OnPropertyChanged(nameof(CriteriasConsider));
        }

        private double MaxRank => AlternativeRanks.Max(a => a.Rating);
        private IEnumerable<RankAlternative> GetAltRanks()
        {
            var bestAlts = CriteriasConsider.SelectMany(c => c.BestAlternatives).Distinct();
            var ranks = new List<RankAlternative>(bestAlts.Count());
            CreateRanks();
            SetRanksTotalRating();
            return ranks.OrderByDescending(r => r.Rating);

            void CreateRanks()
            {
                foreach (Alternative alt in bestAlts)
                {
                    var critsWhoChosedAlt = CriteriasConsider.Where(c => c.BestAlternatives.Contains(alt));
                    RankAlternative rank = new RankAlternative(alt, critsWhoChosedAlt, IgnoreUsage);
                    ranks.Add(rank);
                }
            }
            void SetRanksTotalRating()
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
