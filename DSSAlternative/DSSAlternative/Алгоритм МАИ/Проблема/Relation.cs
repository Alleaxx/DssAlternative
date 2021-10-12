using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface INodeRelation
    {
        INode Main { get; }
        INode From { get; }
        INode To { get; }

        bool Self { get; }
        INodeRelation Mirrored { get; set; }
        bool Unknown { get;  }
        double Value { get; set; }

        void Clear();

        IRating Rating { get; }
        void SetRating(IRating rating);

        string GetTextRelation();
    }
    public class NodeRelation : INodeRelation
    {
        public override string ToString()
        {
            if (Value > 1)
            {
                return $"'{From}' лучше '{To}' в {Value} раз по {Main}";
            }
            else if (Value == 1)
            {
                return $"'{From}' и '{To}' равны по {Main}";
            }
            else
            {
                return $"'{From}' хуже '{To}' в {1 / Value} раз по {Main}";
            }
        }
        public event Action<INodeRelation> Changed;

        public INode Main { get; private set; }

        public INode From { get; private set; }
        public INode To { get; private set; }



        private bool Inited => From != null && To != null;
        public bool Self => Inited && From == To;


        public double Value
        {
            get => Self ? 1 : value;
            set
            {
                SetValue();
                SetMirrored();
                Changed?.Invoke(this);

                void SetValue()
                {

                    if (value < MinValue)
                    {
                        value = MinValue;
                    }
                    else if (value > MaxValue)
                    {
                        value = MaxValue;
                    }
                    this.value = value;
                }
                void SetMirrored()
                {
                    if (Mirrored != null && Mirrored is NodeRelation rel)
                    {
                        if (value == 0)
                            rel.value = 0;
                        else
                            rel.value = 1 / value;
                    }
                }
            }
        }
        protected double value;
        public bool Unknown => value == 0;

        private double MinValue { get; set; } = 0;
        private double MaxValue { get; set; } = 9;

        public INodeRelation Mirrored { get; set; }



        public NodeRelation(INode criteria, INode from, INode to, double val)
        {
            Main = criteria;
            From = from;
            To = to;
            Value = val;
        }


        public IRating Rating => rating ??= CreateRating();
        private IRating rating;
        private IRating CreateRating()
        {
            if (Value == 0)
            {
                return new Rating(0);
            }
            else if (Value < 1)
            {
                return new Rating(To, Mirrored.Value);
            }
            else
            {

                return new Rating(From, Value);
            }
        }
        public void SetRating(IRating rating)
        {
            this.rating = rating;
            INodeRelation rel;
            if (rating.Value == 1 || rating.Value == 0 || rating.Node == From)
            {
                rel = this;
            }
            else
            {
                rel = Mirrored;
            }

            rel.Value = rating.Value;

        }

        public void Clear()
        {
            value = 0;
            rating = CreateRating();
        }

        public string GetTextRelation()
        {
            if (Unknown)
                return "??????";

            if (Value == 1)
            {
                return $"РАВНОЗНАЧЕН";
            }
            else if(Value > 1)
            {
                if (Value >= 9)
                    return "АБСОЛЮТНО ПРЕВОСХОДИТ";
                else if (Value >= 8)
                    return "СИЛЬНО ПРЕОБЛАДАЕТ над";
                else if (Value >= 7)
                    return "СИЛЬНО ПРЕОБЛАДАЕТ над";
                else if (Value >= 6)
                    return "ПРЕОБЛАДАЕТ над";
                else if (Value >= 5)
                    return "ПРЕОБЛАДАЕТ над";
                else if (Value >= 4)
                    return "НЕМНОГО ПРЕОБЛАДАЕТ над";
                else if (Value >= 3)
                    return "НЕМНОГО ПРЕОБЛАДАЕТ над";
                else if (Value >= 2)
                    return "СЛЕГКА ПРЕОБЛАДАЕТ над";
                else
                    return "СЛЕГКА ПРЕОБЛАДАЕТ над";
            }
            else
            {
                Value = 1 / Value;
                if (Value >= 9)
                    return "АБСОЛЮТНО ПРОИГРЫВАЕТ";
                else if (Value >= 8)
                    return "СИЛЬНО ПРОИГРЫВАЕТ";
                else if (Value >= 7)
                    return "СИЛЬНО ПРОИГРЫВАЕТ";
                else if (Value >= 6)
                    return "ПРОИГРЫВАЕТ";
                else if (Value >= 5)
                    return "ПРОИГРЫВАЕТ";
                else if (Value >= 4)
                    return "НЕМНОГО ПРОИГРЫВАЕТ";
                else if (Value >= 3)
                    return "НЕМНОГО ПРОИГРЫВАЕТ";
                else if (Value >= 2)
                    return "СЛЕГКА ПРОИГРЫВАЕТ";
                else
                    return "СЛЕГКА ПРОИГРЫВАЕТ";
            }
        }

    }
}
