using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class Alternative : NotifyObj
    {
        public override string ToString() => Name;

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }
        private string name;
        public string Description { get; set; }
        public string Image { get; set; }


        public Alternative() : this("") { }
        public Alternative(string name)
        {
            Name = name;
        }
    }
}
