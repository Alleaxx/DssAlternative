using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using DSSLib;

namespace DSSView
{
    //Классические критерии
    public class CriteriaWald : Criteria
    {
        public CriteriaWald(IMatrixChance<Alternative,Case,double> matrix) : base(matrix)
        {
            Name = "Критерий Вальда";
            Description = "Критерий крайнего пессимизма. Наиболее осторожный критерий. Ориентирован на наихудшие условия, только среди которых отыскивается наилучший и теперь уже гарантированный результат.";
            DecizionAlgoritm = "- Найти наихудшие варианты исхода по каждой альтернативе\n- Из них выбрать наилучший вариант выбора альтернативы";
        }
        protected override void Count()
        {
            List<double> mins = new List<double>();
            for (int i = 0; i < RowsCount; i++)
            {
                mins.Add(GetMinFromRow(i));
            }
            SetResult(mins.Max(), mins);

            AddStep("Минимумы", mins);
            AddStep("Максимум", Result);
        }
    }
    public class CriteriaMinMax : Criteria
    {
        public CriteriaMinMax(IMatrixChance<Alternative,Case,double> matrix) : base(matrix)
        {
            Name = "Критерий минимакса";
            ChancesRequired = false;
            Description = "...";
            DecizionAlgoritm = "- Найти наилучшие варианты исхода по каждой альтернативе\n- Из них выбрать наихудший вариант выбора альтернативы";
        }
        protected override void Count()
        {
            List<double> maxes = new List<double>();
            for (int i = 0; i < RowsCount; i++)
            {
                maxes.Add(GetMaxFromRow(i));
            }
            SetResult(maxes.Min(), maxes);

            AddStep("Максимумы", maxes);
            AddStep("Минимум", Result);
        }
    }    
    public class CriteriaMaxMax : Criteria
    {
        public CriteriaMaxMax(IMatrixChance<Alternative,Case,double> matrix) : base(matrix)
        {
            Name = "Критерий азартного игрока";
            ChancesRequired = false;
            Description = "Критерий крайнего оптимизма или критерий максимакса. В данном случае ЛПР делает ставку на то, что произойдет наиболее благоприятный исход";
            DecizionAlgoritm = "- Найти наилучшие варианты исхода по каждой альтернативе\n- Из них выбрать наилучший вариант выбора альтернативы";
        }
        protected override void Count()
        {
            List<double> maxes = new List<double>();
            for (int i = 0; i < RowsCount; i++)
            {
                maxes.Add(GetMaxFromRow(i));
            }
            SetResult(maxes.Max(), maxes);

            AddStep("Максимумы", maxes);
            AddStep("Максимум", Result);
        }
    }
    public class CriteriaBaies : Criteria
    {
        public CriteriaBaies(IMatrixChance<Alternative,Case,double> matrix) : base(matrix)
        {
            Name = "Критерий Байеса";
            ChancesRequired = true;
            Description = "Также критерий среднего выигрыша";
            DecizionAlgoritm = "- Найти среднее значение эффективности каждой альтернативы с учетом вероятности каждого исхода\n- Выбрать максимальное значение и соотнести с альтернативой";
        }

        protected override void Count()
        {
            double[] averages = new double[(int)RowsCount];
            for (int r = 0; r < RowsCount; r++)
            {
                double sum = 0;
                for (int c = 0; c < ColsCount; c++)
                {
                    sum += Arr[r, c] * GetChance(c);
                }
                averages[r] = sum;
            }
            SetResult(averages.Max(), averages);

            AddStep("Средние значения", averages);
            AddStep("Максимум", Result);
        }
    }
    public class CriteriaLaplas : Criteria
    {
        public CriteriaLaplas(IMatrixChance<Alternative,Case,double> matrix) : base(matrix)
        {
            Name = "Критерий Лапласа";
            ChancesRequired = false;
            Description = "...";
            DecizionAlgoritm = "- Найти среднее значение эффективности каждой альтернативы. Исходы считаются равновероятными\n- Выбрать максимальное значение и соотнести с альтернативой";
        }

