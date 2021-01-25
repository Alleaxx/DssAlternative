using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{


    //Задача принятия решений в условиях неопределенности
    class ProblemPayMatrix
    {
        public event Action StructureChanged;
        public event Action ValuesChanged;


        //Сама матрица
        public Matrix Matrix { get; set; }
        public CriteriasReport InfoCriterias { get; set; }

        public PayMatrixInfo Info { get; set; }


        //Зависимая матрица
        //public ProblemSafeMatrix SafeMatrix { get; set; }


        //Создание независимой матрицы
        protected ProblemPayMatrix()
        {

        }
        public ProblemPayMatrix(Matrix matrix)
        {
            Matrix = matrix;
            Info = new PayMatrixInfo(matrix);
            Matrix.StructureChanged += Matrix_StructureChanged;
            Matrix.ValuesChanged += Matrix_ValuesChanged;
            InfoCriterias = new CriteriasReport(this);
            //SafeMatrix = new ProblemSafeMatrix(this);
            Matrix_StructureChanged();
        }

        private void Matrix_ValuesChanged()
        {
            UpdateCriterias();
            ValuesChanged?.Invoke();
        }
        private void Matrix_StructureChanged()
        {
            UpdateCriterias();
            StructureChanged?.Invoke();
        }


        private void UpdateCriterias()
        {
            //SafeMatrix.UpdateFromMain();
            if (InfoCriterias != null)
                InfoCriterias.Update();
        }
    }
    
    
    /// <summary>
    /// Дополнительная информация о матрице
    /// </summary>   
    class PayMatrixInfo
    {
        public event Action ChancesChanged;

        //Источник
        public Matrix Matrix { get; set; }


        public List<Case> Cases => Matrix.Cols.ToList();
        public List<Alternative> Alternatives => Matrix.Rows.ToList();


        //Определенность и неопределенность условия
        public bool InUnknownConditions => Cases.TrueForAll(c => !c.IsChanceKnown);
        public bool InUnknownRiscConditions => !Cases.TrueForAll(c => c.IsChanceKnown) && !Cases.TrueForAll(c => !c.IsChanceKnown);
        public bool InRiscConditions => Cases.TrueForAll(c => c.IsChanceKnown);

        public string Conditions => InRiscConditions ? "в условиях риска" : InUnknownConditions ? "в условихя неопределенности" : "в условиях неопределенности и риска";


        //Текущее состояние вероятностей
        public bool AreChancesOk
        {
            get
            {
                bool allUnknown = Cases.TrueForAll(c => !c.IsChanceKnown);
                if (UnknownChances == Cases.Count)
                    return true;

                double sum = KnownChanceSum + UnknownChances * DefaultUnknownChance;
                return sum >= 0.99 && sum <= 1;
            }
        }
        public double UnknownChances => Cases.Where(c => !c.IsChanceKnown).Count();
        public double KnownChanceSum => Cases.Where(c => c.IsChanceKnown).Select(c => c.Chance).Sum();

        private double ChanceDifference => 1 - KnownChanceSum > 0 ? 1 - KnownChanceSum : 0;
        public double DefaultUnknownChance => ChanceDifference / UnknownChances;


        public void AddAlternative(Alternative alternative)
        {
            //Alternatives.Add(alternative);
        }
        public void AddCase(Case case1)
        {
            //Cases.Add(case1);
            case1.ChanceChanged += InnerCase_ChanceChanged;
            ChancesChanged?.Invoke();
        }
        public void RemoveAlternative(Alternative alternative)
        {
            //Alternatives.Remove(alternative);
        }
        public void RemoveCase(Case case1)
        {
            //Cases.Remove(case1);
            case1.ChanceChanged -= InnerCase_ChanceChanged;
            ChancesChanged?.Invoke();
        }



        private void InnerCase_ChanceChanged()
        {
            ChancesChanged?.Invoke();
        }


        public PayMatrixInfo(Matrix data)
        {
            Matrix = data;

            //Matrix.StructureChanged += Matrix_MatrixStructureChanged;
        }

    }


    class MatrixAdvance<R, C, V>
    {
        public event Action StructureChanged;
        public event Action ValuesChanged;

        //Строки и столбцы
        public List<R> Rows { get; set; }
        public int RowsLen => Rows.Count;
        public List<C> Cols { get; set; }
        public int ColsLen => Cols.Count;

        //Матрица значений
        public V[,] Arr { get; set; }


        //В виде списка ячеек
        public CellAdvance<R,C,V>[][] Cells { get; set; }
        private void UpdateCells()
        {
            Cells = new CellAdvance<R, C, V>[ColsLen][];
            for (int r = 0; r < ColsLen; r++)
            {
                Cells[r] = new CellAdvance<R, C, V>[RowsLen];
            }

            for (int r = 0; r < RowsLen; r++)
            {
                for (int c = 0; c < ColsLen; c++)
                {
                    Cells[c][r] = GetCell(new Coords(r,c));
                }
            }
        }
        
        
        public MatrixAdvance(V[,] from)
        {
            Rows = new List<R>(from.GetLength(0));
            for (int r = 0; r < from.GetLength(0); r++)
            {
                Rows.Add(GetNewRow());
            }
            Cols = new List<C>(from.GetLength(1));
            for (int c = 0; c < from.GetLength(1); c++)
            {
                Cols.Add(GetNewCol());
            }

            Arr = from;
            UpdateCells();
        }
        public MatrixAdvance(int rows, int cols)
        {
            Rows = new List<R>(cols);            
            for (int r = 0; r < rows; r++)
            {
                Rows.Add(GetNewRow());
            }
            Cols = new List<C>(rows);
            for (int c = 0; c < cols; c++)
            {
                Cols.Add(GetNewCol());
            }
            Arr = new V[rows,cols];
            UpdateCells();
        }


        //Переопределяемые стандартные значения ячеек и столбцов
        public virtual C GetNewCol() => default;
        public virtual R GetNewRow() => default;
        public virtual V GetNewValue() => default;
        public virtual CellAdvance<R,C,V> GetCell(Coords coords) => default;


        //Добавление и удаление столбцов с ячейками
        public void AddCol(int pos)
        {
            Cols.Insert(pos, GetNewCol());
            V[,] old = Arr;

            Arr = new V[RowsLen, ColsLen];
            for (int r = 0; r < RowsLen; r++)
            {
                for (int c = 0; c < ColsLen; c++)
                {
                    if (c < pos)
                        Arr[r, c] = old[r, c];
                    else if (c == pos)
                        Arr[r, c] = GetNewValue();
                    else
                        Arr[r, c] = old[r, c - 1];
                }
            }
            UpdateCells();
            StructureChanged?.Invoke();
        }
        public void AddRow(int pos)
        {
            Rows.Insert(pos, GetNewRow());
            V[,] old = Arr;

            Arr = new V[RowsLen, ColsLen];
            for (int r = 0; r < RowsLen; r++)
            {
                for (int c = 0; c < ColsLen; c++)
                {
                    if (r < pos)
                        Arr[r, c] = old[r, c];
                    else if (r == pos)
                        Arr[r, c] = GetNewValue();
                    else
                        Arr[r, c] = old[r - 1, c];
                }
            }
            
            UpdateCells();
            StructureChanged?.Invoke();
        }
        public void RemoveCol(C col)
        {
            int pos = Cols.IndexOf(col);
            Cols.Remove(col);
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
            StructureChanged?.Invoke();
        }
        public void RemoveRow(R row)
        {
            int pos = Rows.IndexOf(row);
            Rows.Remove(row);
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
            StructureChanged?.Invoke();
        }

        //Изменение значений
        public void SetValue(Coords coords, V val)
        {
            Arr[coords.X, coords.Y] = val;
            ValuesChanged?.Invoke();
        }

    }
    class Matrix : MatrixAdvance<Alternative, Case, double>
    {
        public override Case GetNewCol() => new Case($"C{ColsLen}", false);
        public override Alternative GetNewRow() => new Alternative($"A{RowsLen}");
        public override CellAdvance<Alternative,Case,double> GetCell(Coords coords) => new CellAdvanced(this, coords);


        public Matrix(double[,] from) : base(from) { }
        public Matrix(int rows, int cols) : base(rows, cols) { }
    }

    abstract class CellAdvance<R,C,V>
    {
        public override string ToString() => Value.ToString();

        
        public Coords Coords { get; set; } = new Coords(0, 0);

        //Координаты в общей матрице
        public MatrixAdvance<R,C,V> Matrix { get; set; }

        public R RowPosition => Matrix.Rows[Coords.X];
        public C ColPosition => Matrix.Cols[Coords.Y];

        public virtual V Value { get; set; }

        public CellAdvance(Coords coords)
        {
            Coords = coords;
        }
    }
    class CellAdvanced : CellAdvance<Alternative,Case,double>
    {
        public override double Value
        {
            get => Matrix.Arr[Coords.X, Coords.Y];
            set => Matrix.SetValue(Coords,value);
        }

        public CellAdvanced(Matrix matrix, Coords coords) : base(coords)
        {
            Matrix = matrix;
        }
    }

    #region Альтернативы, исходы

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
        public bool IsChanceKnown
        {
            get => isChanceKnown;
            set
            {
                isChanceKnown = value;
                ChanceChanged?.Invoke();
            }
        }
        private bool isChanceKnown;
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

        public Case(string name, bool known, double chance = 0)
        {
            Name = name;
            IsChanceKnown = known;
            Chance = chance;
        }
    }

    class AltCaseRel
    {
        public Alternative Alternative { get; set; }
        public Case Case { get; set; }

        public double Value { get; set; }
    }
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

    #endregion


    //Подзадача принятия решений матрицы минимального риска
    //class ProblemSafeMatrix : ProblemPayMatrix
    //{
    //    public ProblemPayMatrix MainMatrix { get; set; }

    //    //Создание зависимой матрицы
    //    public ProblemSafeMatrix(ProblemPayMatrix data)
    //    {
    //        MainMatrix = data;
    //        Matrix = new MatrixView(new Matrix(GetSafeMatrix(data.Matrix.SourceMatrix.Arr)));
    //        InfoCriterias = new CriteriasReport(Matrix);
    //    }

    //    //Обновить содержимое зависимой матрицы
    //    public void UpdateFromMain()
    //    {
    //        Matrix.SourceMatrix.Arr = GetSafeMatrix(MainMatrix.Matrix.SourceMatrix.Arr);
    //        InfoCriterias.Update();
    //    }



    //    //Преобразовать матрицу в матрицу наименьшего риска
    //    public static double[,] GetSafeMatrix(double[,] from)
    //    {
    //        int rows = from.GetLength(0);
    //        int cols = from.GetLength(1);
    //        double[,] safe = new double[rows, cols];


    //        double max = double.MinValue;

    //        for (int x = 0; x < rows; x++)
    //        {
    //            for (int c = 0; c < cols; c++)
    //            {
    //                if (from[x, c] > max)
    //                    max = from[x, c];
    //            }
    //        }

    //        for (int x = 0; x < rows; x++)
    //        {
    //            for (int y = 0; y < cols; y++)
    //            {
    //                safe[x, y] = from[x, y] * -1 + max;
    //            }
    //        }

    //        return safe;
    //    }
    //}


    //Матрица






    #region Строки-столбцы
    //Содержимое матрицы
    //abstract class MatrixContent
    //{ 
    //    public MatrixView Matrix { get; set; }

    //    public virtual int Position { get; set; }
    //    public virtual List<Cell> Cells { get; set; }



    //    public MatrixContent(MatrixView data,int pos)
    //    {
    //        Position = pos;
    //        Matrix = data;
    //        Cells = new List<Cell>();
            
    //        Fill();
    //    }
    //    protected abstract void Fill();
    //}

    /// <summary>
    /// Строка матрицы
    /// </summary>
    //class MatrixRow : MatrixContent
    //{
    //    public MatrixRow(MatrixView data,int pos) : base(data, pos) { }
    //    protected override void Fill()
    //    {
    //        //Заголовочная строка
    //        if(Position == 0)
    //        {
    //            Cells.Add(new CellName(Matrix));
    //            for (int c = 1; c <= Matrix.SourceMatrix.ColsLen; c++)
    //            {
    //                Cells.Add(new CellHeader(Matrix, new Coords(0, c), new CellHeaderInfo(false, c - 1)));
    //            }
    //        }
    //        //Остальные строки
    //        else
    //        {
    //            Cells.Add(new CellHeader(Matrix, new Coords(Position, 0), new CellHeaderInfo(true, Position - 1)));
    //            for (int colPos = 1; colPos <= Matrix.SourceMatrix.ColsLen; colPos++)
    //            {
    //                Cells.Add(new CellValue(Matrix,new Coords(Position,colPos)));
    //            }
    //        }

    //    }
    //}

   /// <summary>
   /// Столбец матрицы
   /// </summary>
    //class MatrixCol : MatrixContent
    //{
    //    public MatrixCol(MatrixView data,int pos) : base(data, pos) { }
    //    protected override void Fill()
    //    {
    //        if(Position == 0)
    //        {
    //            Cells.Add(new CellName(Matrix));
    //            for (int r = 1; r <= Matrix.SourceMatrix.RowsLen; r++)
    //            {
    //                Cells.Add(new CellHeader(Matrix, new Coords(r, Position), new CellHeaderInfo(true, r - 1)));
    //            }
    //        }
    //        else
    //        {
    //            Cells.Add(new CellHeader(Matrix, new Coords(0, Position), new CellHeaderInfo(false, Position - 1)));
    //            for (int rowPos = 1; rowPos <= Matrix.SourceMatrix.RowsLen; rowPos++)
    //            {
    //                Cells.Add(new CellValue(Matrix,new Coords(rowPos,Position)));
    //            }

    //        }
    //    }
    //}

    #endregion


    #region Ячейки матрицы






    /// <summary>
    /// Ячейка матрицы
    /// </summary>
    //abstract class Cell
    //{
    //    public override string ToString() => Value;

    //    public MatrixView Matrix { get; set; }

    //    //Координаты в общей матрице
    //    public Coords Coords { get; set; } = new Coords(0, 0);
    //    public virtual string Value { get; set; }

    //    public Cell(MatrixView matrix, Coords coords)
    //    {
    //        Matrix = matrix;
    //        Coords = coords;
    //    }
    //}  

    /// <summary>
    /// Ячейка матрицы
    /// </summary>
    //class CellValue : Cell
    //{
    //    public double ValueNumber
    //    {
    //        get => Matrix.SourceMatrix.Arr[Coords.X - 1, Coords.Y - 1];
    //        set
    //        {
    //            Matrix.SourceMatrix.Arr[Coords.X - 1, Coords.Y - 1] = value;
    //        }
    //    }

    //    public override string Value
    //    {
    //        get => Matrix.SourceMatrix.Arr[Coords.X - 1, Coords.Y - 1].ToString();
    //        set
    //        {
    //            if(double.TryParse(value, out double res))
    //            {
    //                Matrix.SetValue(new Coords(Coords.X - 1, Coords.Y -1), res);
    //            }
    //        }
    //    }

    //    public CellValue(MatrixView matrix, Coords coords) : base(matrix, coords)
    //    {

    //    }
    //}

    /// <summary>
    /// Ячейка заголовка столбца или строки
    /// </summary>
    //class CellHeader : Cell
    //{
    //    public CellHeaderInfo Info { get; set; }
    //    public override string Value
    //    {
    //        get
    //        {
    //            if (Coords.X == 0)
    //                return Matrix.Info.Cases[Coords.Y - 1].Name;
    //            else
    //                return Matrix.Info.Alternatives[Coords.X - 1].Name;
    //        }
    //        set
    //        {
    //            if (Coords.X == 0)
    //                Matrix.Info.Cases[Coords.Y - 1].Name = value;
    //            else
    //                Matrix.Info.Alternatives[Coords.X - 1].Name = value;
    //        }
    //    }

    //    public CellHeader(MatrixView matrix, Coords coords, CellHeaderInfo info) : base(matrix, coords)
    //    {
    //        Info = info;
    //    }
    //}
    //class CellHeaderInfo
    //{
    //    public bool IsRow { get; set; }
    //    public bool IsCol => !IsRow;

    //    public int Position { get; set; }

    //    public CellHeaderInfo(bool row, int pos)
    //    {
    //        IsRow = row;
    //        Position = pos;
    //    }
    //}

    /// <summary>
    /// Ячейка заголовка матрицы
    /// </summary>
    //class CellName : Cell
    //{
    //    public override string Value
    //    {
    //        get => Matrix.Name;
    //        set
    //        {
    //            Matrix.Name = value;
    //        }
    //    }

    //    public CellName(MatrixView matrix) : base(matrix, new Coords(0, 0)) { }
    //}


    /// <summary>
    /// Координаты ячейки
    /// </summary>


    #endregion





}
