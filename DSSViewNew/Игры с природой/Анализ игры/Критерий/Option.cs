using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    //Настройка для критериев
    public interface IOption
    {
        event Action<double, double> OnValueChanged;
        double Value { get; }
    }
    public class Option : IOption
    {
        public override string ToString() => $"{Name} ({Value})";
        public event Action<double, double> OnValueChanged;

        public string Name { get; private set; }
        public double Value
        {
            get => value;
            set
            {
                double old = value;
                if (value < Min)
                {
                    this.value = Min;
                }
                else if (value > Max)
                {
                    this.value = Max;
                }
                else
                {
                    this.value = value;
                }

                OnValueChanged?.Invoke(old, this.value);
            }
        }
        private double value;


        public double Min { get; private set; }
        public double Max { get; private set; }

        public Option(string name, double val, double min = 0, double max = 1)
        {
            Name = name;
            Min = min;
            Max = max;
            Value = val;
        }
    }

}
