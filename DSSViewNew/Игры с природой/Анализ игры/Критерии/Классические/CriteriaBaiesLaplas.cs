using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSView.Extensions;
namespace DSSView
{
    public abstract class CriteriaBaiesLaplas : Criteria
    {
        public abstract double GetDivider(int c);
        public CriteriaBaiesLaplas(IStatGame game) : base(game)
        {
            Name = "Критерий Байеса-лапласа";
            ChancesRequired = true;
            Description = "Также критерий среднего выигрыша";
            DecizionAlgoritm = "- Найти среднее значение эффективности каждой альтернативы\n- Выбрать максимальное значение и соотнести с альтернативой";
        }

        protected override void Count()
        {
            double[] averages = this.NewRows();
            for (int r = 0; r < Rows; r++)
            {
                double sum = 0;
                for (int c = 0; c < Cols; c++)
                {
                    sum += Arr[r, c] * GetDivider(c);
                }
                averages[r] = sum;
            }
            SetResult(averages.Max(), averages);

            AddStep("Средние значения", averages);
            AddStep("Максимум", Result);
        }
    }


    public class CriteriaBaies : CriteriaBaiesLaplas
    {
        public override double GetDivider(int c) => ChanceFor(c);
        public CriteriaBaies(IStatGame game) : base(game)
        {
            Name = "Критерий Байеса";
            ChancesRequired = true;
            Description = "Также критерий среднего выигрыша";
            DecizionAlgoritm = "- Найти среднее значение эффективности каждой альтернативы с учетом вероятности каждого исхода\n- Выбрать максимальное значение и соотнести с альтернативой";
        }
    }
    public class CriteriaLaplas : CriteriaBaiesLaplas
    {
        public override double GetDivider(int c) => 1 / Cols;
        public CriteriaLaplas(IStatGame game) : base(game)
        {
            Name = "Критерий Лапласа";
            ChancesRequired = false;
            Description = "...";
            DecizionAlgoritm = "- Найти среднее значение эффективности каждой альтернативы. Исходы считаются равновероятными\n- Выбрать максимальное значение и соотнести с альтернативой";
        }
    }
}