        protected override void Count()
        {
            double[] averages = new double[(int)RowsCount];
            for (int r = 0; r < RowsCount; r++)
            {
                double sum = 0;
                for (int c = 0; c < ColsCount; c++)
                {
                    sum += Arr[r, c] / ColsCount;
                }
                averages[r] = sum;
            }
            SetResult(averages.Max(), averages);

            AddStep("Средние значения", averages);
            AddStep("Максимум", Result);
        }
    }
    public class CriteriaSavige : Criteria
    {
        public PayMatrixRisc RiscMatrix { get; set; }
        public CriteriaSavige(IMatrixChance<Alternative,Case,double> matrix) : base(matrix)
        {
            Name = "Критерий Сэвиджа";
            ChancesRequired = false;
            Description = "Критерий минимизации риска. Как и критерий Вальда, критерий Сэвиджа очень осторожен. Если критерий вальда это минимальный выигрыш, то Сэвидж определяет максимальную потерю выигрыша по сравнению с тем, чего можно было бы достичь в данных условиях";
            DecizionAlgoritm = "- Необходимо составить матрицу рисков на основе исходной\n- - Определить наилучшую эффективность каждого исхода\n- - Из наилучшей эффективности каждого исхода вычитается оригинальное значение матрицы\n- Определить наибольшие значения по строкам матрицы\n- Выбрать из них наименьшее значение и соотнести с альтернативой ";

        }

        protected override void Count()
        {
            double[] maxInRowsAfter = new double[(int)RowsCount];
            double[,] riscMatrix = GetRiscMatrix(Arr);
            for (int r = 0; r < RowsCount; r++)
            {
                double max = double.MinValue;
                for (int c = 0; c < ColsCount; c++)
                {
                    if (riscMatrix[r, c] > max)
                        max = riscMatrix[r, c];
                }
                maxInRowsAfter[r] = max;
            }
            SetResult(maxInRowsAfter.Min(), maxInRowsAfter);

            AddStep("Максимумы по строке в матрице рисков", maxInRowsAfter);
            AddStep("Минимум", Result);
        }

        public static double[,] GetRiscMatrix(double[,] from)
        {
            int rows = from.GetLength(0);
            int cols = from.GetLength(1);
            double[] maxesInCols = new double[cols];
            for (int c = 0; c < cols; c++)
            {
                double max = double.MinValue;
                for (int r = 0; r < rows; r++)
                {
                    if (from[r, c] > max)
                        max = from[r, c];
                }
                maxesInCols[c] = max;
            }


            double[,] riscMatrix = new double[rows, cols];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    riscMatrix[r, c] = maxesInCols[c] - from[r, c];
                }
            }

