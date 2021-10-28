using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DSSLib;

using DSSCriterias.Logic;
namespace DSSAHP
{
    public class ViewMatrix : NotifyObj
    {
        public ObservableCollection<StatGameView> Games { get; private set; }
        public StatGameView Selected
        {
            get => selected;
            set
            {
                selected = value;
                OnPropertyChanged();
            }
        }
        private StatGameView selected;

        private readonly ISaver<StatGame> Saver;




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
                Selected = Games.Last();
            }
        }
        private void CloseMatrix(object obj)
        {
            StatGameView oldSelected = Selected;
            int index = Games.IndexOf(oldSelected);
            if (index > 0)
            {
                Selected = Games[index - 1];
            }
            else
            {
                Selected = null;
            }
            Games.Remove(oldSelected);
        }

        private bool IsSavingAvailable(object obj) => Selected != null;
        private void SaveMatrix(object obj)
        {

        }
        private void SaveAsMatrix(object obj)
        {
            Saver.Save(Selected.Source);
        }
        private void OpenMatrix(object obj)
        {
            StatGame newMatrix = Saver.Open();
            if (newMatrix != null)
            {
                Games.Add(new StatGameView(newMatrix));
                Selected = Games.Last();
            }
        }

        private void CreateReport(object obj)
        {

        }


        public ViewMatrix()
        {
            Saver = SaverProvider.Get<StatGame>();

            Games = new ObservableCollection<StatGameView>();
            AddMatrix(new NewGameInfo(3,3, "Стартовая матрица"));
        }
        protected override void InitCommands()
        {
            ShowAddMatrixWindowCommand = new RelayCommand(ShowAddMatrixWindow, obj => true);
            AddMatrixCommand = new RelayCommand(AddMatrix, obj => true);
            CloseMatrixCommand = new RelayCommand(CloseMatrix, obj => Selected != null);
            OpenMatrixCommand = new RelayCommand(OpenMatrix, obj => true);
            SaveMatrixCommand = new RelayCommand(SaveMatrix, IsSavingAvailable);
            SaveAsMatrixCommand = new RelayCommand(SaveAsMatrix, obj => Selected != null);
            CreateReportCommand = new RelayCommand(CreateReport, obj => Selected != null);
        }
    }
}
