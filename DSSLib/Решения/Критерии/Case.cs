using System;
using System.Collections.Generic;
using System.Text;

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
                if (value == -1)
                    IsEqual = true;
            }
        }
        private double chance;

        //Равновероятный шанс среди всез
        [System.Xml.Serialization.XmlIgnore]
        public bool IsEqual { get; set; }
        
        public Case()
        {

        }
        public Case(string name) : this(name,name,-1)
        {

        }
        public Case(string idStr,string name) : this(idStr,name,-1)
        {

        }
        public Case(string idStr,string name, double chance)
        {
            ID = idStr;
            Name = name;
            Chance = chance;
        }

        public void Output()
        {
            Console.WriteLine(Name);
            Console.WriteLine(Print.GetPrintText("ID",$"{ID}",true));
            Console.WriteLine(Print.GetPrintText("Шанс",$"{Chance}",true));
            Console.WriteLine();
        }
    }
}
