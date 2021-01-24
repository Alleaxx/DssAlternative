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
        public List<TabView> Collection { get; set; }
        public TabView Selected
        {
            get => selected;
            set
            {
                selected = value;
                OnPropertyChanged();
            }
        }
        private TabView selected;

        public View()
        {
            Collection = new List<TabView>
            {
                new TabView(),
                new TabView()
            };
        }
    }


    //Раздел
    class TabView : NotifyObj
    {
        public override string ToString() => Name;

        public string Name { get; set; }
        public ObservableCollection<ITab> Tabs { get; set; }

        public ITab Selected
        {
            get => selected;
            set
            {
                selected = value;
                OnPropertyChanged();
            }
        }
        private ITab selected;

        public TabView()
        {
            Name = "Принятие решений в условиях неопределенности и риска";
            Tabs = new ObservableCollection<ITab>();            
            Tabs.Add(new TabMatrixCreator());

            Selected = Tabs[0];
        }
    }



    //Структурный паттерн Компоновщик
    //Вкладки
    interface ITab : INotifyPropertyChanged
    {
        string Name { get; }
        ColorInfo Color { get; }
    }


    //Обычная вкладка
    class Tab : NotifyObj, ITab
    {
        public string Name { get; set; }
        public ColorInfo Color { get; set; }

        public ObservableCollection<ITab> Children { get; set; }
        public bool IsExpanded { get; set; }

        public Tab()
        {
            Children = new ObservableCollection<ITab>();
        }
    }

    //Вкладка создания новых матриц
    class TabMatrixCreator : Tab
    {
        public InfoAboutMatrix Info { get; set; }

        public RelayCommand AddMatrixCommand { get; set; }
        private void AddMatrix(object obj)
        {
            InfoAboutMatrix info = obj as InfoAboutMatrix;
            Children.Add(new ProblemMatrixView(new ProblemPayMatrix(new Matrix(info.Rows,info.Cols))));
        }

        public TabMatrixCreator()
        {
            Info = new InfoAboutMatrix() { Name = "Матрица", Rows = 3, Cols = 3 };
            AddMatrixCommand = new RelayCommand(AddMatrix, obj => true);
        }
    }
    class InfoAboutMatrix
    {
        public string Name { get; set; }
        public int Rows { get; set; }
        public int Cols { get; set; }
    }


    
    class ColorInfo
    {
        public string Background { get; set; }

        public string Main { get; set; } = "black";
        public string Additional { get; set; } = "gray";
    }


}
