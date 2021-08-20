using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class Relation<T,C> : DSSLib.NotifyObj where T:class
    {
        public override string ToString()
        {
            if (Value > 1)
                return $"'{From}' лучше '{To}' в {Value} раз по {Main}";
            else if (Value == 1)
                return $"'{From}' и '{To}' равны по {Main}";
            else
                return $"'{From}' хуже '{To}' в {1 / Value} раз по {Main}";
        }
        public event Action<Relation<T,C>> Changed;

        public C Main { get; private set; }

        public T From { get; private set; }
        public T To { get; private set; }


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
                if (Mirrored != null)
                    Mirrored.SetValueMirror(1 / value);


                OnPropertyChanged();
                OnPropertyChanged(nameof(Results));
                Changed?.Invoke(this);
            }
        }
        protected double value;

        public string Results => NodeRelation.GetTextRelationFor(Value);


        private static double MinValue { get; set; } = 0;
        private static double MaxValue { get; set; } = 100;



        public Relation<T,C> Mirrored { get; set; }

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
    public class NodeRelation : Relation<Node, Node>
    {
        public new event Action<NodeRelation> Changed;
        public NodeRelation(Node main, Node from, Node to, double val) : base(main, from, to, val)
        {
            base.Changed += AlphaRelation_Changed;
        }

        private void AlphaRelation_Changed(Relation<Node, Node> obj)
        {
            Changed?.Invoke(this);
        }


        public static string GetTextRelationFor(double Value)
        {
            if (Value == 1)
            {
                return $"РАВНОЗНАЧЕН";
            }
            else if(Value > 1)
            {
                switch (Value)
                {
                    case 2:
                        return $"почти РАВНОЗНАЧЕН";
                    case 3:
                        return $"НЕМНОГО ПРЕОБЛАДАЕТ над";
                    case 4:
                        return $"НЕМНОГО ПРЕОБЛАДАЕТ над";
                    case 5:
                        return $"ПРЕОБЛАДАЕТ над";
                    case 6:
                        return $"ПРЕОБЛАДАЕТ над";
                    case 7:
                        return $"СИЛЬНО ПРЕОБЛАДАЕТ над";
                    case 8:
                        return $"СИЛЬНО ПРЕОБЛАДАЕТ над";
                    case 9:
                        return $"АБСОЛЮТНО ПРЕВОСХОДИТ";
                    default:
                        return "Неизвестное отношение";
                }
            }
            else
            {
                Value = 1 / Value;
                switch (Value)
                {
                    case 2:
                        return $"почти РАВНОЗНАЧЕН";
                    case 3:
                        return $"НЕМНОГО ПРОИГРЫВАЕТ";
                    case 4:
                        return $"НЕМНОГО ПРОИГРЫВАЕТ";
                    case 5:
                        return $"ПРОИГРЫВАЕТ";
                    case 6:
                        return $"ПРОИГРЫВАЕТ";
                    case 7:
                        return $"СИЛЬНО ПРОИГРЫВАЕТ";
                    case 8:
                        return $"СИЛЬНО ПРОИГРЫВАЕТ";
                    case 9:
                        return $"АБСОЛЮТНО ПРОИГРЫВАЕТ";
                    default:
                        return "Неизвестное отношение";
                }
            }
        }


    }
}
