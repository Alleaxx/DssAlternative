using System;
using System.Collections.Generic;
using System.Text;

namespace DSSLib
{
    public class Choice : IOutput
    {
        public Problem Problem { get; protected set; }

        public Choice(Problem problem)
        {
            Problem = problem;
        }

        protected virtual void CountDesizion()
        {

        }

        public virtual void Output()
        {

        }
    }
}
