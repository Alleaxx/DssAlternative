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
    //Критерий
    public interface ICriteria
    {
        event Action<double, Alternative[]> ResultChanged;

        double Rank { get; }
        double Result { get; }
        Alternative[] BestAlts { get; }
        IOption[] Options { get; }
        List<IStep> Steps { get; }

        void Update();

    }


    public abstract class Criteria : ICriteria
    {
        public event Action<double, Alternative[]> ResultChanged;

        //Описание критерия
        public string Name { get; protected set; }

        //Тип и необходимость вероятностей
        public string Type { get; protected set; }
        public bool ChancesRequired { get; protected set; }

        //Описание алгоритма
        public string Description { get; protected set; }
        public string DecizionAlgoritm { get; protected set; }


        //Коэффициенты, если есть
        public IOption[] Options { get; protected set; }



        //Быстрый доступ
        private IInfoMatrix Info => Matrix.Info;
        private IMatrixChance<Alternative,Case,double> Matrix { get; set; }


        protected double[,] Arr
        {
            get
            {
                double[,] arr = new double[Matrix.Rows, Matrix.Cols];
                for (int r = 0; r < Rows; r++)
                {
                    for (int c = 0; c < Cols; c++)
                    {
                        arr[r, c] = Matrix.Get(r, c);
                    }
                }
                return arr;
            }
        }
        protected double Rows => Matrix.Rows;
        protected double Cols => Matrix.Cols;
        protected double GetChance(int col) => Info.GetChance(col);


        //Условия критерия
        protected List<Func<ConditionCriteria>> Conditions { get; set; }
        public List<ConditionCriteria> ConditionsRank { get; set; }
        public double Rank => ConditionsRank.Select(c => c.Profit).Sum();


        //Результаты
        public double Result { get; protected set; }
        protected List<int> BestAlternativeIndexes { get; set; }

        protected List<int> GetPositions(double search, IEnumerable<double> arr)
        {
            List<int> poses = new List<int>();
            for (int i = 0; i < arr.Count(); i++)
            {
                if (search == arr.ElementAt(i))
                    poses.Add(i);
            }
            return poses;
        }

        public Alternative[] BestAlts => BestAlternativeIndexes.Select(r => Matrix.GetRow(r)).ToArray();
        public List<IStep> Steps { get; private set; }


        public Criteria(IMatrixChance<Alternative,Case,double> matrix)
        {
            Type = "Классический";
            Description = "Неопознанный критерий без описания";
            DecizionAlgoritm = "Алгоритм решения не описан";

            Matrix = matrix;

            Steps = new List<IStep>();
            BestAlternativeIndexes = new List<int>();
            Options = new IOption[0];

            Conditions = new List<Func<ConditionCriteria>>();
            ConditionsRank = new List<ConditionCriteria>();


            Conditions.Add(GetRiscConditions);
            Conditions.Add(GetUnknownConditions);
            ConditionCriteria GetRiscConditions()
            {
                if (!ChancesRequired && Info.InUnknownConditions)
                    return new ConditionCriteria("Применимость в условиях неопределенности", true, 3);
                else if (ChancesRequired && Info.InUnknownConditions)
                    return new ConditionCriteria("Применимость в условиях неопределенности", true, 1);
                else
                    return new ConditionCriteria("Применимость в условиях неопределенности", false, 1);
            }
            ConditionCriteria GetUnknownConditions()
            {
                bool passed = ChancesRequired && Info.InRiscConditions;
                return new ConditionCriteria("Применимость в условиях риска", passed,3);
            }

            Count();

        }


        //Обновить критерий
        public void Update()
        {
            Steps.Clear();
            Count();
            UpdateRank();
            ResultChanged?.Invoke(Result,BestAlts);
        }

        //Рассчитать критерий
        protected abstract void Count();
        protected void UpdateRank()
        {
            ConditionsRank.Clear();
            ConditionsRank = Conditions.Select(c => c.Invoke()).ToList();
        }        

        protected void AddStep(string name, double res) => Steps.Add(new Step(Steps.Count + 1,name,res));
        protected void AddStep(string name, IEnumerable<double> res) => Steps.Add(new StepArr(Steps.Count + 1,name,res));







        /// <summary>
        /// Возвращает минимальное значение из указанной строки
        /// </summary>
        /// <param name="r">Номер строки</param>
        /// <returns>Минимальное значение из строки</returns>
        protected double GetMinFromRow(int r)
        {
            double min = Arr[r, 0];
            for (int c = 0; c < Cols; c++)
            {
                if (Arr[r, c] <= min)
                    min = Arr[r,c];
            }
            return min;
        }


        /// <summary>
        /// Возвращает максимальное значение из указанной строки
        /// </summary>
        /// <param name="r">Номер строки</param>
        /// <returns>Максимальное значение</returns>
        protected double GetMaxFromRow(int r)
        {
            double max = Arr[r, 0];
            for (int c = 0; c < Cols; c++)
            {
                if (Arr[r, c] >= max)
                    max = Arr[r, c];
            }
            return max;
        }


    }

    public interface IStep
    {
        int Order { get; }
        string Name { get; }
    }
    public class Option :  IOption
    {

        public event Action<double,double> Changed;

        public string Name { get; set; }
        public double Value
        {
            get => value;
            set
            {
                double old = value;
                if (value < Min)
                    this.value = Min;
                else if (value > Max)
                    this.value = Max;
                else
                    this.value = value;

                Changed?.Invoke(old, value);
            }
        }
        private double value;


        public double Min { get; set; }
        public double Max { get; set; }

        public Option(string name, double val, double min = 0, double max = 1)
        {
            Name = name;
            Min = min;
            Max = max;
            Value = val;
        }
    }
    public interface IOption
    {
        event Action<double, double> Changed;
        double Value { get; }
    }
    public class Step : IStep
    {

        public int Order { get; set; }
        public string Name { get; set; }
        public double Result { get; set; }

        public Step(int count,string name, double result)
        {
            Order = count;
            Name = name;
            Result = result;
        }
    }
    public class StepArr : IStep
    {
        public int Order { get; set; }
        public string Name { get; set; }
        public double[] Arr { get; set; }

        public StepArr(int count,string name, IEnumerable<double> arr)
        {
            Order = count;
            Name = name;
            Arr = arr.ToArray();
        }
    }


    public class ConditionCriteria
    {
        public string Name { get; set; }
        public bool IsGood { get; set; }
        public double Profit { get; set; }

        public ConditionCriteria(string name, bool good, double profit)
        {
            Name = name;
            IsGood = good;
            Profit = profit;
        }
    }



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
            for (int i = 0; i < Rows; i++)
            {
                mins.Add(GetMinFromRow(i));
            }
            Result = mins.Max();
            BestAlternativeIndexes = GetPositions(Result, mins);

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
            for (int i = 0; i < Rows; i++)
            {
                maxes.Add(GetMaxFromRow(i));
            }
            Result = maxes.Min();
            BestAlternativeIndexes = GetPositions(Result, maxes);

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
            for (int i = 0; i < Rows; i++)
            {
                maxes.Add(GetMaxFromRow(i));
            }
            Result = maxes.Max();
            BestAlternativeIndexes = GetPositions(Result, maxes);

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
            double[] averages = new double[(int)Rows];
            for (int r = 0; r < Rows; r++)
            {
                double sum = 0;
                for (int c = 0; c < Cols; c++)
                {
                    sum += Arr[r, c] * GetChance(c);
                }
                averages[r] = sum;
            }
            Result = averages.Max();
            BestAlternativeIndexes = GetPositions(Result, averages);

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
            Result = averages.Max();
            BestAlternativeIndexes = GetPositions(Result, averages);

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
            double[] maxInRowsAfter = new double[(int)Rows];
            double[,] riscMatrix = GetRiscMatrix(Arr);
            for (int r = 0; r < Rows; r++)
            {
                double max = double.MinValue;
                for (int c = 0; c < Cols; c++)
                {
                    if (riscMatrix[r, c] > max)
                        max = riscMatrix[r, c];
                }
                maxInRowsAfter[r] = max;
            }
            Result = maxInRowsAfter.Min();
            BestAlternativeIndexes = GetPositions(Result, maxInRowsAfter);

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
            double[] gurv = new double[(int)Rows];

            for (int r = 0; r < Rows; r++)
            {
                double max = GetMaxFromRow(r);
                double min = GetMinFromRow(r);

                gurv[r] =  max * GurvitsCoeff.Value + min * (1 - GurvitsCoeff.Value);
            }
            Result = gurv.Max();
            BestAlternativeIndexes = GetPositions(Result, gurv);

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
            double[] coeff = new double[(int)Rows];

            for (int r = 0; r < Rows; r++)
            {
                double min = Arr[r, 0];
                double sum = 0;
                for (int c = 0; c < Cols; c++)
                {
                    if (Arr[r, c] < min)
                        min = Arr[r, c];
                    sum += Arr[r, c] * GetChance(c);
                }
                coeff[r] = sum * LemanCoeff.Value + (1 - LemanCoeff.Value) * min;
            }
            Result = coeff.Max();
            BestAlternativeIndexes = GetPositions(Result, coeff);

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
            double[] multi = new double[(int)Rows];
            for (int r = 0; r < Rows; r++)
            {
                double sum = Arr[r, 0];
                for (int c = 1; c < Cols; c++)
                {
                    sum *= Arr[r, c];
                }
                multi[r] = sum;
            }
            Result = multi.Max();
            BestAlternativeIndexes = GetPositions(Result, multi);

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
            double[,] newArr = new double[(int)Rows, (int)Cols];
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    newArr[r, c] = Arr[r, c] > 0 ? Arr[r, c] / GetChance(c) : Arr[r, c] * GetChance(c);
                }
            }

            double[] minInRows = new double[(int)Rows];
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
            Result = minInRows.Max();
            BestAlternativeIndexes = GetPositions(Result, minInRows);

            AddStep("Вектор", minInRows);
            AddStep("Максимум", Result);
        }
    }
}
