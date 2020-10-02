using System;
using System.Collections.Generic;
using System.Linq;
using DSSLib;

namespace DSSConsole
{
    class Program
    {
        static Choice Choise { get; set; }
        static void Main(string[] args)
        {
            Criteria place = new Criteria("Место",1);
            Criteria reputation = new Criteria("Репутация",5);
            Alternative GUU = new Alternative("ГУУ",
                new KeyValuePair<Criteria, int>(place,10), new KeyValuePair<Criteria, int>(reputation,30));
            Alternative MIREA = new Alternative("МИРЭА",
                new KeyValuePair<Criteria, int>(place,20), new KeyValuePair<Criteria, int>(reputation,15));
            Alternative MGU = new Alternative("МГУ",
                new KeyValuePair<Criteria, int>(place,50), new KeyValuePair<Criteria, int>(reputation,10));
            Choise = new Choice("Выбор университета", new Criteria[] { place, reputation }, new Alternative[] { GUU, MIREA, MGU });
            Choise.Criterias.ToList().ForEach(c => c.Output());
            Choise.Alternatives.ToList().ForEach(a => a.Output());
            Choise.CriteriaMatrix.Output();
            Choise.AlternativeComparisons.ToList().ForEach(c => c.Value.Output());
            Choise.Output();
            Console.ReadKey();
        }
    }
}
