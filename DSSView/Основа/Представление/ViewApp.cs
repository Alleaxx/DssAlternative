using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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

        public ViewMatrix ViewMatrix { get; set; }
        public ViewTree ViewTree { get; set; }
        public ViewAHP ViewAHP { get; set; }


        public RelayCommand OpenMatrixWindow { get; set; }
        public RelayCommand OpenTreeWindow { get; set; }
        public RelayCommand OpenAHPWindow { get; set; }


        private void OpenMatrix(object obj)
        {
            MatrixViewWindow window = new MatrixViewWindow();
            window.Show();
        }
        private void OpenTree(object obj)
        {
            TreeViewWindow window = new TreeViewWindow(ViewTree);
            window.Show();
        }
        private void OpenAHP(object obj)
        {
            AHPWindow window = new AHPWindow(ViewAHP);
            window.Show();
        }

        public View()
        {
            Ex = this;
            ViewMatrix = new ViewMatrix();
            ViewTree = new ViewTree();
            OpenMatrixWindow = new RelayCommand(OpenMatrix, obj => true);
            OpenTreeWindow = new RelayCommand(OpenTree, obj => true);
            OpenAHPWindow = new RelayCommand(OpenAHP, obj => true);
        }
    }
}
