using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    class CriteriasReport
    {
        public event Action Updated;

        public Matrix Matrix { get; set; }
        public ProblemPayMatrix Problem { get; set; }
        public List<Criteria> Criterias { get; set; }
        public CriteriaOption[] Options { get; set; }


        public Alternative BestAlternative => Priorities[0].Alternative;
        public CriteriasPriorAlternative[] Priorities { get; set; }


        public CriteriasReport(ProblemPayMatrix problem)
        {
            Problem = problem;
            Matrix = Problem.Matrix;
            Criterias = new List<Criteria>();
            AddCriterias();

            Matrix.StructureChanged += Matrix_Changed;
            Matrix.ValuesChanged += Matrix_Changed;
        }
        private void AddCriterias()
        {
            //Criterias.Add(new CriteriaAverage(this));
            Criterias.Add(new CriteriaWald(this));
            Criterias.Add(new CriteriaMinMax(this));
            Criterias.Add(new CriteriaMaxMax(this));
            Criterias.Add(new CriteriaLaplas(this));
            Criterias.Add(new CriteriaBaies(this));
            Criterias.Add(new CriteriaSavige(this));
            Criterias.Add(new CriteriaGurvits(this));
            Criterias.Add(new CriteriaLeman(this));
            Criterias.Add(new CriteriaMulti(this));
            Criterias.Add(new CriteriaGerr(this));


            List<CriteriaOption> options = new List<CriteriaOption>();
            for (int i = 0; i < Criterias.Count; i++)
            {
                options.AddRange(Criterias[i].Options);
            }
            Options = options.ToArray();
        }

        private void Matrix_Changed()
        {
            Update();
            Updated?.Invoke();
        }
        public void Update()
        {
            Criterias.ForEach(c => c.Update());

            //Расчет приоритеттов
            Dictionary<Alternative, List<Criteria>> PriorityAlternatives = new Dictionary<Alternative, List<Criteria>>();
            foreach (Criteria criteria in Criterias)
            {
                Alternative[] alternatives = criteria.ChoicesAlternatives;
                for (int i = 0; i < alternatives.Length; i++)
                {
                    if (!PriorityAlternatives.ContainsKey(alternatives[i]))
                        PriorityAlternatives.Add(alternatives[i], new List<Criteria>() { criteria });
                    else
                        PriorityAlternatives[alternatives[i]].Add(criteria);
                }
            }

            Priorities = new CriteriasPriorAlternative[PriorityAlternatives.Count];
            for (int i = 0; i < PriorityAlternatives.Count; i++)
            {
                Priorities[i] = new CriteriasPriorAlternative(PriorityAlternatives.ElementAt(i).Key, PriorityAlternatives.ElementAt(i).Value.ToArray());
            }
            Priorities = Priorities.OrderBy(p => p.Rank).Reverse().ToArray();
        }
    }

    class CriteriasPriorAlternative
    {
        public Alternative Alternative { get; set; }
        public Criteria[] Criterias { get; set; }

        public double Rank => Criterias.Select(c => c.Rank).Sum();

        public CriteriasPriorAlternative(Alternative alternative, Criteria[] criterias)
        {
            Alternative = alternative;
            Criterias = criterias;
        }
    }



    //Критерий (интерфейс компонента)
    abstract class Criteria
    {
        public event Action ResultChanged;


        //Описание критерия
        public string Name { get; protected set; }

        //Тип и необходимость вероятностей
        public string Type { get; protected set; }
        public bool ChancesRequired { get; protected set; }

        //Описание алгоритма
        public string Description { get; protected set; }
        public string DecizionAlgoritm { get; protected set; }
        public string DecizionInAction { get; protected set; }

        //Коэффициент, если есть
        public List<CriteriaOption> Options { get; private set; }

        //Быстрый доступ
        public CriteriasReport Report { get; private set; }
        protected double[,] Matrix => Report.Matrix.Arr;
        protected double Rows => Report.Matrix.RowsLen;
        protected double Cols => Report.Matrix.ColsLen;

        //Условия критерия
        protected List<Func<CriteriaCondition>> Conditions { get; set; }
        public List<CriteriaCondition> ConditionsRank { get; set; }
        public double Rank => ConditionsRank.Select(c => c.Profit).Sum();


        //Результаты
        public double Result { get; protected set; }
        public List<int> Choices { get; protected set; }


        public Alternative[] ChoicesAlternatives => Choices.Select(c => Report.Matrix.Rows[c]).ToArray();


        public Criteria(CriteriasReport data)
        {
            Type = "Классический";
            Description = "Неопознанный критерий без описания";
            DecizionAlgoritm = "Алгоритм решения не описан";
            DecizionInAction = "Решение не описано";
            Report = data;
            Choices = new List<int>();
            Options = new List<CriteriaOption>();

            Conditions = new List<Func<CriteriaCondition>>();
            ConditionsRank = new List<CriteriaCondition>();


            Conditions.Add(GetBasic);
            CriteriaCondition GetBasic()
            {
                bool passed = Result % 2 == 0;
                return new CriteriaCondition("Критериий делимости результата на 2", this, passed, Convert.ToInt32(passed));
            }


            Count();
        }


        //Обновить критерий
        public void Update()
        {
            Count();
            UpdateRank();
            ResultChanged?.Invoke();
        }

        //Рассчитать критерий
        protected abstract void Count();
        protected void UpdateRank()
        {
            ConditionsRank.Clear();
            ConditionsRank = Conditions.Select(c => c.Invoke()).ToList();
        }


        protected double GetChance(int col)
        {
            if (Report.Matrix.Cols[col].IsChanceKnown)
                return Report.Matrix.Cols[col].Chance;
            else
                return Report.Problem.Info.DefaultUnknownChance;
        }




        /// <summary>
        /// Возвращает минимальное значение из указанного столбца
        /// </summary>
        /// <param name="c">Номер колонки</param>
        /// <returns>Минимальное значение из колокни</returns>
        protected double GetMinFromCol(int c)
        {
            double min = Matrix[0,c];
            for (int r = 0; r < Rows; r++)
            {
                if (Matrix[r, c] <= min)
                    min = Matrix[r,c];
            }
            return min;
        }

        /// <summary>
        /// Возвращает минимальное значение из указанной строки
        /// </summary>
        /// <param name="r">Номер строки</param>
        /// <returns>Минимальное значение из строки</returns>
        protected double GetMinFromRow(int r)
        {
            double min = Matrix[r, 0];
            for (int c = 0; c < Cols; c++)
            {
                if (Matrix[r, c] <= min)
                    min = Matrix[r,c];
            }
            return min;
        }

        /// <summary>
        /// Возвращает максимальное значение из указанной колонки
        /// </summary>
        /// <param name="c">Номер колонки</param>
        /// <returns>Максимальное значение</returns>
        protected double GetMaxFromCol(int c)
        {
            double max = Matrix[0, c];
            for (int r = 0; r < Rows; r++)
            {
                if (Matrix[r, c] >= max)
                    max = Matrix[r, c];
            }
            return max;
        }

        /// <summary>
        /// Возвращает максимальное значение из указанной строки
        /// </summary>
        /// <param name="r">Номер строки</param>
        /// <returns>Максимальное значение</returns>
        protected double GetMaxFromRow(int r)
        {
            double max = Matrix[r, 0];
            for (int c = 0; c < Cols; c++)
            {
                if (Matrix[r, c] >= max)
                    max = Matrix[r, c];
            }
            return max;
        }


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
    }
    class CriteriaOption
    {

        public event Action ValueChanged;

        public string Name { get; set; }
        public double Value
        {
            get => value;
            set
            {
                if (value < Min)
                    this.value = Min;
                else if (value > Max)
                    this.value = Max;
                else
                    this.value = value;

                ValueChanged?.Invoke();
            }
        }
        private double value;


        public double Min { get; set; }
        public double Max { get; set; }

        public CriteriaOption(string name, double val, double min = 0, double max = 1)
        {
            Name = name;
            Min = min;
            Max = max;
            Value = val;
        }
    }
    class CriteriaCondition
    {
        public string Name { get; set; }
        public bool IsGood { get; set; }
        public double Profit { get; set; }

        public Criteria Criteria { get; set; }
        public Matrix Matrix { get; set; }

        public CriteriaCondition(string name, Criteria criteria,bool good, double profit)
        {
            Name = name;
            Criteria = criteria;
            Matrix = criteria.Report.Matrix;
            IsGood = good;
            Profit = profit;
        }
    }


    //Компоненты

    //Классические критерии
    class CriteriaWald : Criteria
    {
        public CriteriaWald(CriteriasReport data) : base(data)
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
            Choices = GetPositions(Result, mins);
        }
    }
    class CriteriaMinMax : Criteria
    {
        public CriteriaMinMax(CriteriasReport data) : base(data)
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
            Choices = GetPositions(Result, maxes);
        }
    }    
    class CriteriaMaxMax : Criteria
    {
        public CriteriaMaxMax(CriteriasReport data) : base(data)
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
            Choices = GetPositions(Result, maxes);
        }
    }
    class CriteriaBaies : Criteria
    {
        public CriteriaBaies(CriteriasReport data) : base(data)
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
                    sum += Matrix[r, c] * GetChance(c);
                }
                averages[r] = sum;
            }
            Result = averages.Max();
            Choices = GetPositions(Result, averages);
        }
    }
    class CriteriaLaplas : Criteria
    {
        public CriteriaLaplas(CriteriasReport data) : base(data)
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
                    sum += Matrix[r, c] / Cols;
                }
                averages[r] = sum;
            }
            Result = averages.Max();
            Choices = GetPositions(Result, averages);
        }
    }
    class CriteriaSavige : Criteria
    {
        public ProblemPayMatrix RiscMatrix { get; set; }
        public CriteriaSavige(CriteriasReport data) : base(data)
        {
            Name = "Критерий Сэвиджа";
            ChancesRequired = false;
            Description = "Критерий минимизации риска. Как и критерий Вальда, критерий Сэвиджа очень осторожен. Если критерий вальда это минимальный выигрыш, то Сэвидж определяет максимальную потерю выигрыша по сравнению с тем, чего можно было бы достичь в данных условиях";
            DecizionAlgoritm = "- Необходимо составить матрицу рисков на основе исходной\n- - Определить наилучшую эффективность каждого исхода\n- - Из наилучшей эффективности каждого исхода вычитается оригинальное значение матрицы\n- Определить наибольшие значения по строкам матрицы\n- Выбрать из них наименьшее значение и соотнести с альтернативой ";

        }

        protected override void Count()
        {
            double[] maxInRowsAfter = new double[(int)Rows];
            double[,] riscMatrix = GetRiscMatrix(Matrix);
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
            Choices = GetPositions(Result, maxInRowsAfter);
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
    class CriteriaGurvits : Criteria
    {
        public CriteriaOption GurvitsCoeff { get; set; } = new CriteriaOption("Коэффициент оптимизма", 0.4, 0, 1);
        public CriteriaGurvits(CriteriasReport data) : base(data)
        {
            Name = "Критерий Гурвица";
            Type = "Производный";
            Description = "Позволяет учесть склонность ЛПР к пессимизму или оптимизму.Вводится специальный коэффициент от 0 до 1. Чем более опасна ситуация - тем более осторожен должен быть подход к решению и тем меньшее должно быть значение\n- 0: случай крайнего пессимизма\n- 1: случай крайнего оптимизма";
            DecizionAlgoritm = "- Определить минимальное значение по альтернативе\n- Определить максимальное значение по альтернативе\n- Составляется вектор из минимального и максимального значения альтернатив: максимальное умножается на коэффициент, а минимальное на (1-коэффициент)\n- Из вектора выбирается максимальное значение";
            ChancesRequired = false;


            Options.Add(GurvitsCoeff);
            GurvitsCoeff.ValueChanged += Update;
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
            Choices = GetPositions(Result, gurv);
        }
    }
    class CriteriaLeman : Criteria
    {
        public CriteriaOption LemanCoeff { get; set; } = new CriteriaOption("Коэффициент доверия к информации", 0.6, 0, 1);
        public CriteriaLeman(CriteriasReport data) : base(data)
        {
            Name = "Критерий Ходжа-Лемана";
            Type = "Производный";
            Description = "Вносится фактор субъективности в виде коэффициента доверия к информации\n- при высоком доверии доминирует критерий Баеса-Лапласа\n- при низком доверии доминирует МиниМаксный критерий";
            DecizionAlgoritm = "- Расчитать среднее значение эффективности по альтернативам\n- Рассчитать минимальное значение эффективности по альтернативам\n- Составить вектор по альтернативам: среднее значение умножается на коэффициент, минимальное на (1-коэффициент)\n- Из вектора выбирается максимальное значение и соотносится с альтернативой";
            ChancesRequired = false;


            Options.Add(LemanCoeff);
            LemanCoeff.ValueChanged += Update;
        }

        protected override void Count()
        {
            double[] coeff = new double[(int)Rows];

            for (int r = 0; r < Rows; r++)
            {
                double min = Matrix[r, 0];
                double sum = 0;
                for (int c = 0; c < Cols; c++)
                {
                    if (Matrix[r, c] < min)
                        min = Matrix[r, c];
                    sum += Matrix[r, c] * GetChance(c);
                }
                coeff[r] = sum * LemanCoeff.Value + (1 - LemanCoeff.Value) * min;
            }
            Result = coeff.Max();
            Choices = GetPositions(Result, coeff);
        }
    }
    class CriteriaMulti : Criteria
    {
        public CriteriaMulti(CriteriasReport data) : base(data)
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
                double sum = Matrix[r, 0];
                for (int c = 1; c < Cols; c++)
                {
                    sum *= Matrix[r, c];
                }
                multi[r] = sum;
            }
            Result = multi.Max();
            Choices = GetPositions(Result, multi);
        }
    }



    class CriteriaGerr : Criteria
    {
        public CriteriaGerr(CriteriasReport data) : base(data)
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
                    newArr[r, c] = Matrix[r, c] > 0 ? Matrix[r, c] / GetChance(c) : Matrix[r, c] * GetChance(c);
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
            Choices = GetPositions(Result, minInRows);
        }
    }
}
