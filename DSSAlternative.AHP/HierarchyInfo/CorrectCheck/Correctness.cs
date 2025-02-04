using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    /// <summary>
    /// Интерфейс результат проверки какого-либо объекта. Он может быть корректен, либо ошибочен
    /// </summary>
    public interface ICorrectness
    {
        /// <summary>
        /// Признак отсутствия проверок с ошибкой
        /// </summary>
        bool IsCorrect { get; }

        /// <summary>
        /// Список проверок с указанными состояниями. Проверки выполняются при обращении к свойству IsCorrect
        /// </summary>
        IEnumerable<ICheckResult> GetChecksByState(params CheckState[] states);
    }
    
    /// <summary>
    /// Результат проверки какого-либо объекта. Он может быть корректен, либо ошибочен
    /// </summary>
    public class Correctness : ICorrectness
    {
        public bool IsCorrect => !GetChecksByState(CheckState.Error).Any();

        public Correctness()
        {
            
        }

        /// <summary>
        /// Создать и выполнить проверки, вернуть их список
        /// </summary>
        protected virtual IEnumerable<ICheckResult> CreateChecks()
        {
            return Array.Empty<ICheckResult>();
        }

        public IEnumerable<ICheckResult> GetChecksByState(params CheckState[] states)
        {
            return CreateChecks().Where((c) => states.Contains(c.State)).ToArray();
        }



        /// <summary>
        /// Добавить успешную проверку в список
        /// </summary>
        protected static void AddSuccess(List<ICheckResult> checks, string name, string msg)
        {
            checks.Add(new CheckResult(name, CheckState.Ok, msg));
        }
        /// <summary>
        /// Добавить успешную проверку с ПРЕДУПРЕЖДЕНИЕМ в список
        /// </summary>
        protected static void AddWarning(List<ICheckResult> checks, string name, string msg)
        {
            checks.Add(new CheckResult(name, CheckState.Warning, msg));
        }
        /// <summary>
        /// Добавить ошибочную проверку в список
        /// </summary>
        protected static void AddFail(List<ICheckResult> checks, string name, string msg)
        {
            checks.Add(new CheckResult(name, CheckState.Error, msg));
        }
    }
}
