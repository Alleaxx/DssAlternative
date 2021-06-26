using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface ICheck : IStyled
    {
        string Name { get; }
        bool Passed { get; }
        string Message { get; }
    }
    public class CheckResult : ICheck
    {
        public string Name { get; private set; }
        public bool Passed { get; private set; }
        public string Message { get; private set; }
        private string Class { get; set; }


        public CheckResult(string name, string cl, bool passed, string message)
        {
            Name = name;
            Passed = passed;
            Message = message;
            Class = cl;
        }

        public string GetClass() => Passed ? $"passed {Class}" : $"errored {Class}";
        public string GetStyle() => "";
    }

}
