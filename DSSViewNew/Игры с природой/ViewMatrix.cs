using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DSSLib;

namespace DSSView
{
        //Представление приложения
        //- Коллекция статистических игр

    //Статистическая игра
        //- Матрица
            //- Действия
            //- Случаи
        //- Итоги по критериям (использует матрицу)
            //- Вальд
            //- Гурвиц...
        //- Текстовый отчет (использует итоги и матрицу)

    public class ViewMatrix : NotifyObj
    {
        //public ObservableCollection<PayMatrixView> ListMatrix { get; private set; }
        //public PayMatrixView Selected
        //{
        //    get => selected;
        //    set
        //    {
        //        selected = value;
        //        OnPropertyChanged();
        //    }
        //}
        //private PayMatrixView selected;

        public ObservableCollection<StatGameView> Games { get; private set; }
        public StatGameView SelectedNew
        {
            get => selectedNew;
            set
            {
                selectedNew = value;
                OnPropertyChanged();
            }
        }
        private StatGameView selectedNew;

        private ISaver<StatGame> Saver { get; set; }




        public ICommand ShowAddMatrixWindowCommand { get; private set; }
        public ICommand AddMatrixCommand { get; private set; }
        public ICommand SaveAsMatrixCommand { get; private set; }
        public ICommand SaveMatrixCommand { get; private set; }
        public ICommand OpenMatrixCommand { get; private set; }
        public ICommand CloseMatrixCommand { get; private set; }
        public ICommand CreateReportCommand { get; private set; }


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
                StatGame newGame = new StatGame(info.Name, MtxStat.CreateFromSize(info.Rows, info.Cols));
                Games.Add(new StatGameView(newGame));
                SelectedNew = Games.Last();

                //ListMatrix.Add(new PayMatrixView(new PayMatrixRisc(info.Rows, info.Cols)));
                //Selected = ListMatrix.Last();
            }
        }
        //public void AddSafeMatrix(PayMatrix matrix)
        //{
        //    //ListMatrix.Add(new PayMatrixView(matrix));
        //    //Selected = ListMatrix.Last();
        //}
        private void CloseMatrix(object obj)
        {
            var oldSelected = SelectedNew;
            int index = Games.IndexOf(oldSelected);
            if (index > 0)
                SelectedNew = Games[index - 1];
            else
                SelectedNew = null;
            Games.Remove(oldSelected);


            //PayMatrixView oldSelected = Selected;
            //int index = ListMatrix.IndexOf(oldSelected);
            //if (index > 0)
            //    Selected = ListMatrix[index - 1];
            //else
            //    Selected = null;
            //ListMatrix.Remove(oldSelected);
        }

        private bool IsSavingAvailable(object obj) => false; //SelectedNew != null && SelectedNew.File != null && Selected.File.Exists;
        private void SaveMatrix(object obj)
        {
            //if(IsSavingAvailable(null))
            //{
            //    string path = selected.File.FullName;
            //    var matrix = selected.Matrix;
            //    Saver.Save(path, matrix);
            //}
        }
        private void SaveAsMatrix(object obj)
        {
            Saver.Save(SelectedNew.Source);
        }
        private void OpenMatrix(object obj)
        {
            var newMatrix = Saver.Open();
            if (newMatrix != null)
            {
                Games.Add(new StatGameView(newMatrix));
                SelectedNew = Games.Last();
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
            Saver = XmlProvider.Get<StatGame>();

            ShowAddMatrixWindowCommand = new RelayCommand(ShowAddMatrixWindow, obj => true);
            AddMatrixCommand = new RelayCommand(AddMatrix, obj => true);
            CloseMatrixCommand = new RelayCommand(CloseMatrix, obj => SelectedNew != null);
            OpenMatrixCommand = new RelayCommand(OpenMatrix, obj => true);
            SaveMatrixCommand = new RelayCommand(SaveMatrix, IsSavingAvailable);
            SaveAsMatrixCommand = new RelayCommand(SaveAsMatrix, obj => SelectedNew != null);
            CreateReportCommand = new RelayCommand(CreateReport, obj => SelectedNew != null);


            //ListMatrix = new ObservableCollection<PayMatrixView>();
            //AddMatrix(new NewGameInfo());
            //Selected = ListMatrix.Last();

            Games = new ObservableCollection<StatGameView>();
            AddMatrix(new NewGameInfo() { Name = "Стартовая", Rows = 3, Cols = 3 });
        }
    }
    
    public class NewGameInfo
    {
        public string Name { get; set; } = "Матрица";
        public int Rows { get; set; } = 3;
        public int Cols { get; set; } = 3;
    }
}
