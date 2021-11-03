using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSSAlternative.AHP;

namespace DSSAlternative.AppComponents
{
    public class User
    {
        public string Name { get; set; }
        public List<Template> Templates { get; set; }
        public DssState State { get; set; }

        public User()
        {
            Name = "Гость";
            Templates = new List<Template>();
        }
    }
}
