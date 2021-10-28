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

        void SetUnknown();

        IRating Rating { get; }
        void SetRating(IRating rating);

        string GetTextRelation();
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

        public INode Main { get; private set; }

        public INode From { get; private set; }
        public INode To { get; private set; }


        //Свойства
        private bool Inited => From != null && To != null;
        public bool Self => Inited && From == To;
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
        public bool Unknown => value == 0;




        public NodeRelation(INode criteria, INode from, INode to, double val)
        {
            Main = criteria;
            From = from;
            To = to;
            Value = val;
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


        public string GetTextRelation()
        {
            if (Unknown)
            {
                return "??????";
            }

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
                double testValue = 1 / Value;
                if (testValue >= 9)
                    return "АБСОЛЮТНО ПРОИГРЫВАЕТ";
                else if (testValue >= 8)
                    return "СИЛЬНО ПРОИГРЫВАЕТ";
                else if (testValue >= 7)
                    return "СИЛЬНО ПРОИГРЫВАЕТ";
                else if (testValue >= 6)
                    return "ПРОИГРЫВАЕТ";
                else if (testValue >= 5)
                    return "ПРОИГРЫВАЕТ";
                else if (testValue >= 4)
                    return "НЕМНОГО ПРОИГРЫВАЕТ";
                else if (testValue >= 3)
                    return "НЕМНОГО ПРОИГРЫВАЕТ";
                else if (testValue >= 2)
                    return "СЛЕГКА ПРОИГРЫВАЕТ";
                else
                    return "СЛЕГКА ПРОИГРЫВАЕТ";
            }
        }
    }

    public class RelationPair
    {
        public readonly NodeRelation FromRelation;
        public readonly NodeRelation ToRelation;

        public INode MainNode => FromRelation.Main;
        public INode FromNode => FromRelation.From;
        public INode ToNode => FromRelation.To;

        public IRating Rating { get; set; }
        public void SetRating(IRating rating)
        {

        }

        public RelationPair(NodeRelation from, NodeRelation to)
        {
            FromRelation = from;
            ToRelation = to;
        }
        public RelationPair(NodeRelation main)
        {
            FromRelation = main;
            ToRelation = main.Mirrored as NodeRelation;
        }
    }
}
