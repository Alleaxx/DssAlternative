using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSView.Extensions;
namespace DSSView.Criterias
{
    public class CriteriaMaxMax : Criteria
    {
        public CriteriaMaxMax(IStatGame game) : base(game)
        {
            Name = "Критерий Азартного игрока";
            Situation.Goal = StateGoal.Get(Goals.MaxProfit);
            Situation.Usage = StateUsage.Get(Usages.Couple);
            Situation.Chances = StateChances.Unknown();
            Description = "Критерий крайнего оптимизма или критерий максимакса. В данном случае ЛПР делает ставку на то, что произойдет наиболее благоприятный исход";
            DecizionAlgoritm = "- Найти наилучшие варианты исхода по каждой альтернативе\n- Из них выбрать наилучший вариант выбора альтернативы";
        }
        protected override void Count()
        {
            List<double> maxes = new List<double>((int)Rows);
            for (int i = 0; i < Rows; i++)
            {
                maxes.Add(Arr.MaxFromRow(i));
            }
            SetResult(maxes.Max(), maxes);

            AddStep("Максимумы", maxes);
            AddStep("Максимум", Result);
        }
    }
}
