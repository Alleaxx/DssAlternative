using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSLib;

namespace DSSView
{

    public class ViewMatrix : NotifyObj
    {
        public ObservableCollection<PayMatrixView> ListMatrix { get; private set; }
        public PayMatrixView Selected
        {
            get => selected;
            set
            {
                selected = value;
                OnPropertyChanged();
            }
        }
        private PayMatrixView selected;

        private ISaver<PayMatrix> Saver { get; set; }




        public RelayCommand ShowAddMatrixWindowCommand { get; private set; }
        public RelayCommand AddMatrixCommand { get; private set; }
        public RelayCommand SaveAsMatrixCommand { get; private set; }
        public RelayCommand SaveMatrixCommand { get; private set; }
        public RelayCommand OpenMatrixCommand { get; private set; }
        public RelayCommand CloseMatrixCommand { get; private set; }
        public RelayCommand CreateReportCommand { get; private set; }


        private void ShowAddMatrixWindow(object obj)
        {
            NewGameInfo info = new NewGameInfo();
            NewMatrixWindow window = new NewMatrixWindow(info);
            if(window.ShowDialog() == true)
            {
                AddMatrix(info);
            }
        }
        private void AddMatrix(object obj)
        {
            if(obj is NewGameInfo info)
            {
                ListMatrix.Add(new PayMatrixView(new PayMatrixRisc(info.Rows, info.Cols)));
                Selected = ListMatrix.Last();
            }
        }
        public void AddSafeMatrix(PayMatrix matrix)
        {
            ListMatrix.Add(new PayMatrixView(matrix));
            Selected = ListMatrix.Last();
        }
        private void CloseMatrix(object obj)
        {
            PayMatrixView oldSelected = Selected;
            int index = ListMatrix.IndexOf(oldSelected);
            if (index > 0)
                Selected = ListMatrix[index - 1];
            else
                Selected = null;
            ListMatrix.Remove(oldSelected);
        }

        private bool IsSavingAvailable(object obj) => Selected != null && Selected.File != null && Selected.File.Exists;
        private void SaveMatrix(object obj)
        {
            if(IsSavingAvailable(null))
            {
                string path = selected.File.FullName;
                var matrix = selected.Matrix;
                Saver.Save(path, matrix);
            }
        }
        private void SaveAsMatrix(object obj)
        {
            Saver.Save(Selected.Matrix);
        }
        private void OpenMatrix(object obj)
        {
            var newMatrix = Saver.Open();
            if (newMatrix != null)
            {
                ListMatrix.Add(new PayMatrixView(newMatrix));
                Selected = ListMatrix.Last();
            }
        }

        private void CreateReport(object obj)
        {
            //IReport report = Report.GetReport(Selected.Matrix, (Report.ReportType)obj);

            //report.Create();
            //report.Open();
        }


        public ViewMatrix()
        {
            Saver = XmlProvider.Get<PayMatrix>();

            ShowAddMatrixWindowCommand = new RelayCommand(ShowAddMatrixWindow, obj => true);
            AddMatrixCommand = new RelayCommand(AddMatrix, obj => true);
            CloseMatrixCommand = new RelayCommand(CloseMatrix, obj => Selected != null);
            OpenMatrixCommand = new RelayCommand(OpenMatrix, obj => true);
            SaveMatrixCommand = new RelayCommand(SaveMatrix, obj => Selected != null && Selected.File != null && Selected.File.Exists);
            SaveAsMatrixCommand = new RelayCommand(SaveAsMatrix, obj => Selected != null);
            CreateReportCommand = new RelayCommand(CreateReport, obj => Selected != null);


            ListMatrix = new ObservableCollection<PayMatrixView>();
            AddMatrix(new NewGameInfo());
            Selected = ListMatrix.Last();
        }
    }
    
    public class NewGameInfo
    {
        public string Name { get; set; } = "Матрица";
        public int Rows { get; set; } = 3;
        public int Cols { get; set; } = 3;
    }
}
