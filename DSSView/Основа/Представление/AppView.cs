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


    //Вспомогательные интерфейсы
    class NotifyObj : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }


    //Экземпляр приложения
    class View : NotifyObj
    {
        public static View Ex { get; set; }


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


        public RelayCommand ShowAddMatrixWindowCommand { get; set; }
        public RelayCommand AddMatrixCommand { get; set; }
        public RelayCommand CloseMatrixCommand { get; set; }

        private void ShowAddMatrixWindow(object obj)
        {
            NewMatrixWindow window = new NewMatrixWindow();
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

        public View()
        {
            Ex = this;

            ShowAddMatrixWindowCommand = new RelayCommand(ShowAddMatrixWindow, obj => true);
            AddMatrixCommand = new RelayCommand(AddMatrix, obj => InfoNewMatrix != null);
            CloseMatrixCommand = new RelayCommand(CloseMatrix, obj => Selected != null);

            
            InfoNewMatrix = new InfoAboutMatrix() { Name = "Матрица", Rows = 3, Cols = 3 };
            ListMatrix = new ObservableCollection<PayMatrixView>()
            {
                new PayMatrixView(new PayMatrixRisc(3,3))
            };
            Selected = ListMatrix.Last();
        }
    }
    
    class InfoAboutMatrix
    {
        public string Name { get; set; }
        public int Rows { get; set; }
        public int Cols { get; set; }
    }
}