            return riscMatrix;
        }

    }


    //Производные критерии
    public class CriteriaGurvits : Criteria
    {
        public IOption GurvitsCoeff { get; set; } = new Option("Коэффициент оптимизма", 0.4, 0, 1);
        public CriteriaGurvits(IMatrixChance<Alternative,Case,double> matrix) : base(matrix)
        {
            Name = "Критерий Гурвица";
            Type = "Производный";
            Description = "Позволяет учесть склонность ЛПР к пессимизму или оптимизму.Вводится специальный коэффициент от 0 до 1. Чем более опасна ситуация - тем более осторожен должен быть подход к решению и тем меньшее должно быть значение\n- 0: случай крайнего пессимизма\n- 1: случай крайнего оптимизма";
            DecizionAlgoritm = "- Определить минимальное значение по альтернативе\n- Определить максимальное значение по альтернативе\n- Составляется вектор из минимального и максимального значения альтернатив: максимальное умножается на коэффициент, а минимальное на (1-коэффициент)\n- Из вектора выбирается максимальное значение";
            ChancesRequired = false;

            Options = new IOption[] { GurvitsCoeff };
            GurvitsCoeff.Changed += (double old, double newV) => Update();
        }

        protected override void Count()
        {
            double[] gurv = new double[(int)RowsCount];

            for (int r = 0; r < RowsCount; r++)
            {
                double max = GetMaxFromRow(r);
                double min = GetMinFromRow(r);

                gurv[r] =  max * GurvitsCoeff.Value + min * (1 - GurvitsCoeff.Value);
            }
            SetResult(gurv.Max(), gurv);

            AddStep("Вектор Гурвица", gurv);
            AddStep("Максимум", Result);
        }
    }
    public class CriteriaLeman : Criteria
    {
        public IOption LemanCoeff { get; set; } = new Option("Коэффициент доверия к информации", 0.6, 0, 1);
        public CriteriaLeman(IMatrixChance<Alternative,Case,double> matrix) : base(matrix)
        {
            Name = "Критерий Ходжа-Лемана";
            Type = "Производный";
            Description = "Вносится фактор субъективности в виде коэффициента доверия к информации\n- при высоком доверии доминирует критерий Баеса-Лапласа\n- при низком доверии доминирует МиниМаксный критерий";
            DecizionAlgoritm = "- Расчитать среднее значение эффективности по альтернативам\n- Рассчитать минимальное значение эффективности по альтернативам\n- Составить вектор по альтернативам: среднее значение умножается на коэффициент, минимальное на (1-коэффициент)\n- Из вектора выбирается максимальное значение и соотносится с альтернативой";
            ChancesRequired = false;

            Options = new IOption[] { LemanCoeff };
            LemanCoeff.Changed += (double old, double newV) => Update();
        }

        protected override void Count()
        {
            double[] coeff = new double[(int)RowsCount];

            for (int r = 0; r < RowsCount; r++)
            {
                double min = Arr[r, 0];
                double sum = 0;
                for (int c = 0; c < ColsCount; c++)
                {
                    if (Arr[r, c] < min)
                        min = Arr[r, c];
                    sum += Arr[r, c] * GetChance(c);
                }
                coeff[r] = sum * LemanCoeff.Value + (1 - LemanCoeff.Value) * min;
            }
            SetResult(coeff.Max(), coeff);

            AddStep("Вектор Ходжа-Лемана", coeff);
            AddStep("Максимум", Result);
        }
    }
    public class CriteriaMulti : Criteria
    {
        public CriteriaMulti(IMatrixChance<Alternative,Case,double> matrix) : base(matrix)
        {
            Name = "Критерий произведений";
            Type = "Производный";
            ChancesRequired = false;
            Description = "Применяется при принятии решений в условиях неопределенности. Более нейтрален по сравнению с максиминным и максимаксным критерием";
            DecizionAlgoritm = "- Составить вектор по альтернативам, перемножив значения исхода\n- Выбрать максимальное значение из вектора и соотнести с альтернативой";
        }

        protected override void Count()
        {
            double[] multi = new double[(int)RowsCount];
            for (int r = 0; r < RowsCount; r++)
            {
                double sum = Arr[r, 0];
                for (int c = 1; c < ColsCount; c++)
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


    public class CriteriaGerr : Criteria
    {
        public CriteriaGerr(IMatrixChance<Alternative,Case,double> matrix) : base(matrix)
        {
            Name = "Критерий Гермейера";
            Type = "Производный";
            Description = "Ориентирован на величину потерь";
            DecizionAlgoritm = "- Определить минимальное значение по альтернативе\n- Определить максимальное значение по альтернативе\n- Составляется вектор из минимального и максимального значения альтернатив: максимальное умножается на коэффициент, а минимальное на (1-коэффициент)\n- Из вектора выбирается максимальное значение";
            ChancesRequired = false;

        }

        protected override void Count()
        {
            double[,] newArr = new double[(int)RowsCount, (int)ColsCount];
            for (int r = 0; r < RowsCount; r++)
            {
                for (int c = 0; c < ColsCount; c++)
                {
                    newArr[r, c] = Arr[r, c] > 0 ? Arr[r, c] / GetChance(c) : Arr[r, c] * GetChance(c);
                }
            }

            double[] minInRows = new double[(int)RowsCount];
            for (int r = 0; r < RowsCount; r++)
            {
                double m = newArr[r, 0];
                for (int c = 1; c < ColsCount; c++)
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
