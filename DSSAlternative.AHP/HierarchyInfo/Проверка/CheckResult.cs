using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    /// <summary>
    /// Интерфейс результата отдельной проверки
    /// </summary>
    public interface ICheckResult
    {
        /// <summary>
        /// Краткое название
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Сообщение проверки
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Фактическое состояние
        /// </summary>
        CheckState State { get; }

        /// <summary>
        /// Признак успешной проверки
        /// </summary>
        bool IsOk { get; }
    }
    
    /// <summary>
    /// Результат отдельной проверки
    /// </summary>
    public class CheckResult : ICheckResult
    {
        public string Title { get; private set; }
        public string Message { get; private set; }

        public bool IsOk => State != CheckState.Error;
        public CheckState State { get; private set; }

        public CheckResult(string name, CheckState state, string message)
        {
            Title = name;
            Message = message;
            State = state;
        }
    }
}
