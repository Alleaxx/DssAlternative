using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class Case
    {
        public event Action OnChanceChanged;

        public override string ToString() => Name;
        public string Name { get; set; }

        public double Chance
        {
            get => chance;
            set
            {
                double old = chance;
                chance = value;
                OnChanceChanged?.Invoke();
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
