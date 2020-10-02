using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DSSLib
{
    /// <summary>
    /// Задача выбора, решаемая методом анализа иерархий
    /// </summary>
    /// 
    [Serializable]
    public class ChoiceAHP : IOutput
    {
        public override string ToString() => string.IsNullOrEmpty(Name) ? $"Выбор по {Criterias.Length} критериям из {Alternatives.Length} альтернатив" : Name;

        //Название решения
        public string Name { get; set; }


        //Критерии и альтернативы
        public Criteria[] Criterias { get; set; }
        public Alternative[] Alternatives { get; set; }

        //Загрузить решение
        public void AfterLoad()
        {
            foreach (Alternative alternative in Alternatives)
            {
                foreach (AlternativeCriteriaPriority priority in alternative.CriteriasPriorities)
                {
                    Criteria criteria = Criterias.ToList().Find(c => c.ID == priority.CriteriaID);
                    if (criteria != null)
                        priority.Criteria = criteria; 
                }
            }
        }


        //Проверить на наличие у альтернатив всех указанных критериев в выборе
        public CheckChoiceResult CheckAll()
        {
            CheckChoiceResult check = new CheckChoiceResult();
            foreach (Alternative alternative in Alternatives)
            {
                foreach (Criteria criteria in Criterias)
                {
                    if(alternative.GetCriteriaPriority(criteria) == null)
                    {
                        check.Success = false;
                        check.Messages.Add($"- В альтернативе {alternative.Name} отсутствует приоритет необходимого критерия {criteria.Name}");
                    }
                }
            }
            return check;
        }
        [XmlIgnore]
        private bool IsCalculationAvailable => CheckAll().Success;

        //Обновить решение
        public void Calculate()
        {
            if (IsCalculationAvailable)
            {
                CriteriaMatrix = new CriteriaMatrix(Criterias);
                CriteriaWeights = CriteriaMatrix.Weights;
                AlternativeComparisons = Criterias.Select(c => new AlternativesMatrix(c, Alternatives)).ToDictionary(c => c.Criteria);

                AlternativeWeights = new Dictionary<Alternative, double>();
                foreach (Alternative alternative in Alternatives)
                {
                    double weight = 0;
                    foreach (Criteria criteria in Criterias)
                    {
                        weight += CriteriaWeights[criteria] * AlternativeComparisons[criteria].Weights[alternative];
                    }
                    AlternativeWeights.Add(alternative, weight);
                }
            }
        }

        //Матрица критериев
        [XmlIgnore]
        public CriteriaMatrix CriteriaMatrix { get; private set; }
        [XmlIgnore]
        public Dictionary<Criteria,double> CriteriaWeights { get; private set; }



        //Матрица альтернатив по критериям
        [XmlIgnore]
        public Dictionary<Criteria, AlternativesMatrix> AlternativeComparisons { get; set; }
        [XmlIgnore]
        public Dictionary<Alternative,double> AlternativeWeights { get; set; }


        //Конструктор
        public ChoiceAHP()
        {

        }
        public ChoiceAHP(string name, Criteria[] criterias, Alternative[] alternatives)
        {
            Name = name;
            Criterias = criterias.Where(c => c.Importance > 0).ToArray();
            Alternatives = alternatives;
            Calculate();
            foreach (Criteria criteria in Criterias)
            {
                criteria.ImportanceUpdated += Calculate;
            }
        }


        //Вывод
        public void Output()
        {
            Console.WriteLine($"ЗАДАЧА: {Name}");
            Console.WriteLine(Print.GetPrintText("Критериев",$"{Criterias.Length}",false));
            foreach (var item in Criterias)
            {
                Console.WriteLine(Print.GetPrintText($"{item.Name}",$"база: {item.Importance}, рассчет: {Math.Round(CriteriaWeights[item],4)}",true));
            }
            Console.WriteLine(Print.GetPrintText("Альтернативы",$"{Alternatives.Length}",false));
            foreach (var item in Alternatives)
            {
                Console.WriteLine(Print.GetPrintText($"{item.Name}",$"{Math.Round(AlternativeWeights[item],4)}",true));
            }


            Console.WriteLine();

        }
    }


    public class CheckChoiceResult
    {
        public bool Success { get; set; }
        public string Result => Success ? "Успех, ошибок не обнаружено" : $"Внимание, обнаружено {Messages.Count} ошибок";
        public List<string> Messages { get; set; }

        public CheckChoiceResult()
        {
            Success = true;
            Messages = new List<string>();
        }
    }
}
