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
    public class ChoiceAHP : Choice
    {
        public override string ToString() => $"Решение МАИ: {Problem.Name}";

        //Проверить на наличие у альтернатив всех указанных критериев в выборе
        public static CheckChoiceResult CheckAll(Problem problem)
        {
            CheckChoiceResult check = new CheckChoiceResult();
            if(problem.Alternatives.Count == 0)
            {
                check.Success = false;
                check.Messages.Add("В проблеме не задано альтернатив, из которых можно выбирать");
                return check;
            }
            if(problem.Criterias.Count == 0)
            {
                check.Success = false;
                check.Messages.Add("В проблеме не заданы критерии, по которым можно выбирать альтернативы");
                return check;
            }

            foreach (Alternative alternative in problem.Alternatives)
            {
                foreach (Criteria criteria in problem.Criterias)
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
        public static bool IsSolvable(Problem problem) => CheckAll(problem).Success;



        //Матрица критериев
        public CriteriaMatrix CriteriaMatrix { get; private set; }
        //Итоговые веса критериев
        public Dictionary<Criteria,double> CriteriaWeights { get; private set; }



        //Матрица альтернатив по критериям
        public Dictionary<Criteria, AlternativesMatrix> AlternativeComparisons { get; set; }
        //Итоговые веса альтернатив
        public Dictionary<Alternative,double> AlternativeWeights { get; set; }


        //Конструктор
        public ChoiceAHP(Problem problem) : base(problem)
        {
            CountDesizion();
        }
        //Рассчет решения
        protected override void CountDesizion()
        {
            CriteriaMatrix = new CriteriaMatrix(Problem.Criterias);
            CriteriaWeights = CriteriaMatrix.Weights;
            AlternativeComparisons = Problem.Criterias.Select(c => new AlternativesMatrix(c, Problem.Alternatives.ToArray())).ToDictionary(c => c.Criteria);

            AlternativeWeights = new Dictionary<Alternative, double>();
            foreach (Alternative alternative in Problem.Alternatives)
            {
                double weight = 0;
                foreach (Criteria criteria in Problem.Criterias)
                {
                    weight += CriteriaWeights[criteria] * AlternativeComparisons[criteria].Weights[alternative];
                }
                AlternativeWeights.Add(alternative, weight);
            }
        }


        //Вывод
        public override void Output()
        {
            Console.WriteLine($"ЗАДАЧА: {Problem.Name}");
            Console.WriteLine(Print.GetPrintText("Веса критериев",$"{Problem.Criterias.Count}",false));
            foreach (var item in Problem.Criterias)
            {
                Console.WriteLine(Print.GetPrintText($"{item.Name}",$"база: {item.Importance}, рассчет: {Math.Round(CriteriaWeights[item],4)}",true));
            }
            Console.WriteLine(Print.GetPrintText("Матрицы по критериям",$"{AlternativeComparisons.Count}",false));
            foreach (var item in AlternativeComparisons)
            {
                item.Value.Output();
            }
            Console.WriteLine(Print.GetPrintText("Веса альтернатив",$"{Problem.Alternatives.Count}",false));
            foreach (var item in Problem.Alternatives)
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
