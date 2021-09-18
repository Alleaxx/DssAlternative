using DSSLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    //Вью-модель
    public class View<T> : NotifyObj
    {
        public override string ToString()
        {
            return $"Представление '{Source}'";
        }

        public T Source
        {
            get => source;
            protected set
            {
                source = value;
                OnPropertyChanged();
            }
        }
        private T source;

        public View(T source)
        {
            Source = source;
            InitCommands();
        }
        protected virtual void InitCommands()
        {

        }
    }
}
