using DSSAlternative.AHP.Logs;

namespace DSSAlternative.Web.Services
{
    /// <summary>
    /// Дефолтный объект логгера
    /// </summary>
    public class LoggerService : Logger 
    {
        public LoggerService()
        {
            Default.OnLogAdded += Default_OnLogAdded;
        }

        private void Default_OnLogAdded(Log obj)
        {
            this.AddLog(obj);
        }
    }
}
