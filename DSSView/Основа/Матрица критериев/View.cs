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
    

    
}
