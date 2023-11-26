using DSSAlternative.AHP.Logs;

namespace DSSAlternative.Web.Services
{
    public class LoggerService : Logger 
    {
        public LoggerService()
        {
            Logger.Default.OnLogAdded += Default_OnLogAdded;
        }

        private void Default_OnLogAdded(Log obj)
        {
            this.AddLog(obj);
        }
    }
}
