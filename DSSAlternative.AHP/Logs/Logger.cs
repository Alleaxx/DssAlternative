using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP.Logs
{
    public interface ILogger
    {
        event Action<Log> OnLogAdded;
        IEnumerable<Log> Logs { get; }
        void AddLog(Log log);
    }
    public class Logger : ILogger
    {
        public static ILogger Default
        {
            get
            {
                if (defaultLogger == null)
                {
                    defaultLogger = new Logger();
                }
                return defaultLogger;
            }
        }
        private static ILogger defaultLogger;

        public event Action<Log> OnLogAdded;

        public IEnumerable<Log> Logs => LogsList;
        private List<Log> LogsList { get; init; }
        public Logger()
        {
            LogsList = new List<Log>();
        }
        public void AddLog(Log log)
        {
            LogsList.Add(log);
            OnLogAdded?.Invoke(log);
        }
    }
}
