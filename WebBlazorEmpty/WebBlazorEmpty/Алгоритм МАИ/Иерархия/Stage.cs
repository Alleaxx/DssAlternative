using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBlazorEmpty.AHP
{
    public interface IStage
    {
        string Name { get; }
        string Way { get; }
        string Description { get; }
    }

    public class BigStage
    {

        public string Name { get; set; }
        public int ProgressNow { get; set; }
        public int ProgressMax { get; set; }

        public bool Warning { get; set; }
        public bool Error { get; set; }

        public BigStage(string name,int now, int max, bool warn = false, bool error = false)
        {
            Name = name;
            ProgressNow = now;
            ProgressMax = max;
            Warning = warn;
            Error = error;
        }
    }

    public class Stage : IStage
    {
        public string Name { get; set; }
        public string Way { get; set; }
        public string Description { get; set; }

        public List<IStage> Stages { get; set; } = new List<IStage>();

        public Stage(string name) : this(name, "/", "") { }
        public Stage(string name, string way) : this(name, way, "") { }
        public Stage(string name, string way, string descr)
        {
            Name = name;
            Way = way;
    
            Description = descr;
        }
    }
}
