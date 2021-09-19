using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSView.Extensions;
namespace DSSView
{
    public class CriteriaGurvits : Criteria
    {
        public IOption GurvitsCoeff { get;  private set; } = new Option("Коэффициент оптимизма", 0.4, 0, 1);
        public CriteriaGurvits(IStatGame game) : base(game)
        {
            Name = "Критерий Гурвица";
            Type = "Производный";
            Description = "Позволяет учесть склонность ЛПР к пессимизму или оптимизму.Вводится специальный коэффициент от 0 до 1. Чем более опасна ситуация - тем более осторожен должен быть подход к решению и тем меньшее должно быть значение\n- 0: случай крайнего пессимизма\n- 1: случай крайнего оптимизма";
            DecizionAlgoritm = "- Определить минимальное значение по альтернативе\n- Определить максимальное значение по альтернативе\n- Составляется вектор из минимального и максимального значения альтернатив: максимальное умножается на коэффициент, а минимальное на (1-коэффициент)\n- Из вектора выбирается максимальное значение";

            Situation.Goal = StateGoal.Get(Goals.RiscAllowed);
            Situation.Usage = StateUsage.Get(Usages.Couple);
            Situation.Chances = StateChances.Unknown();
            AddOption(GurvitsCoeff);
        }

        protected override void Count()
        {
            double[] gurv = this.NewRows();

            for (int r = 0; r < Rows; r++)
            {
                double max = Arr.MaxFromRow(r);
                double min = Arr.MinFromRow(r);

                gurv[r] = (max * GurvitsCoeff.Value) + (min * (1 - GurvitsCoeff.Value));
            }
            SetResult(gurv.Max(), gurv);

            AddStep("Вектор Гурвица", gurv);
            AddStep("Максимум", Result);
        }
    }
}
