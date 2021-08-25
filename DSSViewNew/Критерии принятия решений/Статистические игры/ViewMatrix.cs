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
        public ObservableCollection<PayMatrixView> ListMatrix { get; set; }
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

        public InfoAboutMatrix InfoNewMatrix { get; set; }

        private IXMLProvider<PayMatrix> XmlProvider { get; set; }
        private IFileSelector FileSelector { get; set; }




        public RelayCommand ShowAddMatrixWindowCommand { get; set; }
        public RelayCommand AddMatrixCommand { get; set; }
        public RelayCommand SaveAsMatrixCommand { get; set; }
        public RelayCommand SaveMatrixCommand { get; set; }
        public RelayCommand OpenMatrixCommand { get; set; }
        public RelayCommand CloseMatrixCommand { get; set; }
        public RelayCommand CreateReportCommand { get; set; }


        private void ShowAddMatrixWindow(object obj)
        {
            NewMatrixWindow window = new NewMatrixWindow(this);
            window.ShowDialog();
        }
        private void AddMatrix(object obj)
        {
            ListMatrix.Add(new PayMatrixView(new PayMatrixRisc(InfoNewMatrix.Rows, InfoNewMatrix.Cols)));
            Selected = ListMatrix.Last();
        }
        public void AddSafeMatrix(PayMatrix matrix)
        {
            ListMatrix.Add(new PayMatrixView(matrix));
            Selected = ListMatrix.Last();
        }
        private void CloseMatrix(object obj)
        {
            PayMatrixView oldSelected = Selected;
            if (ListMatrix.Count > 1)
                Selected = ListMatrix.First();
            else
                Selected = null;
            ListMatrix.Remove(oldSelected);
        }

        private void SaveMatrixExe(object obj)
        {
            SaveMatrixToFile(Selected.File.FullName,Selected.Matrix);
        }
        private void SaveAsMatrixExe(object obj)
        {
            FileInfo file = FileSelector.Save();
            if(file != null && file.Exists)
                SaveMatrixToFile(file.FullName,Selected.Matrix);
        }
        private void SaveMatrixToFile(string path,PayMatrix matrix)
        {
            string xml = XmlProvider.ToXml(matrix);
            using(FileStream stream = new FileStream(path, FileMode.Create))
            {
                byte[] array = Encoding.Default.GetBytes(xml);
                stream.Write(array, 0, array.Length);
            }
        }
        private void OpenMatrixExe(object obj)
        {
            FileInfo fromFile = FileSelector.Open();
            if (fromFile != null && fromFile.Exists)
            {
                using(FileStream stream = File.OpenRead(fromFile.FullName))
                {
                    byte[] array = new byte[stream.Length];
                    stream.Read(array, 0, array.Length);
                    string xml = Encoding.Default.GetString(array);
                    PayMatrix newMatrix = XmlProvider.FromXml(xml);
                    ListMatrix.Add(new PayMatrixView(newMatrix,fromFile));
                    Selected = ListMatrix.Last();
                }
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
            XmlProvider = new MatrixProvider();
            FileSelector = new DialogFileSelector();

            ShowAddMatrixWindowCommand = new RelayCommand(ShowAddMatrixWindow, obj => true);
            AddMatrixCommand = new RelayCommand(AddMatrix, obj => InfoNewMatrix != null);
            CloseMatrixCommand = new RelayCommand(CloseMatrix, obj => Selected != null);
            OpenMatrixCommand = new RelayCommand(OpenMatrixExe, obj => true);
            SaveMatrixCommand = new RelayCommand(SaveMatrixExe, obj => Selected != null && Selected.File != null && Selected.File.Exists);
            SaveAsMatrixCommand = new RelayCommand(SaveAsMatrixExe, obj => Selected != null);
            CreateReportCommand = new RelayCommand(CreateReport, obj => Selected != null);

            
            InfoNewMatrix = new InfoAboutMatrix() { Name = "Матрица", Rows = 3, Cols = 3 };
            ListMatrix = new ObservableCollection<PayMatrixView>()
            {
                new PayMatrixView(new PayMatrixRisc(3,3))
            };
            Selected = ListMatrix.Last();
        }
    }
    
    public class InfoAboutMatrix
    {
        public string Name { get; set; }
        public int Rows { get; set; }
        public int Cols { get; set; }
    }
}
