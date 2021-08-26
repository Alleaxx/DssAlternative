using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class CriteriaLaplas : Criteria
    {
        public CriteriaLaplas(IStatGame game) : base(game)
        {
            Name = "Критерий Лапласа";
            ChancesRequired = false;
            Description = "...";
            DecizionAlgoritm = "- Найти среднее значение эффективности каждой альтернативы. Исходы считаются равновероятными\n- Выбрать максимальное значение и соотнести с альтернативой";
        }

        protected override void Count()
        {
            double[] averages = new double[(int)Rows];
            for (int r = 0; r < Rows; r++)
            {
                double sum = 0;
                for (int c = 0; c < Cols; c++)
                {
                    sum += Arr[r, c] / Cols;
                }
                averages[r] = sum;
            }
            SetResult(averages.Max(), averages);

            AddStep("Средние значения", averages);
            AddStep("Максимум", Result);
        }
    }
}
