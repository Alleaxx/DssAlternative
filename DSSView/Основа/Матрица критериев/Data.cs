using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{

    /// <summary>
    /// Матрица
    /// </summary>
    class Data
    {
        //Содержание матрицы
        public double[,] Matrix { get; set; }
        public int Rows => Matrix.GetLength(0);
        public int Cols => Matrix.GetLength(1);

        //Информация и критерии
        public InfoData Information { get; set; }
        public InfoCriterias InfoCriterias { get; set; }


        //Столбцы и строки матрицы
        public MatrixRow[] RowsInfo { get; set; }
        public MatrixCol[] ColumsInfo { get; set; }


        //Зависимая матрица
        public Data MainMatrix { get; set; }
        public bool Independent => MainMatrix == null && SafeData != null;
        public Data SafeData { get; set; }


        //Создание независимой матрицы
        public Data(int rows, int cols)
        {
            Matrix = new double[rows,cols];
            for (int i = 0; i < Rows; i++)
            {
                for (int a = 0; a < Cols; a++)
                {
                    Matrix[i,a] = 0;
                }
            }

            CreateInfo();
            CreateRowsColumns();
            CreateCriterias();
            SafeData = new Data(this);
        }
        //Создание зависимой матрицы
        public Data(Data data)
        {
            MainMatrix = data;
            UpdateFromMain();
            CreateInfo();
            CreateRowsColumns();
            CreateCriterias();
            Information = new InfoData("", Rows, Cols);
        }

        private void CreateInfo()
        {
            if (!Independent)
                Information = new InfoData("Матрица наименьшего риска", Rows, Cols);
            else
                Information = new InfoData($"Матрица {Rows}x{Cols}", Rows,Cols);
        }
        private void CreateRowsColumns()
        {
            RowsInfo = new MatrixRow[Rows];
            for (int i = 0; i < Rows; i++)
            {
                RowsInfo[i] = new MatrixRow(this, i);
            }
            ColumsInfo = new MatrixCol[Cols];
            for (int i = 0; i < Cols; i++)
            {
                ColumsInfo[i] = new MatrixCol(this, i);
            }
        }
        private void CreateCriterias()
        {
            InfoCriterias = new InfoCriterias(this);
        }


        //Обновить содержимое зависимой матрицы
        protected virtual void UpdateFromMain()
        {
            Matrix = new double[MainMatrix.Rows, MainMatrix.Cols];

            double max = double.MinValue;

            for (int x = 0; x < Rows; x++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (MainMatrix.Matrix[x, c] > max)
                        max = MainMatrix.Matrix[x, c];
                }
            }

            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Cols; y++)
                {
                    Matrix[x, y] = MainMatrix.Matrix[x, y] * -1 + max;
                }
            }

            if(InfoCriterias != null)
                InfoCriterias.Update();
        }

        //Обновить значение в матрице
        public virtual void SetValue(Coords coords, double value)
        {
            Matrix[coords.X, coords.Y] = value;

            if (Independent)
                SafeData.UpdateFromMain();
            if (InfoCriterias != null)
                InfoCriterias.Update();
        }
    }



    //Содержимое матрицы
    abstract class MatrixContent
    { 
        public Data Matrix { get; set; }

        public virtual int Position { get; set; }
        public virtual List<Cell> Cells { get; set; }



        public MatrixContent(Data data,int pos, bool newInfo)
        {
            Position = pos;
            Matrix = data;
            Cells = new List<Cell>();

            if (newInfo)
                AutoFill();
            else
                ManualFill();
        }
        protected abstract void ManualFill();
        protected abstract void AutoFill();


    }

    /// <summary>
    /// Строка матрицы
    /// </summary>
    class MatrixRow : MatrixContent
    {
        public MatrixRow(Data data,int pos,bool newV = false) : base(data, pos, newV) { }
        protected override void AutoFill()
        {
            for (int i = 0; i < Matrix.Rows; i++)
            {
                Cells.Add(new CellValue(Matrix, 0, new Coords(Position,i)));
            }
        }
        protected override void ManualFill()
        {
            int rowPos = Position;
            for (int colPos = 0; colPos < Matrix.Cols; colPos++)
            {
                Cells.Add(new CellValue(Matrix, Matrix.Matrix[rowPos,colPos], new Coords(rowPos,colPos)));
            }
        }
    }

   /// <summary>
   /// Столбец матрицы
   /// </summary>
    class MatrixCol : MatrixContent
    {
        public MatrixCol(Data data,int pos,bool newV = false) : base(data, pos, newV) { }
        protected override void AutoFill()
        {
            for (int i = 0; i < Matrix.Cols; i++)
            {
                Cells.Add(new CellValue(Matrix, 0, new Coords(i,Position)));
            }
        }
        protected override void ManualFill()
        {
            int colPos = Position;
            for (int rowPos = 0; rowPos < Matrix.Rows; rowPos++)
            {
                Cells.Add(new CellValue(Matrix, Matrix.Matrix[rowPos,colPos], new Coords(rowPos,colPos)));
            }
        }
    }





    /// <summary>
    /// Ячейка матрицы
    /// </summary>
    abstract class Cell
    {
        public Data Matrix { get; set; }

        public Coords Coords { get; set; } = new Coords(0, 0);
        public virtual double Value { get; set; }
    }  

    /// <summary>
    /// Ячейка матрицы
    /// </summary>
    class CellValue : Cell
    {
        public override double Value
        {
            get => Matrix.Matrix[Coords.X, Coords.Y];
            set
            {
                Matrix.SetValue(Coords, value);
            }
        }

        public CellValue(Data matrix,double value, Coords coords)
        {
            Matrix = matrix;
            Value = value;
            Coords = coords;
        }
    }
    class CellHeader : Cell
    {

    }


    /// <summary>
    /// Редактируемая ячейка матрицы
    /// </summary>
    class CellView : NotifyObj
    {
        public Cell Cell { get; set; }

        public bool IsEditable { get; set; }

        public double Value
        {
            get => Cell.Value;
            set
            {
                Cell.Value = value;
                OnPropertyChanged();
            }
        }

        public CellView(Cell cell)
        {
            Cell = cell;
            IsEditable = true;
        }
    }




    /// <summary>
    /// Координаты ячейки
    /// </summary>
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


    /// <summary>
    /// Дополнительная информация о матрице
    /// </summary>
    class InfoData
    {
        public string Name { get; set; }
        public string[] Rows { get; set; }
        public string[] Cols { get; set; }

        public InfoData(string name, int rows, int cols)
        {
            Name = name;
            Rows = new string[rows];
            for (int i = 0; i < rows; i++)
            {
                Rows[i] = $"Строка {i}";
            }
            Cols = new string[cols];
            for (int i = 0; i < cols; i++)
            {
                Cols[i] = $"Столбец {i}";
            }
        }
    }

}
