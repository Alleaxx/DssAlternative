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

        //Подчиненные альтернативы
        public List<Alternative> Alternatives { get; set; }


        //Приоритет альтернативы по критериям
        public List<AlternativeCriteriaPriority> CriteriasPriorities { get; set; } = new List<AlternativeCriteriaPriority>();
        public AlternativeCriteriaPriority GetCriteriaPriority(Criteria criteria) => CriteriasPriorities.Find(c => c.Key == criteria);


        //Выгода альтернативы по случаям
        public List<AlternativeCaseProfit> CaseProfits { get; set; } = new List<AlternativeCaseProfit>();
        public AlternativeCaseProfit GetCaseProfit(Case caseSel) => CaseProfits.Find(c => c.Key == caseSel);



        public Alternative()
        {

        }
        public Alternative(string id,string name) : this(null,id,name)
        {

        }
        public Alternative(Alternative owner, string id, string name)
        {
            ID = id;
            Parent = owner;
            Name = name;
        }
        public Alternative SetCaseProfits(params (Case key,int profit)[] cases)
        {
            CaseProfits = cases.Select(c => new AlternativeCaseProfit(c.key,c.profit)).ToList();
            CaseProfits.ForEach(c => c.Alternative = this);
            return this;
        }
        public Alternative SetCriteriasPrior(params (Criteria key,int importance)[] criterias)
        {
            CriteriasPriorities = criterias.Select(c => new AlternativeCriteriaPriority(c.key, c.importance)).ToList();
            CriteriasPriorities.ForEach(c => c.Alternative = this);
            return this;
        }


        public void Output()
        {
            Console.WriteLine($"Альтернатива {Name}");
            Console.WriteLine(Print.GetPrintText("ID",ID,true));
            string parent = Parent == null ? "-- корень --" : $"{Parent.Name}";
            Console.WriteLine(Print.GetPrintText("Родитель",$"{parent}",true));
            Console.WriteLine(Print.GetPrintText("Критерии", $"{CriteriasPriorities.Count} всего",false));
            foreach (var pair in CriteriasPriorities)
            {
                Console.WriteLine(Print.GetPrintText(" " + pair.Key,$"{pair.Priority}",true));
            }
            Console.WriteLine(Print.GetPrintText("Исходы случаев", $"{CaseProfits.Count} всего",false));
            foreach (var pair in CaseProfits)
            {
                Console.WriteLine(Print.GetPrintText(" " + pair.Key.Name,$"{pair.Profit}",true));
            }
            Console.WriteLine();
        }
        public void OutputFull()
        {
            Output();
        }
    }



    [Serializable]
    public class AlternativeRelation<T> where T:IId
    {
        [XmlIgnore]
        public Alternative Alternative { get; set; }

        [XmlIgnore]
        //Критерий
        public T Key
        {
            get => key;
            set
            {
                key = value;
                keyID = value.ID;
            }
        }
        private T key;


        //ID критерия
        [XmlIgnore]
        public string KeyID
        {
            get => keyID;
            set
            {
                keyID = value;
            }
        }
        private string keyID;



        public AlternativeRelation()
        {

        }
        public AlternativeRelation(T key)
        {
            Key = key;
        }
        public AlternativeRelation(Alternative a, T key)
        {
            Alternative = a;
            Key = key;
        }

    }
    /// <summary>
    /// Приоритет альтернативы по критерию – численный показатель преимущества альтернативы по указанному критерию
    /// </summary>
    public class AlternativeCriteriaPriority : AlternativeRelation<Criteria>
    {        
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
            get => KeyID;
            set
            {
                KeyID = value;
            }
        }



        public AlternativeCriteriaPriority()
        {

        }
        public AlternativeCriteriaPriority(Criteria criteria, int priority) : base(criteria)
        {
            Priority = priority;
        }
    }
    public class AlternativeCaseProfit : AlternativeRelation<Case>
    {
        //ID критерия
        [XmlAttribute]
        public string CaseID
        {
            get => KeyID;
            set
            {
                KeyID = value;
            }
        }
        [XmlAttribute]
        public int Profit { get; set; }

        public AlternativeCaseProfit()
        {

        }
        public AlternativeCaseProfit(Case caseC, int profit) : base(caseC)
        {
            Profit = profit;
        }
    }


}
