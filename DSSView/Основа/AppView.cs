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
            Name = "Платежная матрица";
            Tabs = new ObservableCollection<ITab>();
            
            Tab creator = new TabMatrixCreator();
            Tabs.Add(creator);

            Selected = Tabs[0];
        }
    }



    //Структурный паттерн Компоновщик
    //Вкладки
    interface ITab : INotifyPropertyChanged
    {
        string Name { get; }
        string Tooltip { get; }

        ColorInfo Color { get; }


        object Object { get; }

        void Add(ITab tab);
        void Remove(ITab tab);
    }


    //Обычная вкладка
    class Tab : NotifyObj, ITab
    {
        public string Name { get; set; }
        public virtual string Tooltip => Object != null ? Object.ToString() : "null";
        public ColorInfo Color { get; set; }

        public ObservableCollection<ITab> Children { get; set; }
        public bool IsExpanded { get; set; }

        public object Object
        {
            get => Obj;
            set
            {
                Obj = value;
                OnPropertyChanged();
            }
        }
        private object Obj;

        public Tab(string name, object obj = null)
        {
            Name = name;
            Object = obj;
            Children = new ObservableCollection<ITab>();

            if (obj == null)
                Object = this;
        }

        public void Add(ITab tab)
        {
            Children.Add(tab);
        }
        public void Remove(ITab tab)
        {
            Children.Remove(tab);
        }
    }

    //Вкладка создания новых матриц
    class TabMatrixCreator : Tab
    {
        public override string Tooltip => "Список матриц";
        public InfoMatrix Info { get; set; }

        public RelayCommand AddMatrixCommand { get; set; }
        private void AddMatrix(object obj)
        {
            InfoMatrix info = obj as InfoMatrix;
            Children.Add(new DataView(info.Rows, info.Cols));
        }

        public TabMatrixCreator() : base("Создание матриц")
        {
            Info = new InfoMatrix() { Name = "Матрица", Rows = 2, Cols = 3 };
            AddMatrixCommand = new RelayCommand(AddMatrix, obj => true);
        }
    }
    class InfoMatrix
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
