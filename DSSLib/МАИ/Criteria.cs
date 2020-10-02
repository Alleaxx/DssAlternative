using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DSSLib
{
    [Serializable]
    public class Criteria : IOutput
    {
        public override string ToString() => $"{Name}";
        public event Action ImportanceUpdated;

        private static int id { get; set; }
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
                ImportanceUpdated?.Invoke();
            }
        }
        private int importance;

        public List<CriteriaValue> CriteriaValues { get; set; }
        private CriteriaValue value;

        public Criteria()
        {
            id++;
        }
        public Criteria(string name, int importance) : this($"Criteria-{id}", name, importance) { }
        public Criteria(string idStr,string name, int importance) : this()
        {
            ID = idStr;
            Name = name;
            Importance = importance;
        }


        public void Output()
        {
            Console.WriteLine($"Критерий {Name}");
            Console.WriteLine(Print.GetPrintText("ID",$"{ID}",true));
            Console.WriteLine(Print.GetPrintText("Приоритет",$"{Importance}",true));
            Console.WriteLine();
        }
    }

    public class CriteriaValue
    {
        public string Descriprion { get; set; }
        public int Priority { get; set; }
    }
}
