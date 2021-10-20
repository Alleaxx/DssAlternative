using DSSLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DSSView
{
    public class StatGameView : View<StatGame>
    {
        public MtxStat Mtx => Source.Mtx;
        public IEnumerable<IEnumerable<MtxCell<Alternative, Case, double>>> View
        {
            get
            {
                return Enumerable.Range(0, Mtx.RowsCount).Select(i => Mtx.RowElements(i));
            }
        }

        public IEnumerable<Alternative> Alternatives => Mtx.Rows;
        public IEnumerable<Case> Cases => Mtx.Cols;

        public Situation Situation => Source.Situation;
        public StateChances[] Chances { get; private set; }
        public StateGoal[] Goals { get; private set; }
        public StateUsage[] Usages { get; private set; }

        //Отчет
        public GameAnalysis Report => Source.Report;
        public IEnumerable<ICriteria> Criterias => Report.CriteriasConsider;
        public IEnumerable<Alternative> BestAlts => Report.BestAlternatives;



        public StatGameView() : this(new StatGame())
        {

        }
        public StatGameView(StatGame stat) : base(stat)
        {
            AddListeners();
            FillSitutationArrs();

            void AddListeners()
            {
                Mtx.OnRowAdded += Stat_RowChanged;
                Mtx.OnColAdded += Stat_ColChanged;

                Mtx.OnColRemoved += Stat_ColChanged;
                Mtx.OnRowRemoved += Stat_RowChanged;

                Mtx.OnValuesChanged += Stat_ValuesChanged;
                Source.Situation.OnChanged += () => Stat_ValuesChanged(default);
            }
            void FillSitutationArrs()
            {
                Chances = Enum.GetValues(typeof(Chances)).OfType<Chances>().Select(en => StateChances.Get(en)).ToArray();
                Goals = Enum.GetValues(typeof(Goals)).OfType<Goals>().Select(en => StateGoal.Get(en)).ToArray();
                Usages = Enum.GetValues(typeof(Usages)).OfType<Usages>().Select(en => StateUsage.Get(en)).ToArray();
            }
        }


        protected override void InitCommands()
        {
            AddRowCommand = new RelayCommand(AddRow);
            AddColCommand = new RelayCommand(AddCol);
            RemoveCommand = new RelayCommand(Remove);
            SaveCommand = new RelayCommand(Save);
            OpenCommand = new RelayCommand(Open);
        }
        public ICommand SaveCommand { get; private set; }
        public ICommand OpenCommand { get; private set; }
        public ICommand AddRowCommand { get; private set; }
        public ICommand AddColCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }


        private void Save(object obj)
        {
            XmlProvider.Get<StatGame>().Save(Source);
        }
        private void Open(object obj)
        {
            StatGame res = XmlProvider.Get<StatGame>().Open();
            if (res != null)
            {
                Source = res;
            }
        }
        private void AddRow(object obj)
        {
            if (obj == null)
            {
                Mtx.AddRowEnd();
            }
            else if (obj is Alternative alt)
            {
                Mtx.AddRowAfter(alt);
            }
        }
        private void AddCol(object obj)
        {
            if (obj == null)
            {
                Mtx.AddColEnd();
            }
            else if (obj is Case cas)
            {
                Mtx.AddColAfter(cas);
            }
        }
        private void Remove(object obj)
        {
            if (obj is Alternative alt && Alternatives.Count() > 1)
            {
                Mtx.RemoveRow(alt);
            }
            else if (obj is Case cas && Cases.Count() > 1)
            {
                Mtx.RemoveCol(cas);
            }
            else if(obj is IEnumerable<MtxCell<Alternative, Case, double>> en)
            {
                Mtx.RemoveRow(en.First().From);
            }
        }


        //Обновление матрицы
        private void Stat_RowChanged(Alternative obj)
        {
            UpdateAll();
        }
        private void Stat_ColChanged(Case obj)
        {
            UpdateAll();
        }
        private void Stat_ValuesChanged(Coords obj)
        {
            UpdateValues();
        }


        private void UpdateAll()
        {
            UpdateStructure();
            UpdateValues();
        }
        private void UpdateStructure()
        {
            OnPropertyChanged(nameof(View));
            OnPropertyChanged(nameof(Source));
            OnPropertyChanged(nameof(Report));
            OnPropertyChanged(nameof(Alternatives));
            OnPropertyChanged(nameof(Cases));
        }
        private void UpdateValues()
        {
            OnPropertyChanged(nameof(Criterias));
            OnPropertyChanged(nameof(BestAlts));
        }
    }

}
