using System;
using System.Collections.Generic;
using System.Text;

using System.Xml.Serialization;

namespace DSSLib
{
    [Serializable]
    public class Case : IOutput, IId
    {
        //Общая информация
        public string ID { get; set; }
        public string Name { get; set; }
        //Шанс
        public double Chance
        {
            get => chance;
            set
            {
                chance = value;
                if (value == 0)
                    IsEqual = true;
            }
        }
        private double chance;

        //Выгода
        public double Benefit { get; set; }

        //Равновероятный шанс среди всез
        [XmlIgnore]
        public bool IsEqual { get; set; }


        public List<Alternative> AvailAlts { get; set; } = new List<Alternative>();

        public Case()
        {

        }
        public Case(string idStr,string name, double chance) : this(idStr,name,chance,0)
        {

        }
        public Case(string idStr,string name, double chance, double profit)
        {
            ID = idStr;
            Name = name;
            Chance = chance;
            Benefit = profit;
        }


        public void Output()
        {
            Console.WriteLine($"Исход [{ID,-10} {Name,-20} {Chance}]");
        }
    }
}
