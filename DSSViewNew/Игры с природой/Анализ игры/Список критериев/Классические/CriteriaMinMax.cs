using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSView.Extensions;
namespace DSSView.Criterias
{
    public class CriteriaMinMax : Criteria
    {
        public CriteriaMinMax(IStatGame game) : base(game)
        {
            Name = "Критерий Минимакса";
            Situation.Goal = StateGoal.Get(Goals.MinRisc);
            Situation.Usage = StateUsage.Get(Usages.Couple);
            Situation.Chances = StateChances.Unknown();
            Description = "...";
            DecizionAlgoritm = "- Найти наилучшие варианты исхода по каждой альтернативе\n- Из них выбрать наихудший вариант выбора альтернативы";
        }
        protected override void Count()
        {
            List<double> maxes = new List<double>((int)Rows);
            for (int i = 0; i < Rows; i++)
            {
                maxes.Add(Arr.MaxFromRow(i));
            }
            SetResult(maxes.Min(), maxes);

            AddStep("Максимумы", maxes);
            AddStep("Минимум", Result);
        }
    }
}
