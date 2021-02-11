using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    
    interface IMatrix<R, C, V>
    {
        V Get(int row, int col);
        void Set(int row, int col, V val);

        int Rows { get; }
        int Cols { get; }

        C GetCol(int pos);
        R GetRow(int pos);


        void AddRow(int pos);
        void AddCol(int pos);
        void RemoveRow(R row);
        void RemoveCol(C col);


        event Action<R> RowChanged;
        event Action<C> ColChanged;
        event Action<Coords> ValuesChanged;
    }


    //Главная матрица
    abstract class MatrixContainer<R, C, V>
    {
        public event Action<R> RowChanged;
        public event Action<C> ColChanged;
        public event Action<Coords> ValuesChanged;

        //Строки и столбцы
        public List<R> Rows { get; set; }
        public int RowsLen => Rows.Count;
        public List<C> Cols { get; set; }
        public int ColsLen => Cols.Count;

        //Матрица значений
        public V[,] Arr { get; set; }


        //В виде списка ячеек
        public CellContainer<R,C,V>[][] Cells { get; set; }
        protected void UpdateCells()
        {
            Cells = new CellContainer<R, C, V>[ColsLen][];
            for (int r = 0; r < ColsLen; r++)
            {
                Cells[r] = new CellContainer<R, C, V>[RowsLen];
            }

            for (int r = 0; r < RowsLen; r++)
            {
                for (int c = 0; c < ColsLen; c++)
                {
                    Cells[c][r] = Filler.GetNewCell(this, new Coords(r,c));
                }
            }
        }

        public MatrixContainer()
        {
            Filler = new FillerDefaultMatrix<R, C, V>();
            Rows = new List<R>();
            Cols = new List<C>();
            Arr = new V[0, 0];
            UpdateCells();
        }
        public MatrixContainer(int rows, int cols, IRowColFiller<R,C,V> filler)
        {
            Filler = filler;
            Rows = new List<R>(cols);            
            for (int r = 0; r < rows; r++)
            {
                Rows.Add(Filler.GetNewRow(this));
            }
            Cols = new List<C>(rows);
            for (int c = 0; c < cols; c++)
            {
                Cols.Add(Filler.GetNewColumn(this));
            }
            Arr = new V[rows,cols];
            UpdateCells();
        }


        //Переопределяемые стандартные значения ячеек и столбцов
        protected IRowColFiller<R,C,V> Filler { get; set; }

        //Добавление и удаление столбцов с ячейками
        public void AddCol(int pos)
        {
            V[,] old = Arr;

            AddColToList(pos);
            Arr = new V[RowsLen, ColsLen];
            for (int r = 0; r < RowsLen; r++)
            {
                for (int c = 0; c < ColsLen; c++)
                {
                    if (c < pos)
                        Arr[r, c] = old[r, c];
                    else if (c == pos)
                        Arr[r, c] = Filler.GetNewValue(this);
                    else
                        Arr[r, c] = old[r, c - 1];
                }
            }
            UpdateCells();
            ColChanged?.Invoke(Cols[pos]);
        }
        public void AddRow(int pos)
        {
            AddRowToList(pos);
            V[,] old = Arr;

            Arr = new V[RowsLen, ColsLen];
            for (int r = 0; r < RowsLen; r++)
            {
                for (int c = 0; c < ColsLen; c++)
                {
                    if (r < pos)
                        Arr[r, c] = old[r, c];
                    else if (r == pos)
                        Arr[r, c] = Filler.GetNewValue(this);
                    else
                        Arr[r, c] = old[r - 1, c];
                }
            }
            
            UpdateCells();
            RowChanged?.Invoke(Rows[pos]);
        }
        public void RemoveCol(C col)
        {
            int pos = Cols.IndexOf(col);
            RemoveColFromList(col);
            V[,] oldArr = Arr;
            Arr = new V[RowsLen, ColsLen];

            for (int r = 0; r < RowsLen; r++)
            {
                for (int c = 0; c < ColsLen; c++)
                {
                    if (c < pos)
                        Arr[r, c] = oldArr[r, c];
                    else
                        Arr[r, c] = oldArr[r, c + 1];
                }
            }
            
            UpdateCells();
            ColChanged?.Invoke(col);
        }
        public void RemoveRow(R row)
        {
            int pos = Rows.IndexOf(row);
            RemoveRowFromList(row);
            V[,] oldArr = Arr;
            Arr = new V[RowsLen, ColsLen];

            for (int r = 0; r < RowsLen; r++)
            {
                for (int c = 0; c < ColsLen; c++)
                {
                    if (r < pos)
                        Arr[r, c] = oldArr[r, c];
                    else
                        Arr[r, c] = oldArr[r + 1, c];
                }
            }
            
            UpdateCells();
            RowChanged?.Invoke(row);
        }
        
        protected virtual void AddColToList(int pos)
        {
            Cols.Insert(pos, Filler.GetNewColumn(this));
        }
        protected virtual void AddRowToList(int pos)
        {
            Rows.Insert(pos, Filler.GetNewRow(this));
        }
        protected virtual void RemoveColFromList(C col)
        {
            Cols.Remove(col);
        }
        protected virtual void RemoveRowFromList(R row)
        {
            Rows.Remove(row);
        }

        //Изменение значений
        public virtual void SetValue(Coords coords, V val)
        {
            Arr[coords.X, coords.Y] = val;
            ValuesChanged?.Invoke(coords);
        }

    }

    //Интерфейс заполнителя строк столбцов и значений для матрицы
    interface IRowColFiller<R,C,V>
    {
        R GetNewRow(MatrixContainer<R,C,V> matrix);
        C GetNewColumn(MatrixContainer<R,C,V> matrix);
        V GetNewValue(MatrixContainer<R,C,V> matrix);

        CellContainer<R, C, V> GetNewCell(MatrixContainer<R,C,V> matrix, Coords coords);
    }
    class FillerDefaultMatrix<R, C, V> : IRowColFiller<R, C, V>
    {
        public CellContainer<R, C, V> GetNewCell(MatrixContainer<R, C, V> matrix, Coords coords)
            => default;

        public C GetNewColumn(MatrixContainer<R, C, V> matrix)
            => default;

        public R GetNewRow(MatrixContainer<R, C, V> matrix)
            => default;

        public V GetNewValue(MatrixContainer<R, C, V> matrix)
            => default;
    }
    class FillerPayMatrix : IRowColFiller<Alternative, Case, double>
    {
        public CellContainer<Alternative, Case, double> GetNewCell(MatrixContainer<Alternative, Case, double> matrix, Coords coords)
            => new Cell(matrix, coords);

        public Case GetNewColumn(MatrixContainer<Alternative, Case, double> matrix)
            => new Case($"C{matrix.ColsLen}");

        public Alternative GetNewRow(MatrixContainer<Alternative, Case, double> matrix)
            => new Alternative($"A{matrix.RowsLen}");

        public double GetNewValue(MatrixContainer<Alternative, Case, double> matrix)
            => default;
    }


    //Платежная матрица
    abstract class PayMatrix : MatrixContainer<Alternative, Case, double>
    {
        public event Action CaseChanceChanged;

        protected override void AddColToList(int pos)
        {
            base.AddColToList(pos);
            Case case1 = Cols.Last();
            case1.ChanceChanged += UpdateChances;
        }
        protected void UpdateChances()
        {
            CaseChanceChanged?.Invoke();
        }
        protected override void RemoveColFromList(Case col)
        {
            base.RemoveColFromList(col);
            col.ChanceChanged -= UpdateChances;

        }


        public virtual InfoPayMatrix Info { get; set; }
        public CriteriasReport Report { get; set; }

        public PayMatrix(int rows, int cols) : base(rows, cols, new FillerPayMatrix())
        {
            Info = new InfoPayMatrix(this);
            Report = new CriteriasReport(this);
        }
    }
    //Риска
    class PayMatrixRisc : PayMatrix
    {
        public PayMatrixRisc(int rows, int cols) : base(rows, cols)
        {

        }
    }
    //Риска 2.0
    class PayMatrixSafe : PayMatrix
    {
        public PayMatrix MainMatrix { get; set; }

        public PayMatrixSafe(PayMatrixRisc matrix) : base(matrix.RowsLen, matrix.ColsLen)
        {
            MainMatrix = matrix;

            MainMatrix.ValuesChanged += c => UpdateSafeMatrixFromMain();
            MainMatrix.RowChanged += c => UpdateSafeMatrixFromMain();
            MainMatrix.ColChanged += c => UpdateSafeMatrixFromMain();
            UpdateSafeMatrixFromMain();
        }

        private void UpdateSafeMatrixFromMain()
        {
            Rows = MainMatrix.Rows;
            Cols = MainMatrix.Cols;
            Arr = new double[RowsLen, ColsLen];
            double max = double.MinValue;
            for (int r = 0; r < RowsLen; r++)
            {
                for (int c = 0; c < ColsLen; c++)
                {
                    Arr[r, c] = MainMatrix.Arr[r, c];
                    if (Arr[r, c] > max)
                        max = Arr[r, c];
                }
            }

            for (int r = 0; r < RowsLen; r++)
            {
                for (int c = 0; c < ColsLen; c++)
                {
                    Arr[r, c] = Arr[r, c] * -1 + max;
                }
            }
            UpdateCells();
            Report.Update();
        }
    }

    class InfoPayMatrix
    {
        public event Action ChancesChanged;

        //Источник
        public PayMatrix Matrix { get; set; }


        //Случаи и альтернативы
        public List<Case> Cases => Matrix.Cols.ToList();
        public List<Alternative> Alternatives => Matrix.Rows.ToList();


        //Определенность и неопределенность условия
        public bool InUnknownConditions
        {
            get => inUknownConditions;
            set
            {
                inUknownConditions = value;
                ChancesChanged?.Invoke();
            }
        }
        private bool inUknownConditions = true;
        public bool InRiscConditions => !InUnknownConditions;



        //Текущее состояние вероятностей
        public bool AreChancesOk
        {
            get
            {
                if (InUnknownConditions)
                    return true;

                double sum = ChancesSum;
                return sum >= 0.99 && sum <= 1;
            }
        }
        public double ChancesSum => InUnknownConditions ? 1 : Cases.Select(c => c.Chance).Sum();

        public double DefaultChance => (double)1 / Cases.Count;

        
        public InfoPayMatrix(PayMatrix data)
        {
            Matrix = data;
            Matrix.RowChanged += r => InnerCase_ChanceChanged();
            Matrix.ColChanged += c => InnerCase_ChanceChanged();
            Matrix.CaseChanceChanged += InnerCase_ChanceChanged;

        }
        private void InnerCase_ChanceChanged()
        {
            ChancesChanged?.Invoke();
        }

    }

    //Ячейки
    class Coords
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coords(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    abstract class CellContainer<R,C,V>
    {
        public override string ToString() => Value.ToString();

        
        public Coords Coords { get; set; } = new Coords(0, 0);

        //Координаты в общей матрице
        public MatrixContainer<R,C,V> Matrix { get; set; }

        public R RowPosition => Matrix.Rows[Coords.X];
        public C ColPosition => Matrix.Cols[Coords.Y];

        public virtual V Value
        {
            get => Matrix.Arr[Coords.X, Coords.Y];
            set => Matrix.SetValue(Coords, value);
        }

        public CellContainer(MatrixContainer<R,C,V> matrix, Coords coords)
        {
            Matrix = matrix;
            Coords = coords;
        }
    }
    class Cell : CellContainer<Alternative,Case,double>
    {
        public Cell(MatrixContainer<Alternative,Case,double> matrix, Coords coords) : base(matrix, coords) { }
    }



    class Alternative
    {
        public override string ToString() => Name;

        public string Name { get; set; }
        public Alternative(string name)
        {
            Name = name;
        }
    }
    class Case
    {
        public event Action ChanceChanged;

        public override string ToString() => Name;
        public string Name { get; set; }

        public double Chance
        {
            get => chance;
            set
            {
                double old = chance;
                chance = value;
                ChanceChanged?.Invoke();
            }
        }
        private double chance;

        public Case(string name, double chance = 0)
        {
            Name = name;
            Chance = chance;
        }
    }
}
