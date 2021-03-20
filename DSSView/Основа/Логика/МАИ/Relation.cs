using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class Relation<T,C> : NotifyObj where T:class
    {
        public override string ToString()
        {
            if (Value > 1)
                return $"'{From}' лучше '{To}' в {Value} раз";
            else if (Value == 1)
                return $"'{From}' и '{To}' равны";
            else
                return $"'{From}' хуже '{To}' в {1 / Value} раз";
        }
        public event Action<Relation<T,C>> Changed;

        public C Main { get; private set; }

        public T From { get; private set; }
        public T To { get; private set; }


        public void SetFromTo(C main,T first, T to)
        {
            From = first;
            To = to;
        }

        public bool Inited => From != null && To != null;
        public bool Self => Inited && From == To;

        public double Value
        {
            get => Self ? 1 : value;
            set
            {
                if (value < MinValue)
                    value = MinValue;
                else if (value > MaxValue)
                    value = MaxValue;

                this.value = value;
                OnPropertyChanged();
                Changed?.Invoke(this);
            }
        }
        protected double value;

        private static double MinValue { get; set; } = 0;
        private static double MaxValue { get; set; } = 100;


        public void SetValueMirror(double mirrored)
        {
            value = mirrored;
            OnPropertyChanged(nameof(Value));
        }

        public Relation(C main,T from, T to, double val)
        {
            Main = main;
            From = from;
            To = to;
            Value = val;
        }
    }
}
