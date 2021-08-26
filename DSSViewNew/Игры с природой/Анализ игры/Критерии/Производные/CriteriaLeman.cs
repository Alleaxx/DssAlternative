using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class CriteriaLeman : Criteria
    {
        public IOption LemanCoeff { get; set; } = new Option("Коэффициент доверия к информации", 0.6, 0, 1);
        public CriteriaLeman(IStatGame game) : base(game)
        {
            Name = "Критерий Ходжа-Лемана";
            Type = "Производный";
            Description = "Вносится фактор субъективности в виде коэффициента доверия к информации\n- при высоком доверии доминирует критерий Баеса-Лапласа\n- при низком доверии доминирует МиниМаксный критерий";
            DecizionAlgoritm = "- Расчитать среднее значение эффективности по альтернативам\n- Рассчитать минимальное значение эффективности по альтернативам\n- Составить вектор по альтернативам: среднее значение умножается на коэффициент, минимальное на (1-коэффициент)\n- Из вектора выбирается максимальное значение и соотносится с альтернативой";
            ChancesRequired = false;

            AddOption(LemanCoeff);
        }

        protected override void Count()
        {
            double[] coeff = new double[(int)Rows];

            for (int r = 0; r < Rows; r++)
            {
                double min = Arr[r, 0];
                double sum = 0;
                for (int c = 0; c < Cols; c++)
                {
                    if (Arr[r, c] < min)
                        min = Arr[r, c];
                    sum += Arr[r, c] * ChanceFor(c);
                }
                coeff[r] = (sum * LemanCoeff.Value) + ((1 - LemanCoeff.Value) * min);
            }
            SetResult(coeff.Max(), coeff);

            AddStep("Вектор Ходжа-Лемана", coeff);
            AddStep("Максимум", Result);
        }
    }
}
