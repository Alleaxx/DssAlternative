using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface ICheck
    {
        string Name { get; }
        bool IsOk { get; }
        string Message { get; }
        ErrorState State { get; }
    }
    public class CheckResult : ICheck
    {
        public string Name { get; private set; }
        public string Message { get; private set; }

        public bool IsOk => State != ErrorState.Error;
        public ErrorState State { get; private set; }

        public CheckResult(string name, ErrorState state, string message)
        {
            Name = name;
            Message = message;
            State = state;
        }
    }


}
