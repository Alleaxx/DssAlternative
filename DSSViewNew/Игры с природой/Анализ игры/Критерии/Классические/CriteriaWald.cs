using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class CriteriaWald : Criteria
    {
        public CriteriaWald(IStatGame game) : base(game)
        {
            Name = "Критерий Вальда";
            Description = "Критерий крайнего пессимизма. Наиболее осторожный критерий. Ориентирован на наихудшие условия, только среди которых отыскивается наилучший и теперь уже гарантированный результат.";
            DecizionAlgoritm = "- Найти наихудшие варианты исхода по каждой альтернативе\n- Из них выбрать наилучший вариант выбора альтернативы";
        }
        protected override void Count()
        {
            List<double> mins = new List<double>();
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
