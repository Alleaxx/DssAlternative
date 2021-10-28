using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAHP
{

    public interface IViewElement
    {
        IEnumerable<IViewElement> Elements { get; }
        IViewElement Selected { get; }

        void Add(params IViewElement[] elems);
        void Remove(params IViewElement[] elems);
    }
    public class ViewElement : DSSLib.NotifyObj, IViewElement
    {
        public override string ToString() => Name;

        public virtual IEnumerable<IViewElement> Elements => Collection;
        public IViewElement Selected
        {
            get => selected;
            set
            {
                selected = value;
                OnPropertyChanged();
            }
        }
        private IViewElement selected;

        public string Name { get; set; }

        private ObservableCollection<IViewElement> collection { get; set; } = new ObservableCollection<IViewElement>();
        protected ReadOnlyCollection<IViewElement> Collection { get; set; }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }
        private bool isSelected;


        public ViewElement()
        {
            Collection = new ReadOnlyCollection<IViewElement>(collection);
        }


        protected virtual void Update()
        {
            OnPropertyChanged(nameof(Elements));
        }

        public void Add(params IViewElement[] elems)
        {
            foreach (var elem in elems)
            {
                collection.Add(elem);
            }
        }
        public void Remove(params IViewElement[] elems)
        {
            for (int i = 0; i < elems.Length; i++)
            {
                IViewElement elem = elems[i];
                collection.Remove(elem);
            }
        }
    }


}
