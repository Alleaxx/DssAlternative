using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP.Logs
{
    public static class LogExtensions
    {
        public static void Add(this ILogger logger, object sourceObject, string title, LogState state, string message = null, LogCategory cate = LogCategory.Misc)
        {
            logger.AddLog(new Log(sourceObject, title, message, state, cate));
        }
        public static void AddInfo(this ILogger logger, object sourceObject, string title, string message = null, LogCategory cate = LogCategory.Misc)
        {
            logger.Add(sourceObject, title, LogState.Info, message, cate);
        }
        public static void AddWarning(this ILogger logger, object sourceObject, string title, string message = null, LogCategory cate = LogCategory.Misc)
        {
            logger.Add(sourceObject, title, LogState.Info, message, cate);
        }
        public static void AddError(this ILogger logger, object sourceObject, string title, string message = null, LogCategory cate = LogCategory.Misc)
        {
            logger.Add(sourceObject, title, LogState.Info, message, cate);
        }

        public static void AddUIInfo(this ILogger logger, object sourceObject, string title, string message = null)
        {
            logger.Add(sourceObject, title, LogState.Info, message, LogCategory.UI);
        }
    }
}
