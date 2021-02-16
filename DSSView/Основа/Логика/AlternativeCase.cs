using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class Alternative
    {
        public override string ToString() => Name;

        public string Name { get; set; }
        public Alternative() : this("") { }
        public Alternative(string name)
        {
            Name = name;
        }
    }
    public class Case
    {
        public event Action ChanceChanged;

        public override string ToString() => Name;
        public string Name { get; set; }

        public double Chance
        {
            get => chance;
            set
            {
                double old = chance;
                chance = value;
                ChanceChanged?.Invoke();
            }
        }
        private double chance;

        public Case() : this("") { }
        public Case(string name, double chance = 0)
        {
            Name = name;
            Chance = chance;
        }
    }
}
