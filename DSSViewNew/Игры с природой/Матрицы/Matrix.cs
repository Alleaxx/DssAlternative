using DSSLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class MatrixException : Exception
    {
        public MatrixException(string msg) : base(msg)
        {

        }
    }
    public class MatrixGetValueException : MatrixException
    {
        public MatrixGetValueException(int row, int col) : base($"Указанной ячейки [{row},{col}] не существует")
        {

        }
    }
    public class Matrix<R, C, V>
    {
        public override string ToString()
        {
            return $"Матрица {RowsCount}x{ColsCount} ({typeof(R).Name}-{typeof(C).Name}-{typeof(V).Name})";
        }

        public event Action<R> OnRowAdded;
        public event Action<R> OnRowRemoved;
        public event Action<C> OnColAdded;
        public event Action<C> OnColRemoved;
        public event Action<Coords> OnValuesChanged;


        //Значения матрицы
        private readonly List<MtxCell<R, C, V>> Source = new List<MtxCell<R, C, V>>();
        public IEnumerable<MtxCell<R, C, V>> Cells => Source;
        public V[,] Values
        {
            get
            {
                V[,] arr = new V[RowsCount, ColsCount];
                for (int x = 0; x < arr.Rows(); x++)
                {
                    for (int y = 0; y < arr.Cols(); y++)
                    {
                        arr[x, y] = Source.First(s => s.Coords.X == x && s.Coords.Y == y).Value;
                    }
                }
                return arr;
            }
        }

        public bool IsEmpty => !Source.Any();
        protected void SetEmpty()
        {
            Source.Clear();
        }



        //Информация о матрице
        public IEnumerable<MtxCell<R, C, V>> RowElements(int row)
        {
            return Source.Where(el => el.Coords.X == row);
        }


        //Объекты строк и столбцов
        public R[] Rows => Source.Select(r => r.From).Distinct().ToArray();
        public C[] Cols => Source.Select(r => r.To).Distinct().ToArray();

        public int RowsCount => IsEmpty ? 0 : Source.Max(s => s.Coords.X) + 1;
        public int ColsCount => IsEmpty ? 0 : Source.Max(s => s.Coords.Y) + 1;

        //Позиция строк и столбцов
        private int IndexOf(R row)
        {
            return Source.First(cell => cell.From.Equals(row)).Coords.X;
        }
        private int IndexOf(C col)
        {
            return Source.First(cell => cell.To.Equals(col)).Coords.Y;
        }


        protected void AddCell(MtxCell<R, C, V> cell)
        {
            Source.Add(cell);
            CellAdded(cell);
        }
        
        //Добавление столбца
        public void AddColEnd()
        {
            AddCol(ColsCount);
        }
        public void AddColBefore(C col)
        {
            AddCol(IndexOf(col));
        }
        private void AddCol(int pos)
        {
            AddCol(pos, CellFactory.NewCol(pos), CellFactory.NewValue);
        }
        private void AddCol(int colIndex, C col, V val)
        {
            OffsetCols(colIndex, 1);
            AddColCells();
            OnColAdded?.Invoke(col);

            void AddColCells()
            {
                for (int r = 0; r < Rows.Length; r++)
                {
                    R row = Rows[r];
                    Coords coords = Coords.Of(r, colIndex);
                    var cell = CellFactory.NewCell(coords, row, col, val);
                    AddCell(cell);
                }
            }
        }

        //Добавление строки
        public void AddRowEnd()
        {
            AddRow(RowsCount);
        }
        public void AddRowBefore(R row)
        {
            AddRow(IndexOf(row));
        }
        private void AddRow(int pos)
        {
            AddRow(pos, CellFactory.NewRow(pos), CellFactory.NewValue);
        }
        private void AddRow(int rowIndex, R row, V val)  
        {
            OffsetRows(rowIndex, 1);
            AddRowCells();
            OnRowAdded?.Invoke(row);

            void AddRowCells()
            {
                for (int c = 0; c < Cols.Length; c++)
                {
                    C col = Cols[c];
                    Coords coords = Coords.Of(rowIndex, c);
                    var cell = CellFactory.NewCell(coords, row, col, val);
                    AddCell(cell);
                }
            }
        }


        protected virtual void CellRemoved(MtxCell<R, C, V> removed)
        {
            removed.OnValueUpdated -= Cell_OnValueUpdate;
        }
        protected virtual void CellAdded(MtxCell<R, C, V> added)
        {
            added.OnValueUpdated += Cell_OnValueUpdate;
        }
        private void Cell_OnValueUpdate(MtxCell<R, C, V> obj)
        {
            OnValuesChanged?.Invoke(obj.Coords);
        }


        //Сдвинуть координаты ячеек на указанное значения
        private void OffsetRows(int begin, int offset)
        {
            var cells = Source.Where(c => c.Coords.X >= begin);
            foreach (var cell in cells)
            {
                int x = cell.Coords.X + offset;
                int y = cell.Coords.Y;
                cell.Coords = Coords.Of(x, y);
            }
            //Offset(c => c.X, c => c.Y, begin, offset, true);
        }
        private void OffsetCols(int begin, int offset)
        {
            var cells = Source.Where(c => c.Coords.Y >= begin);
            foreach (var cell in cells)
            {
                int x = cell.Coords.X;
                int y = cell.Coords.Y + offset;
                cell.Coords = Coords.Of(x, y);
            }
            //Offset(c => c.Y, c => c.X, begin, offset, false);
        }

        private void Offset(Func<Coords, int> prim, Func<Coords, int> sec, int begin, int offset, bool firstPrimary)
        {
            var cells = Source.Where(c => prim.Invoke(c.Coords) >= begin);
            foreach (var cell in cells)
            {
                int secondary = sec.Invoke(cell.Coords);
                int primary = prim.Invoke(cell.Coords) + offset;
                if (firstPrimary)
                {
                    cell.Coords = Coords.Of(primary, secondary);
                }
                else
                {
                    cell.Coords = Coords.Of(secondary, primary);
                }
            }
        }

        //Получение
        public V Get(int row, int col) => Source.First(s => s.Coords.X == row && s.Coords.Y == col).Value;
        public C GetCol(int pos) => Source.First(s => s.Coords.Y == pos).To;
        public R GetRow(int pos) => Source.First(s => s.Coords.X == pos).From;
        public MtxCell<R, C, V> GetCell(int x, int y)
        {
            return Source.First(s => s.Coords.X == x && s.Coords.Y == y);
        }
        public MtxCell<R, C, V> GetCell(R row, C col)
        {
            return Source.First(s => s.From.Equals(row) && s.To.Equals(col));
        }

        //Удаление
        public void RemoveCol(int col)
        {
            RemoveCol(Cols[col]);
        }
        public void RemoveCol(C col)
        {
            int index = IndexOf(col);
            OffsetCols(index + 1, -1);
            Source.RemoveAll(c => c.To.Equals(col));
            OnColRemoved?.Invoke(col);
        }
        public void RemoveRow(int row)
        {
            RemoveRow(Rows[row]);
        }
        public void RemoveRow(R row)
        {
            int index = IndexOf(row);
            OffsetRows(index + 1, -1);
            Source.RemoveAll(c => c.From.Equals(row));
            OnRowRemoved?.Invoke(row);
        }


        //Изменение значения
        public void Set(int row, int col, V val)
        {
            Set(GetCell(row, col), val);
        }
        public void Set(R row, C col, V val)
        {
            Set(GetCell(row, col), val);
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
                throw new MatrixGetValueException(0, 0);
            }
        }


        public Matrix() : this(new MtxCellFactory<R, C, V>())
        {

        }
        public Matrix(MtxCellFactory<R, C, V> cellFactory)
        {
            CellFactory = cellFactory;
            AddDefaultCell();
        }
        private void AddDefaultCell()
        {
            AddCell(CellFactory.NewCell());
        }
        private MtxCellFactory<R, C, V> CellFactory { get; set; }
    }
}
