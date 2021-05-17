using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSLib.New
{
    interface IAlternative
    {
        string Name { get; set; }
        string Description { get; set; }
    }

    public class Alternative : IAlternative
    {
        public override string ToString() => Name;

        
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }


        public Alternative() : this("") { }
        public Alternative(string name)
        {
            Name = name;
        }
    }
}
