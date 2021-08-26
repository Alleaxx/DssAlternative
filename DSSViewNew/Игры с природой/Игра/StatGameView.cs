using DSSLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DSSView
{
    //Вью-модель
    public class View<T> : NotifyObj
    {
        public override string ToString() => $"Представление '{Source}'";
        public T Source
        {
            get => source;
            protected set
            {
                source = value;
                OnPropertyChanged();
            }
        }
        private T source;
        public View(T source)
        {
            Source = source;
        }
    }
    public class StatGameView : View<StatGame>
    {
        //Матрица
        public MtxStat Mtx => Source.Mtx;
        public IEnumerable<IEnumerable<MtxCell<Alternative, Case, double>>> View
            => Enumerable.Range(0, Mtx.RowsCount).Select(i => Mtx.RowElems(i));

        public IEnumerable<Alternative> Alternatives => Mtx.Rows;
        public IEnumerable<Case> Cases => Mtx.Cols;


        //Вероятности
        public GameState State => Source.State;
        public string Status => State.Status;
        public double SumChances => State.SumCases;
        public bool InRiscConditions
        {
            get => State.Risc;
            set
            {
                State.Risc = value;
                OnPropertyChanged();
            }
        }
        public bool ChancesOk => State.IsOk;

        //Отчет
        public StatGameAnalysis Report => Source.Report;
        public IEnumerable<ICriteria> Criterias => Report.Criterias;
        public IEnumerable<Alternative> BestAlts => Report.BestAlternatives;



        public StatGameView() : this(new StatGame())
        {

        }
        public StatGameView(StatGame stat) : base(stat)
        {
            Mtx.OnRowChanged += Stat_RowChanged;
            Mtx.OnColChanged += Stat_ColChanged;
            Mtx.OnValuesChanged += Stat_ValuesChanged;

            State.OnCaseChanceChanged += State_OnCaseChanceChanged;

            AddRowCommand = new RelayCommand(AddRow);
            AddColCommand = new RelayCommand(AddCol);
            RemoveCommand = new RelayCommand(Remove);
            SaveCommand = new RelayCommand(Save);
            OpenCommand = new RelayCommand(Open);
        }

        private void State_OnCaseChanceChanged(Case obj)
        {
            StateUpdate();
        }
        private void Stat_ValuesChanged(Coords obj)
        {
            UpdateValues();
        }



        public ICommand SaveCommand { get; private set; }
        public ICommand OpenCommand { get; private set; }

        private void Save(object obj)
        {
            XmlProvider.Get<StatGame>().Save(Source);
        }
        private void Open(object obj)
        {
            var res = XmlProvider.Get<StatGame>().Open();
            if (res != null)
            {
                Source = res;
            }
        }


        public ICommand AddRowCommand { get; private set; }
        public ICommand AddColCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        private void AddRow(object obj)
        {
            if (obj == null)
            {
                Mtx.AddRow();
            }
            else if (obj is Alternative alt)
            {
                Mtx.AddRowIndex(alt);
            }
        }
        private void AddCol(object obj)
        {
            if (obj == null)
            {
                Mtx.AddCol();
            }
            else if (obj is Case cas)
            {
                Mtx.AddColIndex(cas);
            }
        }
        private void Remove(object obj)
        {
            if (obj is Alternative alt)
                Mtx.RemoveRow(alt);
            else if (obj is Case cas)
                Mtx.RemoveCol(cas);
        }


        //Обновление матрицы
        private void Stat_RowChanged(Alternative obj)
        {
            Update();
        }
        private void Stat_ColChanged(Case obj)
        {
            Update();
        }


        private void Update()
        {
            OnPropertyChanged(nameof(View));
            OnPropertyChanged(nameof(Source));
            OnPropertyChanged(nameof(Report));
            OnPropertyChanged(nameof(Alternatives));
            OnPropertyChanged(nameof(Cases));
            UpdateValues();
        }
        private void UpdateValues()
        {
            OnPropertyChanged(nameof(Criterias));
            OnPropertyChanged(nameof(BestAlts));
        }
        private void StateUpdate()
        {
            OnPropertyChanged(nameof(State));
            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(SumChances));
            OnPropertyChanged(nameof(InRiscConditions));
            OnPropertyChanged(nameof(ChancesOk));
        }
    }

}
