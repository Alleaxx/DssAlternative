using System;
using System.Collections.Generic;
using System.Text;

namespace DSSLib
{

    public class Case : NotifyObj
    {
        public event Action OnChanceChanged;

        public override string ToString() => $"Случай \"{Name}\"";
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

        public double Chance
        {
            get => chance;
            set
            {
                double old = chance;
                chance = value;
                OnChanceChanged?.Invoke();
                OnPropertyChanged();
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
