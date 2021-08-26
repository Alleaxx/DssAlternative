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

        RankCriteria Rank { get; }
        double Result { get; }
        IEnumerable<Alternative> BestAlternatives { get; }
        List<IOption> Options { get; }
        List<IStep> Steps { get; }

        void Update();
    }

    public abstract class Criteria : NotifyObj, ICriteria
    {
        public override string ToString() => $"{Name} - {Result} ({Rank})";

        public event Action<double, IEnumerable<Alternative>> ResultChanged;

        //Описание критерия
        public string Name { get; protected set; }
        public string NameShort => Name.Replace("Критерий ", "");

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
            option.OnValueChanged += Option_Changed;
        }
        private void Option_Changed(double arg1, double arg2)
        {
            Update();
        }



        //Применимость
        public RankCriteria Rank { get; private set; }


        //Результаты
        public double Result
        {
            get => result;
            set
            {
                result = value;
                OnPropertyChanged();
            }
        }
        private double result;
        protected void SetResult(double res, IEnumerable<double> arrArg)
        {
            Result = res;
            var bestIndexes = GetPositions(res, arrArg);
            BestAlternatives = bestIndexes.Select(r => Game.GetRow(r));

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
        public IEnumerable<Alternative> BestAlternatives { get; private set; }


        public Criteria(IStatGame game)
        {
            Game = game;

            Name = "???";
            Type = "Классический";
            Description = "Неопознанный критерий без описания";
            DecizionAlgoritm = "Алгоритм решения не описан";

            Update();
        }

        //Обновление значений
        public void Update()
        {
            Steps.Clear();
            Count();
            UpdateRank();
            OnPropertyChanged(nameof(BestAlternatives));
            OnPropertyChanged(nameof(Steps));
            ResultChanged?.Invoke(Result, BestAlternatives);
        }
        private void UpdateRank()
        {
            Rank = new RankCriteria(this, Game);
        }

        //Рассчет
        protected abstract void Count();

        //Шаги рассчета
        public List<IStep> Steps { get; private set; } = new List<IStep>();
        protected void AddStep(string name, double res)
        {
            Steps.Add(new Step(Steps.Count + 1, name, res));
        }
        protected void AddStep(string name, IEnumerable<double> res)
        {
            Steps.Add(new StepArr(Steps.Count + 1, name, res));
        }


        //Быстрый доступ для рассчетов
        public IStatGame Game { get; private set; }
        protected double[,] Arr => Game.Arr;
        protected double Rows => Arr.Rows();
        protected double Cols => Arr.Cols();
        protected double ChanceFor(int col) => Game.GetChance(col);
    }


    //Информация статистической игры, необходимой для критерия
    public interface IStatGame
    {
        event Action OnInfoUpdated;

        double[,] Arr { get; }

        bool InRiscConditions { get; }

        double GetChance(int col);

        double Get(int r, int c);
        Alternative GetRow(int r);
    }
}
