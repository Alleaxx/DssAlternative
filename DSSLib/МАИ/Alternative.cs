using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace DSSLib
{
    /// <summary>
    /// Альтернатива - возможный вариант решения проблемы со своими преимуществами и недостатками
    /// </summary>
    [Serializable]
    public class Alternative : IOutput
    {
        public override string ToString() => $"Альтернатива {Name}";

        //Общая информация
        public Alternative Parent { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }

        
        //Приоритет альтернативы по критериям
        public List<AlternativeCriteriaPriority> CriteriasPriorities { get; set; }
        public AlternativeCriteriaPriority GetCriteriaPriority(Criteria criteria) => CriteriasPriorities.Find(c => c.Criteria == criteria);



        public Alternative()
        {

        }
        public Alternative(string name, params (Criteria key,int importance)[] criterias) : this(null,name,criterias)
        {

        }
        public Alternative(Alternative owner, string name, params (Criteria key,int importance)[] criterias)
        {
            Parent = owner;
            Name = name;
            CriteriasPriorities = new List<AlternativeCriteriaPriority>();
            CriteriasPriorities = criterias.Select(c => new AlternativeCriteriaPriority(c.key, c.importance)).ToList();
        }


        public void Output()
        {
            Console.WriteLine($"Альтернатива {Name}");
            Console.WriteLine(Print.GetPrintText("ID",ID,true));
            string parent = Parent == null ? "-- корень --" : $"{Parent.Name}";
            Console.WriteLine(Print.GetPrintText("Родитель",$"{parent}",true));
            Console.WriteLine(Print.GetPrintText("Критерии", $"{CriteriasPriorities.Count} всего",false));
            foreach (AlternativeCriteriaPriority pair in CriteriasPriorities)
            {
                Console.WriteLine(Print.GetPrintText(" " + pair.Criteria,$"{pair.Priority}",true));
            }
            Console.WriteLine();
        }
        public void OutputFull()
        {
            Output();
        }
    }

    [Serializable]
    public class AlternativeCriteriaPriority
    {
        [XmlIgnore]
        //Критерий
        public Criteria Criteria
        {
            get => criteria;
            set
            {
                criteria = value;
                criteriaID = value.ID;
            }
        }
        private Criteria criteria;
        
        //Приоритет критерия
        [XmlAttribute]
        public int Priority
        {
            get => priority;
            set
            {
                priority = value;

            }
        }
        private int priority;

        //ID критерия
        [XmlAttribute]
        public string CriteriaID
        {
            get => criteriaID;
            set
            {
                criteriaID = value;
            }
        }
        private string criteriaID;



        public AlternativeCriteriaPriority()
        {

        }
        public AlternativeCriteriaPriority(Criteria criteria, int priority)
        {
            Criteria = criteria;
            Priority = priority;
        }
    }

}
