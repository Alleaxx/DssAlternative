using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBlazorEmpty.AHP
{
    public class Relation<T,C> where T:class
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
                Unknown = false;
                if (Mirrored != null)
                    Mirrored.SetValueMirror(1 / value);

                Changed?.Invoke(this);
            }
        }
        protected double value;
        public bool Unknown { get; private set; }

        public string Results => NodeRelation.GetTextRelationFor(Value);


        private static double MinValue { get; set; } = 0;
        private static double MaxValue { get; set; } = 100;



        public Relation<T,C> Mirrored { get; set; }

        public void SetValueMirror(double mirrored)
        {
            Unknown = false;
            value = mirrored;
        }

        public Relation(C main,T from, T to, double val)
        {
            Main = main;
            From = from;
            To = to;
            Value = val;
        }
    }
    public interface INodeRelation
    {
        INode Main { get; }
        INode From { get; }
        INode To { get; }
        bool Self { get; }
        INodeRelation Mirrored { get; set; }
        bool Unknown { get; }
        double Value { get; set; }

        INode Node { get; }
        double Rating { get; }
        void SetRating(INode node, double val);
    }
    public class NodeRelation : Relation<INode, INode>, INodeRelation
    {
        public NodeRelation(INode criteria, INode from, INode to, double val) : base(criteria, from, to, val)
        {

        }


        public INode Node => node ?? (Value >= 1 ? From : To);
        private INode node;
        public double Rating => rating ?? (Value >= 1 ? Value : (1 / Value));
        private double? rating;

        public void SetRating(INode from, double val)
        {
            node = from;
            rating = val;

            if (from == From)
                Value = val;
            else
                Mirrored.Value = val;
        }




        public static string GetTextRelationFor(double Value)
        {
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


        INodeRelation INodeRelation.Mirrored { get => base.Mirrored as INodeRelation; set => base.Mirrored = value as NodeRelation; }
    }

}
