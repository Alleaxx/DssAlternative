using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class Note
    {
        public override string ToString()
        {
            return $"{Profit} - {Name}";
        }

        public string Name { get; private set; }
        public double Profit { get; private set; }
        public bool IsGood => Profit > 0;

        public Note(string name, double profit)
        {
            Name = name;
            Profit = profit;
        }
    }
}
