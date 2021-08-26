using DSSLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    //Критерий
    public interface ICriteria
    {
        event Action<double, IEnumerable<Alternative>> ResultChanged;

        double Rank { get; }
        double Result { get; }
        IEnumerable<Alternative> BestAlternatives { get; }
        List<IOption> Options { get; }
        List<IStep> Steps { get; }

        void Update();

    }
    public abstract class Criteria : ICriteria
    {
        public override string ToString() => $"{Name} - {Result} ({Rank})";

        public event Action<double, IEnumerable<Alternative>> ResultChanged;

        //Описание критерия
        public string Name { get; protected set; }

        //Тип и необходимость вероятностей
        public string Type { get; protected set; }
        public bool ChancesRequired { get; protected set; }

        //Описание алгоритма
        public string Description { get; protected set; }
        public string DecizionAlgoritm { get; protected set; }


        //Коэффициенты, если есть
        public List<IOption> Options { get; private set; } = new List<IOption>();
        protected void AddOption(IOption option)
        {
            Options.Add(option);
            option.Changed += Option_Changed;
        }
        private void Option_Changed(double arg1, double arg2)
        {
            Update();
        }


        //Быстрый доступ
        public IStatGame Game { get; private set; }

        protected double[,] Arr => Game.Arr;
        //{
        //    get
        //    {
        //        double[,] arr = new double[Matrix.Rows, Matrix.Cols];
        //        for (int r = 0; r < RowsCount; r++)
        //        {
        //            for (int c = 0; c < ColsCount; c++)
        //            {
        //                arr[r, c] = Matrix.Get(r, c);
        //            }
        //        }
        //        return arr;
        //    }
        //}
        protected double RowsCount => Game.RowsCount;
        protected double ColsCount => Game.ColsCount;
        protected double GetChance(int col) => Game.GetChance(col);


        //Применимость критерия
        public List<Note> ConditionsRank { get; set; } = new List<Note>();
        public double Rank => ConditionsRank.Sum(c => c.Profit);
        private void UpdateRank()
        {
            ConditionsRank.Clear();

            bool isOkUnknown = !ChancesRequired && Game.InUnknownConditions;
            bool isAnyway = !ChancesRequired && Game.InRiscConditions;

            bool isNotOkRisc = ChancesRequired && Game.InUnknownConditions;
            bool isOkRisc = ChancesRequired && Game.InRiscConditions;

            if (isOkUnknown)
            {
                Add("Применяется в условиях неопределенности", 3);
            }
            if (isAnyway)
            {
                Add("Может применяться и в условиях риска", 1);
            }
            if (isNotOkRisc)
            {
                Add("Не применяется в условиях неопределенности", -3);
            }
            if (isOkRisc)
            {
                Add("Применяется в условиях риска", 5);
            }

            void Add(string name, double rank)
            {
                ConditionsRank.Add(new Note(name, false, rank));
            }
        }


        //Результаты
        public double Result { get; private set; }
        protected IEnumerable<int> BestAlternativeIndexes { get; private set; }
        protected void SetResult(double res,IEnumerable<double> arrArg)
        {
            Result = res;
            BestAlternativeIndexes = GetPositions(res, arrArg);

            IEnumerable<int> GetPositions(double search, IEnumerable<double> arr)
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

        public IEnumerable<Alternative> BestAlternatives => BestAlternativeIndexes.Select(r => Game.GetRow(r));
        public List<IStep> Steps { get; private set; } = new List<IStep>();


        public Criteria(IStatGame game)
        {
            Game = game;

            Type = "Классический";
            Description = "Неопознанный критерий без описания";
            DecizionAlgoritm = "Алгоритм решения не описан";

            ConditionsRank = new List<Note>();

            UpdateRank();
            Count();
        }


        //Обновить критерий
        public void Update()
        {
            Steps.Clear();
            Count();
            UpdateRank();
            ResultChanged?.Invoke(Result, BestAlternatives);
        }

        //Рассчитать критерий
        protected abstract void Count();

        protected void AddStep(string name, double res)
        {
            Steps.Add(new Step(Steps.Count + 1, name, res));
        }
        protected void AddStep(string name, IEnumerable<double> res)
        {
            Steps.Add(new StepArr(Steps.Count + 1, name, res));
        }







        /// <summary>
        /// Возвращает минимальное значение из указанной строки
        /// </summary>
        /// <param name="r">Номер строки</param>
        /// <returns>Минимальное значение из строки</returns>
        protected double GetMinFromRow(int r)
        {
            double min = Arr[r, 0];
            for (int c = 0; c < ColsCount; c++)
            {
                if (Arr[r, c] <= min)
                    min = Arr[r, c];
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
            for (int c = 0; c < ColsCount; c++)
            {
                if (Arr[r, c] >= max)
                    max = Arr[r, c];
            }
            return max;
        }
    }


    //Информация для критерия
    public interface IStatGame
    {
        event Action OnInfoUpdated;

        double[,] Arr { get; }
        double RowsCount { get; }
        double ColsCount { get; }

        bool InRiscConditions { get; }
        bool InUnknownConditions { get; }

        double GetChance(int col);

        double Get(int r, int c);
        Alternative GetRow(int r);
    }


    //Настройка для критериев
    public interface IOption
    {
        event Action<double, double> Changed;
        double Value { get; }
    }
    public class Option : IOption
    {

        public event Action<double, double> Changed;

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


    //Шаг алгоритма
    public interface IStep
    {
        int Order { get; }
        string Name { get; }
    }
    public class Step : IStep
    {

        public int Order { get; set; }
        public string Name { get; set; }
        public double Result { get; set; }

        public Step(int count, string name, double result)
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

        public StepArr(int count, string name, IEnumerable<double> arr)
        {
            Order = count;
            Name = name;
            Arr = arr.ToArray();
        }
    }


    public class Note
    {
        public string Name { get; set; }
        public double Profit { get; set; }
        public bool IsGood => Profit > 0;

        public Note(string name, bool good, double profit)
        {
            Name = name;
            Profit = profit;
        }
    }



}
