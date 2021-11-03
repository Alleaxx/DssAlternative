using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface INodeRelation
    {
        event Action<INodeRelation> OnChanged;

        INode Main { get; }
        INode From { get; }
        INode To { get; }

        bool Self { get; }
        INodeRelation Mirrored { get; set; }
        bool Unknown { get;  }
        double Value { get; set; }

        void Reflect();
        void SetUnknown();

        IRating Rating { get; }
        void SetRating(IRating rating);
    }
    public class NodeRelation : INodeRelation
    {
        private const double MinValue = 0;
        private const double MaxValue = 9;

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
        
        
        public event Action<INodeRelation> OnChanged;


        public INode Main { get; init; }
        public INode From { get; init; }
        public INode To { get; init; }


        //Свойства
        public bool Self => From == To;
        public INodeRelation Mirrored { get; set; }


        public double Value
        {
            get => Self ? 1 : value;
            set
            {
                SetValue();
                SetMirrored();
                OnChanged?.Invoke(this);

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
                    else
                    {
                        this.value = value;
                    }
                }
                void SetMirrored()
                {
                    if (Mirrored != null && Mirrored is NodeRelation rel)
                    {
                        if (value == 0)
                        {
                            rel.value = 0;
                        }
                        else
                        {
                            rel.value = 1 / value;
                        }
                    }
                }
            }
        }
        protected double value;
        public bool Unknown => Value == 0;
        public void Reflect()
        {
            Value = 1 / Value;
        }


        public NodeRelation(INode criteria, INode from) : this(criteria, from, from, 1)
        {
            Mirrored = this;
        }
        public NodeRelation(INode criteria, INode from, INode to, double val)
        {
            Main = criteria;
            From = from;
            To = to;
            Value = val;
        }


        public static INodeRelation CreateRelation(INode criteria, INode from, INode to, double val)
        {
            return default;
        }


        //Рейтинг на основе значения
        public IRating Rating => CreateRating();
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
            if(rating.Value == 0)
            {
                SetUnknown();
            }
            else if(rating.Value == 1)
            {
                Value = 1;
            }
            else
            {
                INodeRelation relation = rating.Node.Equals(From) ? this : Mirrored;
                relation.Value = rating.Value;
            }
        }
        public void SetUnknown()
        {
            Value = 0;
        }
    }
}
