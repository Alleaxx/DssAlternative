using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    /// <summary>
    /// Визуализированные и редактируемая матрица
    /// </summary>
    class DataView : Data, ITab, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public ObservableCollection<MatrixRow> RowsInfo { get; set; }
        public ObservableCollection<MatrixCol> ColumsInfo { get; set; }

        public ObservableCollection<MatrixRow> SafeRowsInfo { get; set; }



        public DataView(int rows, int cols) : base(rows, cols)
        {
            CreateRowsColumns();
            SafeData = new DataView(this);


            InfoCriterias = new InfoCriteriasView(this);
            Reports = new ObservableCollection<ITab>();
            Add(InfoCriterias as InfoCriteriasView);

            if(SafeData != null)
                Add(SafeData as DataView);
        }
        public DataView(DataView data) : base(data)
        {
            Information = new InfoData("Матрица наименьшего риска", Rows, Cols);
            MainMatrix = data;
            UpdateFromMain();
            InfoCriterias = new InfoCriteriasView(this);
            Reports = new ObservableCollection<ITab>();
            Add(InfoCriterias as InfoCriteriasView);
            CreateRowsColumns();
        }
        protected override void UpdateFromMain()
        {
            base.UpdateFromMain();
        }

        private void CreateRowsColumns()
        {
            RowsInfo = new ObservableCollection<MatrixRow>();
            for (int i = 0; i < Rows; i++)
            {
                RowsInfo.Add(new MatrixRow(this, i));
            }
            ColumsInfo = new ObservableCollection<MatrixCol>();
            for (int i = 0; i < Cols; i++)
            {
                ColumsInfo.Add(new MatrixCol(this, i));
            }
        }


        public override void SetValue(Coords coords, double value)
        {
            base.SetValue(coords, value);
        }


        //Интерфейс вкладки
        public string Name => Information.Name;
        public string Tooltip => Information.Name;
        public ColorInfo Color => new ColorInfo();
        public object Object => this;

        public ObservableCollection<ITab> Reports { get; set; }

        public void Add(ITab tab)
        {
            Reports.Add(tab);
        }
        public void Remove(ITab tab)
        {
            Reports.Remove(tab);
        }
    }
    
    //Содержимое матрицы
    abstract class MatrixContent
    { 
        public DataView Matrix { get; set; }

        public virtual int Position { get; set; }
        public virtual List<Cell> Cells { get; set; }



        public MatrixContent(DataView data,int pos, bool newInfo)
        {
            Position = pos;
            Matrix = data;
            Cells = new List<Cell>();

            if (newInfo)
                AutoFill();
            else
                ManualFill();
        }
        public abstract void ManualFill();
        public abstract void AutoFill();


    }

    /// <summary>
    /// Строка матрицы
    /// </summary>
    class MatrixRow : MatrixContent
    {
        public MatrixRow(DataView data,int pos,bool newV = false) : base(data, pos, newV) { }
        public override void AutoFill()
        {
            for (int i = 0; i < Matrix.Rows; i++)
            {
                Cells.Add(new CellValue(Matrix, 0, new Coords(Position,i)));
            }
        }
        public override void ManualFill()
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
        public MatrixCol(DataView data,int pos,bool newV = false) : base(data, pos, newV) { }
        public override void AutoFill()
        {
            for (int i = 0; i < Matrix.Cols; i++)
            {
                Cells.Add(new CellValue(Matrix, 0, new Coords(i,Position)));
            }
        }
        public override void ManualFill()
        {
            int colPos = Position;
            for (int rowPos = 0; rowPos < Matrix.Rows; rowPos++)
            {
                Cells.Add(new CellValue(Matrix, Matrix.Matrix[rowPos,colPos], new Coords(rowPos,colPos)));
            }
        }
    }

    //class MatrixContentEdit : MatrixContent
    //{
    //    protected MatrixContent Component { get; set; }

    //    public override List<Cell> Cells { get => Component.Cells; set => Component.Cells = value; }
    //    public override int Position { get => Component.Position; set => Component.Position = value; }

    //    public MatrixContentEdit(MatrixContent comp,bool newV = false) : base(comp.Matrix,comp.Position,newV)
    //    {
    //        Component = comp;
    //    }

    //    public override void ManualFill()
    //    {
    //        Component.ManualFill();
    //    }
    //    public override void AutoFill()
    //    {
    //        Component.AutoFill();
    //    }
    //}





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
    /// Редактируемая ячейка матрицы
    /// </summary>
    class CellValue : Cell, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public bool IsEditable { get; set; }

        public override double Value
        {
            get => Matrix.Matrix[Coords.X, Coords.Y];
            set
            {
                if(Matrix != null)
                {
                    Matrix.SetValue(Coords, value);
                    OnPropertyChanged();
                }
            }
        }

        public CellValue(DataView matrix,double value, Coords coords)
        {
            Matrix = matrix;
            Value = value;
            Coords = coords;
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
    
}
