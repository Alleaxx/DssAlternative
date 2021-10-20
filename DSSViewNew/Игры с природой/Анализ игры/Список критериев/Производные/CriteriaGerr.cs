using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSView.Extensions;
namespace DSSView.Criterias
{
    public class CriteriaGerr : Criteria
    {
        public CriteriaGerr(IStatGame game) : base(game)
        {
            Name = "Критерий Гермейера";
            Type = "Производный";
            Description = "Ориентирован на величину потерь";
            DecizionAlgoritm = "- Определить минимальное значение по альтернативе\n- Определить максимальное значение по альтернативе\n- Составляется вектор из минимального и максимального значения альтернатив: максимальное умножается на коэффициент, а минимальное на (1-коэффициент)\n- Из вектора выбирается максимальное значение";

            Situation.Goal = StateGoal.Get(Goals.RiscAllowed);
            Situation.Usage = StateUsage.Get(Usages.Any);
            Situation.Chances = StateChances.Riscs();
        }

        protected override void Count()
        {
            double[,] newArr = new double[(int)Rows, (int)Cols];
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    newArr[r, c] = Arr[r, c] > 0 ? Arr[r, c] / ChanceFor(c) : Arr[r, c] * ChanceFor(c);
                }
            }

            double[] minInRows = this.NewRows();
            for (int r = 0; r < Rows; r++)
            {
                double m = newArr[r, 0];
                for (int c = 1; c < Cols; c++)
                {
                    if (newArr[r, c] < m)
                    {
                        m = newArr[r, c];
                    }
                }
                minInRows[r] = m;
            }
            SetResult(minInRows.Max(), minInRows);

            AddStep("Вектор", minInRows);
            AddStep("Максимум", Result);
        }
    }
}
