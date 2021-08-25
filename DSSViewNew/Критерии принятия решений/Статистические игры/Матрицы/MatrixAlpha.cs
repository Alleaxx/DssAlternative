using DSSLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DSSView
{
    public class Matrix<R, C, V>
    {
        public event Action<R> OnRowChanged;
        public event Action<C> OnColChanged;
        public event Action<Coords> OnValuesChanged;


        //Значения матрицы
        private List<MtxCell<R, C, V>> Source { get; set; } = new List<MtxCell<R, C, V>>();
        public IEnumerable<MtxCell<R, C, V>> Cells => Source;
        public V[,] Values
        {
            get
            {
                V[,] arr = new V[RowsCount, ColsCount];
                for (int x = 0; x < arr.GetLength(0); x++)
                {
                    for (int y = 0; y < arr.GetLength(1); y++)
                    {
                        arr[x, y] = Source.Where(s => s.Coords.X == x && s.Coords.Y == y).First().Value;
                    }
                }
                return arr;
            }
        }



        //Информация о матрице
        //Все ячейки указанной строки или столбца
        public IEnumerable<MtxCell<R, C, V>> RowElems(int row) => Source.Where(el => el.Coords.X == row);
        public IEnumerable<MtxCell<R, C, V>> RowElems(R row) => Source.Where(el => el.From.Equals(row));
        public IEnumerable<MtxCell<R, C, V>> ColElems(int col) => Source.Where(el => el.Coords.Y == col);
        public IEnumerable<MtxCell<R, C, V>> ColElems(C col) => Source.Where(el => el.To.Equals(col));


        //Объекты строк и столбцов
        public R[] Rows => Source.Select(r => r.From).Distinct().ToArray();
        public C[] Cols => Source.Select(r => r.To).Distinct().ToArray();

        //Количество строк и столбцов
        public int RowsCount => Source.Max(s => s.Coords.X) + 1;
        public int ColsCount => Source.Max(s => s.Coords.Y) + 1;

        //Позиция строк и столбцов
        private int IndexOf(R row) => Source.First(cell => cell.From.Equals(row)).Coords.X;
        private int IndexOf(C col) => Source.First(cell => cell.To.Equals(col)).Coords.Y;


        //Добавление столбца
        public void AddCol() => AddCol(ColsCount);
        public void AddCol(C col) => AddCol(IndexOf(col));
        public void AddCol(int pos) => AddCol(pos, CellFactory.NewCol);
        public void AddCol(int pos, C col)
        {
            var rows = Rows;
            int colIndex = pos;
            OffsetCols(colIndex, 1);
            for (int i = 0; i < rows.Length; i++)
            {
                R row = rows[i];
                Coords coords = new Coords(i, colIndex);
                var cell = CellFactory.NewCell(coords, row, col, CellFactory.NewValue);
                Source.Add(cell);
                CellAdded(cell);
            }
            OnColChanged?.Invoke(col);
        }

        //Добавление строки
        public void AddRow() => AddRow(RowsCount);
        public void AddRow(R row) => AddRow(IndexOf(row));
        public void AddRow(int pos) => AddRow(pos, CellFactory.NewRow);
        public void AddRow(int pos, R row)
        {
            var cols = Cols;
            int rowIndex = pos;
            OffsetRows(rowIndex, 1);
            for (int i = 0; i < cols.Length; i++)
            {
                C col = cols[i];
                Coords coords = new Coords(rowIndex, i);
                var cell = CellFactory.NewCell(coords, row, col, CellFactory.NewValue);
                Source.Add(cell);
                CellAdded(cell);
            }
            OnRowChanged?.Invoke(row);
        }

        protected virtual void CellRemoved(MtxCell<R, C, V> removed)
        {
            removed.OnValueUpdate -= Cell_OnValueUpdate;
        }
        protected virtual void CellAdded(MtxCell<R, C, V> added)
        {
            added.OnValueUpdate += Cell_OnValueUpdate;
        }
        private void Cell_OnValueUpdate(MtxCell<R, C, V> obj)
        {
            OnValuesChanged?.Invoke(obj.Coords);
        }

        //Сдвинуть координаты ячеек на указанное значени
        private void OffsetRows(int begin, int offset)
        {
            var cells = Source.Where(c => c.Coords.X >= begin);
            foreach (var cell in cells)
            {
                int x = cell.Coords.X + offset;
                int y = cell.Coords.Y;
                cell.Coords = new Coords(x, y);
            }
        }
        private void OffsetCols(int begin, int offset)
        {
            var cells = Source.Where(c => c.Coords.Y >= begin);
            foreach (var cell in cells)
            {
                int x = cell.Coords.X;
                int y = cell.Coords.Y + offset;
                cell.Coords = new Coords(x, y);
            }
        }


        //Получение
        public V Get(int row, int col) => Source.First(s => s.Coords.X == row && s.Coords.Y == col).Value;
        public C GetCol(int pos) => Source.First(s => s.Coords.Y == pos).To;
        public R GetRow(int pos) => Source.First(s => s.Coords.X == pos).From;


        //Удаление
        public void RemoveCol(C col)
        {
            int index = IndexOf(col);
            var elems = Source.Where(c => c.To.Equals(col));
            Source.RemoveAll(c => c.To.Equals(col));
            OffsetCols(index + 1, -1);
            OnColChanged?.Invoke(col);
        }
        public void RemoveRow(R row)
        {
            int index = IndexOf(row);
            Source.RemoveAll(c => c.From.Equals(row));
            OffsetRows(index + 1, -1);
            OnRowChanged?.Invoke(row);
        }


        //Изменение значения
        public void Set(int row, int col, V val)
        {
            var cell = Source.Where(s => s.Coords.X == row && s.Coords.Y == col).FirstOrDefault();
            Set(cell, val);
        }
        public void Set(R row, C col, V val)
        {
            var cell = Source.Where(s => s.From.Equals(row) && s.To.Equals(col)).FirstOrDefault();
            Set(cell, val);
        }
        private void Set(MtxCell<R, C, V> cell, V val)
        {
            if (cell != null)
            {
                cell.Value = val;
                OnValuesChanged?.Invoke(cell.Coords);
            }
            else
            {
                throw new Exception("Нельзя установить значение в пустую ячейку");
            }
        }


        public Matrix() : this(new MtxCellFactory<R, C, V>())
        {

        }
        public Matrix(MtxCellFactory<R, C, V> cellFactory)
        {
            CellFactory = cellFactory;
            Source.Add(CellFactory.NewCell());
        }

        //Получение новых значений для ячеек
        private MtxCellFactory<R, C, V> CellFactory { get; set; }
    }
    public class MtxCell<R, C, V>
    {
        public override string ToString() => $"{Coords} {Value}: {From} - {To}";

        public event Action<MtxCell<R, C, V>> OnValueUpdate;

        public Coords Coords { get; set; }
        public R From { get; set; }
        public C To { get; set; }
        public V Value
        {
            get => value;
            set
            {
                this.value = value;
                OnValueUpdate?.Invoke(this);
            }
        }
        private V value;
    }
    public class MtxCellFactory<R, C, V>
    {
        public virtual MtxCell<R, C, V> NewCell() => new MtxCell<R, C, V>();
        public virtual MtxCell<R, C, V> NewCell(Coords coords, R r, C c, V v) => new MtxCell<R, C, V>();
        public virtual R NewRow => default;
        public virtual C NewCol => default;
        public virtual V NewValue => default;

    }


    //Матрица статистической игры
    public class MtxStat : Matrix<Alternative, Case, double>
    {
        public MtxStat() : base(new MtxStatFactory())
        {

        }

        public static MtxStat FromSize(int rows, int cols) => default;
        public static MtxStat FromArr() => default;
        public static MtxStat FromSafe() => default;
        public static MtxStat FromRisc() => default;

    }
    public class AltCase : MtxCell<Alternative, Case, double>
    {

    }
    public class MtxStatFactory : MtxCellFactory<Alternative, Case, double>
    {
        public override MtxCell<Alternative, Case, double> NewCell()
            => new AltCase() { Coords = new Coords(0,0), From = NewRow, To = NewCol, Value = NewValue };

        public override MtxCell<Alternative, Case, double> NewCell(Coords coords, Alternative r, Case c, double v)
            => new AltCase() { Coords = coords, From = r, To = c, Value = v };


        public override Case NewCol => new Case("Случай");
        public override Alternative NewRow => new Alternative("Альтернатива");
        public override double NewValue => 0;
    }

    //Статистическая игра
    public class StatGame
    {
        public event Action OnStateUpdated;

        public MtxStat Mtx { get; set; }
        public ReportCriterias Report { get; set; }
        public GameState Conditions { get; set; }

        public StatGame()
        {
            Mtx = new MtxStat();
            Conditions = new GameState(this);
            //Report = new ReportCriterias();
            Mtx.OnRowChanged += r => MtxUpdated();
            Mtx.OnColChanged += c => MtxUpdated();
            Mtx.OnValuesChanged += c => MtxUpdated();
        }
        private void MtxUpdated()
        {
            OnStateUpdated?.Invoke();
        }
    }
    public class GameState
    {
        public event Action<Case> OnCaseChanceChanged;

        private StatGame Game { get; set; }
        private IEnumerable<Case> Cases => Game.Mtx.Cols;


        public double SumCases => Cases.Select(c => GetChance(c)).Sum();
        public bool IsOk => SumCases == 1;
        public bool Risc
        {
            get => risc;
            set
            {
                risc = value;
                OnCaseChanceChanged?.Invoke(null);
            }
        }
        private bool risc;
        public bool Unknown => !Risc;
        public string Status => IsOk ? Risc ? "Условия риска" : "Условия неизвестности" : "Некорректные условия";
        public double GetChance(Case c)
        {
            if (Unknown)
                return 1 / (double)Cases.Count();
            else
                return c.Chance;
        }


        public GameState(StatGame game)
        {
            Game = game;
            game.Mtx.OnColChanged += Mtx_OnColChanged;
            foreach (var c in Cases)
            {
                c.OnChanceChanged += Case_OnChanceChanged;
            }
        }

        private void Case_OnChanceChanged()
        {
            OnCaseChanceChanged?.Invoke(null);
        }


        private void Mtx_OnColChanged(Case obj)
        {
            obj.OnChanceChanged += Case_OnChanceChanged;
        }
    }


    //Вью-модель
    public class View<T> : NotifyObj
    {
        public T Source { get; private set; }
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
        public double Sum => Mtx.Sum();

        //Вероятности
        public GameState State => Source.Conditions;
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
        }

        private void State_OnCaseChanceChanged(Case obj)
        {
            StateUpdate();
        }
        private void Stat_ValuesChanged(Coords obj)
        {
            UpdateValues();
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
                Mtx.AddRow(alt);
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
                Mtx.AddCol(cas);
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
            UpdateValues();
        }
        private void UpdateValues()
        {
            OnPropertyChanged(nameof(Sum));
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


    //Методы расширения для матрицы
    public static class MtxExtensions
    {
        public static double Sum(this MtxStat mtx)
        {
            return mtx.Cells.Sum(c => c.Value);
        }
    }

}
