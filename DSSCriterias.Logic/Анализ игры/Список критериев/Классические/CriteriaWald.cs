using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using DSSCriterias.Logic.Extensions;
namespace DSSCriterias.Logic.Criterias
{
    public class CriteriaWald : Criteria
    {
        public CriteriaWald(IStatGame game) : base(game)
        {
            Name = "Критерий Вальда";
            Situation.Goal = StateGoal.Get(Goals.MinRisc);
            Situation.Usage = StateUsage.Get(Usages.OneTime);
            Situation.Chances = StateChances.Unknown();
            Description = "Критерий крайнего пессимизма. Наиболее осторожный критерий, будто игра ведется не с природой, а с противником. Ориентирован на наихудшие условия, только среди которых отыскивается наилучший и теперь уже гарантированный результат.";
            DecizionAlgoritm = "- Найти наихудшие варианты исхода по каждой альтернативе\n- Из них выбрать наилучший вариант выбора альтернативы";
        }
        protected override void Count()
        {
            List<double> mins = new List<double>((int)Rows);
            for (int i = 0; i < Rows; i++)
            {
                mins.Add(Arr.MinFromRow(i));
            }
            SetResult(mins.Max(), mins);

            AddStep("Минимумы", mins);
            AddStep("Максимум", Result);
        }
    }
}
