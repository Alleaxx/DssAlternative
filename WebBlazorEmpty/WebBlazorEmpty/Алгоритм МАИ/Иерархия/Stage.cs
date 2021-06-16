using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBlazorEmpty.AHP
{
    public interface IStage
    {
        string Name { get; }

        public bool Warning { get; }
        public bool Error { get; }
        public bool Hidden { get; }
    }

    public class Stage : IStage
    {
        public string Name { get; private set; }
        public Stage(string name)
        {
            Name = name;
        }


        public bool Warning => GetWarning();
        public bool Error => GetError();
        public bool Hidden => GetHidden();


        public Func<bool> GetWarning { get; set; } = () => false;
        public Func<bool> GetError { get; set; } = () => false;
       
        public Func<bool> GetHidden { get; set; } = () => false;
    }
}
