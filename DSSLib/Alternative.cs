using System;
using System.Collections;
using System.Collections.Generic;

namespace DSSLib
{
    [Serializable]
    public class Alternative : IOutput
    {
        public override string ToString() => $"Альтернатива {Name}";

        public string ID { get; set; }
        public string Name { get; set; }
        public Dictionary<Criteria, int> Criterias { get; set; }

        public Alternative(string name, params KeyValuePair<Criteria, int>[] criterias)
        {
            Name = name;
            Criterias = new Dictionary<Criteria, int>();
            foreach (var criteria in criterias)
            {
                if (!Criterias.ContainsKey(criteria.Key))
                    Criterias.Add(criteria.Key,criteria.Value);
            }
        }


        public void Output()
        {
            Console.WriteLine($"Альтернатива {Name}");
            Console.WriteLine(Print.GetPrintText("ID",ID,true));
            Console.WriteLine(Print.GetPrintText("Критерии",Criterias.Count.ToString(),false));
            foreach (var pair in Criterias)
            {
                Console.WriteLine(Print.GetPrintText(" " + pair.Key.Name,pair.Value.ToString(),true));
            }
            Console.WriteLine();
        }


    }

}
