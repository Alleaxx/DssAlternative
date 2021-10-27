using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSCriterias.Logic
{
    //Шаг алгоритма рассчета критерия
    public interface IStep
    {
        int Order { get; }
        string Name { get; }
    }
    public class Step : IStep
    {
        public override string ToString()
        {
            return $"Шаг {Order}) {Name} - {Result}";
        }

        public int Order { get; private set; }
        public string Name { get; private set; }
        public double Result { get; private set; }

        public Step(int count, string name, double result)
        {
            Order = count;
            Name = name;
            Result = result;
        }
    }
    public class StepArr : IStep
    {
        public override string ToString()
        {
            return $"Шаг {Order}) {Name} - [{string.Join(";", Arr)}]";
        }

        public int Order { get; private set; }
        public string Name { get; private set; }
        public IEnumerable<double> Arr { get; private set; }

        public StepArr(int count, string name, IEnumerable<double> arr)
        {
            Order = count;
            Name = name;
            Arr = arr;
        }
    }
}
