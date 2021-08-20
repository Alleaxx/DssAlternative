using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{

    public interface IMatrix<R, C, V>
    {
        V Get(int row, int col);
        void Set(int row, int col, V val);


        V[,] Arr { get; }
        R[] RowsArr { get; }
        C[] ColsArr { get; }


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


    public interface IMatrixChance<R, C, V> : IMatrix<R,C,V>
    {
        IInfoMatrix Info { get; }
    }
    public interface IInfoMatrix
    {
        event Action ChancesChanged;

        double GetChance(int col);

        bool InUnknownConditions { get; }
        bool InRiscConditions { get; }
    }


    //Главная матрица
    public abstract class MatrixContainer<R, C, V> :  IMatrix<R, C, V>
    {
        public event Action<R> RowChanged;
        public event Action<C> ColChanged;
        public event Action<Coords> ValuesChanged;

        public V Get(int row, int col) => Arr[row, col];
        public void Set(int row, int col, V val)
        {
            Arr[row, col] = val;
            ValuesChanged?.Invoke(new Coords(row,col));
        }
        public C GetCol(int pos) => ColsList[pos];
        public R GetRow(int pos) => RowsList[pos];
        public int Rows => RowsList.Count;
        public int Cols => ColsList.Count;

        
        public R[] RowsArr => RowsList.ToArray();
        public C[] ColsArr => ColsList.ToArray();

        protected List<R> RowsList { get; set; }
        protected List<C> ColsList { get; set; }
        public V[,] Arr { get; set; }



        public CellContainer<R,C,V>[][] Cells { get; set; }
        protected void UpdateCells()
        {
            Cells = new CellContainer<R, C, V>[Cols][];
            for (int r = 0; r < Cols; r++)
            {
                Cells[r] = new CellContainer<R, C, V>[Rows];
            }

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    Cells[c][r] = Filler.GetNewCell(new Coords(r,c));
                }
            }
        }



        public MatrixContainer() : this(0, 0) { }
        public MatrixContainer(int rows, int cols)
        {
            CreateFiller();
            RowsList = new List<R>(cols);            
            for (int r = 0; r < rows; r++)
            {
                RowsList.Add(Filler.GetNewRow());
            }
            ColsList = new List<C>(rows);
            for (int c = 0; c < cols; c++)
            {
                ColsList.Add(Filler.GetNewColumn());
            }
            Arr = new V[rows,cols];
            UpdateCells();
        }
        protected abstract void CreateFiller();


        protected IRowColFiller<R,C,V> Filler { private get; set; } = new FillerDefaultMatrix<R, C, V>();
        public void AddCol(int pos)
        {
            V[,] old = Arr;

            AddColToList(pos);
            Arr = new V[Rows, Cols];
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (c < pos)
                        Arr[r, c] = old[r, c];
                    else if (c == pos)
                        Arr[r, c] = Filler.GetNewValue();
                    else
                        Arr[r, c] = old[r, c - 1];
                }
            }
            UpdateCells();
            ColChanged?.Invoke(ColsList[pos]);
        }
        public void AddRow(int pos)
        {
            AddRowToList(pos);
            V[,] old = Arr;

            Arr = new V[Rows, Cols];
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (r < pos)
                        Arr[r, c] = old[r, c];
                    else if (r == pos)
                        Arr[r, c] = Filler.GetNewValue();
                    else
                        Arr[r, c] = old[r - 1, c];
                }
            }
            
            UpdateCells();
            RowChanged?.Invoke(RowsList[pos]);
        }
        public void RemoveCol(C col)
        {
            int pos = ColsList.IndexOf(col);
            RemoveColFromList(col);
            V[,] oldArr = Arr;
            Arr = new V[Rows, Cols];

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
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
            int pos = RowsList.IndexOf(row);
            RemoveRowFromList(row);
            V[,] oldArr = Arr;
            Arr = new V[Rows, Cols];

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
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
            ColsList.Insert(pos, Filler.GetNewColumn());
        }
        protected virtual void AddRowToList(int pos)
        {
            RowsList.Insert(pos, Filler.GetNewRow());
        }
        protected virtual void RemoveColFromList(C col)
        {
            ColsList.Remove(col);
        }
        protected virtual void RemoveRowFromList(R row)
        {
            RowsList.Remove(row);
        }

        protected virtual void Update()
        {
            ValuesChanged?.Invoke(new Coords(0,0));
        }
    }


    //Заполнитель
    public interface IRowColFiller<R,C,V>
    {
        R GetNewRow();
        C GetNewColumn();
        V GetNewValue();

        CellContainer<R, C, V> GetNewCell(Coords coords);
    }
    public class FillerDefaultMatrix<R, C, V> : IRowColFiller<R, C, V>
    {
        public CellContainer<R, C, V> GetNewCell(Coords c) => default;
        public C GetNewColumn() => default;
        public R GetNewRow() => default;
        public V GetNewValue() => default;
    }


    //Ячейки
    public struct Coords
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coords(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    public abstract class CellContainer<R,C,V>
    {
        public override string ToString() => Value.ToString();

        
        public Coords Coords { get; set; }

        public R Row => Matrix.GetRow(Coords.X);
        public C Col => Matrix.GetCol(Coords.Y);

        public virtual V Value
        {
            get => Matrix.Get(Coords.X, Coords.Y);
            set => Matrix.Set(Coords.X, Coords.Y, value);
        }


        public CellContainer(IMatrix<R,C,V> matrix, Coords coords)
        {
            Matrix = matrix;
            Coords = coords;
        }
        private IMatrix<R, C, V> Matrix { get; set; }
    }




}
