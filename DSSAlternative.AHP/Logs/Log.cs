using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP.Logs
{
    public class Log
    {
        public override string ToString()
        {
            return $"{State} | {CreationDate:HH:mm:ss} | {Category} - {Title}";
        }

        private static int TotalID;
        public int ID { get; init; }
        public object SourceObject { get; init; }
        public string Title { get; init; }
        public string Message { get; init; }
        public LogState State { get; init; }
        public LogCategory Category { get; init; }
        public DateTime CreationDate { get; init; }

        public Log(object source, string title, string message, LogState state, LogCategory category)
        {
            ID = TotalID++;
            SourceObject = source;
            Title = title;
            Message = message;
            State = state;
            Category = category;
            CreationDate = DateTime.Now;
        }
    }
}
