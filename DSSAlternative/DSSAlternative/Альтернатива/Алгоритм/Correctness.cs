using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface ICorrectness
    {
        bool IsCorrect { get; }
        IEnumerable<ICheck> ChecksList { get; }
        IEnumerable<ICheck> ErrorsList { get; }
    }
    public class Correctness : ICorrectness
    {
        public bool IsCorrect => !ErrorsList.Any();
        public IEnumerable<ICheck> ChecksList
        {
            get
            {
                ExecuteCheck();
                return Checks;
            }
        }
        public IEnumerable<ICheck> ErrorsList => ChecksList.Where(c => !c.IsOk);


        private readonly List<ICheck> Checks;
        public Correctness()
        {
            Checks = new List<ICheck>();
        }
        private void ExecuteCheck()
        {
            Checks.Clear();
            OwnCheck();
        }
        protected virtual void OwnCheck()
        {

        }

        protected void AddSuccess(string name, string msg)
        {
            Checks.Add(new CheckResult(name, ErrorState.Ok, msg));
        }
        protected void AddWarning(string name, string msg)
        {
            Checks.Add(new CheckResult(name, ErrorState.Warning, msg));
        }
        protected void AddFail(string name, string msg)
        {
            Checks.Add(new CheckResult(name, ErrorState.Error, msg));
        }
    }
}
