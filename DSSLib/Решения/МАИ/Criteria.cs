using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DSSLib
{
    [Serializable]
    public class Criteria : IOutput, IId
    {
        public override string ToString() => $"{Name}";

        [XmlAttribute]
        public string ID { get; set; }
        public string Name { get; set; }

        public int Importance
        {
            get => importance;
            set
            {
                if (value < 0)
                    value = 0;
                importance = value;
            }
        }
        private int importance;

        public Criteria()
        {

        }
        public Criteria(string idStr,string name, int importance)
        {
            ID = idStr;
            Name = name;
            Importance = importance;
        }


        public void Output()
        {
            Console.WriteLine($"Критерий [{ID,-15} {Name,-20} {Importance}]");
        }
    }

    //public class CriteriaValue
    //{
    //    public string Descriprion { get; set; }
    //    public int Priority { get; set; }
    //}
}
