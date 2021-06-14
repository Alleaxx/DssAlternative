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

        public bool Warning { get; }
        public bool Error { get; }
        public bool Hidden { get; }
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



        public bool Warning => GetWarning();
        public bool Error => GetError();
        public bool Hidden => GetHidden();


        public Func<bool> GetWarning { get; set; } = () => false;
        public Func<bool> GetError { get; set; } = () => false;
       
        public Func<bool> GetHidden { get; set; } = () => false;
    }
}
