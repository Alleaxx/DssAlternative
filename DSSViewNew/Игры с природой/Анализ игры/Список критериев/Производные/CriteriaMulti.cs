using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSView.Extensions;
namespace DSSView.Criterias
{
    //Только с положительной матрицей
    public class CriteriaMulti : Criteria
    {
        public CriteriaMulti(IStatGame game) : base(game)
        {
            Name = "Критерий Произведений";
            Type = "Производный";
            Situation.Goal = StateGoal.Get(Goals.RiscAllowed);
            Situation.Usage = StateUsage.Get(Usages.Any);
            Situation.Chances = StateChances.Get(Chances.Unknown);
            Description = "Применяется при принятии решений в условиях неопределенности. Более нейтрален по сравнению с максиминным и максимаксным критерием";
            DecizionAlgoritm = "- Составить вектор по альтернативам, перемножив значения исхода\n- Выбрать максимальное значение из вектора и соотнести с альтернативой";
        }

        protected override void Count()
        {
            double[] multi = this.NewRows();
            for (int r = 0; r < Rows; r++)
            {
                double sum = Arr[r, 0];
                for (int c = 1; c < Cols; c++)
                {
                    sum *= Arr[r, c];
                }
                multi[r] = sum;
            }
            SetResult(multi.Max(), multi);

            AddStep("Произведения", multi);
            AddStep("Максимум", Result);
        }
    }

}
