using DSSAlternative.AHP.HierarchyInfo;
using DSSAlternative.AHP.Ratings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP.Relations
{
    /// <summary>
    /// Конкретное отношение между двумя узлами
    /// </summary>
    public interface IRelationNode
    {
        /// <summary>
        /// Возникает при изменении значения отношения
        /// </summary>
        event Action<IRelationNode> OnValueChanged;

        /// <summary>
        /// Отношения критерия, к которому принадлежит сравнение
        /// </summary>
        IRelationsCriteria CriteriaContext { get; }

        /// <summary>
        /// Главный узел (критерий), по которому идёт сравнение узлов
        /// </summary>
        INode Main { get; }
        /// <summary>
        /// Первичный узел который сравнивают со вторичным
        /// </summary>
        INode From { get; }
        /// <summary>
        /// Вторичный узел, который сравнивают с первичным
        /// </summary>
        INode To { get; }

        /// <summary>
        /// Признак отношения самого к себе (всегда равен 1, первичный узел равен вторичному)
        /// </summary>
        bool Self { get; }

        /// <summary>
        /// Зеркальное отношение, где значение инвертировано, а первичный узел является вторичным и наоборот
        /// </summary>
        IRelationNode Mirrored { get; set; }

        /// <summary>
        /// Отношение неизвестно, т.е. равно 0 в цифровом значении
        /// </summary>
        bool Unknown { get;  }

        /// <summary>
        /// Цифровое значение, показатель "приоритета" первичного узла над вторичным.
        /// При изменении меняет также значение и зеркального отношения
        /// </summary>
        double Value { get; set; }

        /// <summary>
        /// Округлённое значение для отображения в интерфейсе
        /// </summary>
        double ValueRounded { get; set; }
        /// <summary>
        /// Проставленный рейтинг, соответствует цифровому значению
        /// </summary>
        IRating Rating { get; }


        /// <summary>
        /// Отразить / перевернуть отношение
        /// </summary>
        void Reflect();
        /// <summary>
        /// Установить отношение в "неизвестный" статус (значение 0)
        /// </summary>
        void SetUnknown();


        /// <summary>
        /// Установить рейтинг отношению и поменять его значение согласно переданному рейтингу
        /// </summary>
        void SetRating(IRating rating);
    }


    /// <summary>
    /// Конкретное отношение между двумя узлами
    /// </summary>
    public class RelationNode : IRelationNode
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
        
        public event Action<IRelationNode> OnValueChanged;

        public IRelationsCriteria CriteriaContext { get; init; }
        public INode Main { get; init; }
        public INode From { get; init; }
        public INode To { get; init; }

        public bool Self => From == To;
        public bool Unknown => Value == 0;

        public IRelationNode Mirrored { get; set; }

        public double ValueRounded
        {
            get => Math.Round(Value, 4);
            set
            {
                Value = value;
            }
        }
        public double Value
        {
            get => Self ? 1 : value;
            set
            {
                SetValue();
                SetMirrored();
                OnValueChanged?.Invoke(this);

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
                    if (Mirrored != null && Mirrored is RelationNode rel)
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
        private double value;

        public RelationNode(IRelationsCriteria criteriaRelations, INode criteria, INode from, INode to, double val)
        {
            CriteriaContext = criteriaRelations;
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
                IRelationNode relation = rating.Node.Equals(From) ? this : Mirrored;
                relation.Value = rating.Value;
            }
        }
        public void SetUnknown()
        {
            Value = 0;
        }


        public void Reflect()
        {
            Value = 1 / Value;
        }
    }
}
