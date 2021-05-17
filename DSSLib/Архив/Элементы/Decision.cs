using System;
using System.Collections.Generic;
using System.Text;

namespace DSSLib
{
    public class Decision : IOutput
    {
        public Problem Problem { get; protected set; }

        public Decision(Problem problem)
        {
            Problem = problem;
        }

        protected virtual void Solve() { }
        public virtual void Output() { }

        

        protected static DecisionCheckResult CheckBasic(Problem problem)
        {
            DecisionCheckResult check = new DecisionCheckResult();
            if(problem.Alternatives.Count == 0)
            {
                check.Success = false;
                check.Messages.Add("- В проблеме не задано альтернатив, из которых можно выбирать");
                return check;
            }
            return check;
        }
    }


    public class DecisionCheckResult
    {
        public bool Success { get; set; }
        public string Result => Success ? "Успех, ошибок не обнаружено" : $"Внимание, обнаружено {Messages.Count} ошибок";
        public List<string> Messages { get; set; }

        public DecisionCheckResult()
        {
            Success = true;
            Messages = new List<string>();
        }
    }
}
