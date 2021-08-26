using DSSLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class Matrix<R, C, V>
    {
        public override string ToString() => $"Матрица {RowsCount}x{ColsCount} ({typeof(R).Name}-{typeof(C).Name}-{typeof(V).Name})";

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

        public bool IsEmpty => Source.Count == 0;
        protected void Clear()
        {
            Source.Clear();
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
        public int RowsCount => !IsEmpty ? Source.Max(s => s.Coords.X) + 1 : 0;
        public int ColsCount => !IsEmpty ? Source.Max(s => s.Coords.Y) + 1 : 0;

        //Позиция строк и столбцов
        private int IndexOf(R row) => Source.First(cell => cell.From.Equals(row)).Coords.X;
        private int IndexOf(C col) => Source.First(cell => cell.To.Equals(col)).Coords.Y;


        protected void Add(MtxCell<R, C, V> cell)
        {
            Source.Add(cell);
        }
        //Добавление столбца
        public void AddCol() => AddCol(ColsCount);
        public void AddColIndex(C col) => AddCol(IndexOf(col));
        public void AddCol(C col, V val) => AddCol(ColsCount, col, val);
        public void AddCol(int pos) => AddCol(pos, CellFactory.NewCol, CellFactory.NewValue);
        public void AddCol(int pos, C col, V val)
        {
            var rows = Rows;
            int colIndex = pos;
            OffsetCols(colIndex, 1);
            for (int i = 0; i < rows.Length; i++)
            {
                R row = rows[i];
                Coords coords = new Coords(i, colIndex);
                var cell = CellFactory.NewCell(coords, row, col, val);
                Source.Add(cell);
                CellAdded(cell);
            }
            OnColChanged?.Invoke(col);
        }

        //Добавление строки
        public void AddRow() => AddRow(RowsCount);
        public void AddRowIndex(R row) => AddRow(IndexOf(row));
        public void AddRow(R row, V val) => AddRow(RowsCount, row, val);
        public void AddRow(int pos) => AddRow(pos, CellFactory.NewRow, CellFactory.NewValue);
        public void AddRow(int pos, R row, V val)
        {
            var cols = Cols;
            int rowIndex = pos;
            OffsetRows(rowIndex, 1);
            for (int i = 0; i < cols.Length; i++)
            {
                C col = cols[i];
                Coords coords = new Coords(rowIndex, i);
                var cell = CellFactory.NewCell(coords, row, col, val);
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
}
