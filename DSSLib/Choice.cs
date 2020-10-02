using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSSLib
{
    public class Choice : IOutput
    {
        public override string ToString() => string.IsNullOrEmpty(Name) ? $"Выбор по {Criterias.Length} критериям из {Alternatives.Length} альтернатив" : Name;

        public string Name { get; private set; }

        public Criteria[] Criterias { get; private set; }
        public Alternative[] Alternatives { get; private set; }

        private void UpdateCriteriaMatrix()
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
        public CriteriaMatrix CriteriaMatrix { get; private set; }
        public Dictionary<Criteria,double> CriteriaWeights { get; private set; }

        public Dictionary<Criteria, AlternativesMatrix> AlternativeComparisons { get; set; }
        public Dictionary<Alternative,double> AlternativeWeights { get; set; }

        public Choice(string name, Criteria[] criterias, Alternative[] alternatives)
        {
            Name = name;
            Criterias = criterias.Where(c => c.Importance > 0).ToArray();
            Alternatives = alternatives;
            UpdateCriteriaMatrix();
            foreach (Criteria criteria in Criterias)
            {
                criteria.ImportanceUpdated += UpdateCriteriaMatrix;
            }
        }


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



    public abstract class Matrix<K> : IOutput
    {
        public double[,] Arr { get; protected set; }
        public K[] Keys { get; protected set; }
        
        public Dictionary<K,double> Weights
        {
            get
            {
                Dictionary<K, double> weights = new Dictionary<K, double>();
                double sumTotal = 0;
                foreach (double val in Arr)
                {
                    sumTotal += val;
                }
                int rows = Arr.GetUpperBound(0) + 1;
                int columns = Arr.Length / rows;
                for (int x = 0; x < rows; x++)
                {
                    K key = Keys[x];
                    double sumRow = 0;
                    for (int y = 0; y < columns; y++)
                    {
                        sumRow += Arr[x, y];
                    }
                    double weight = sumRow / sumTotal;
                    weights.Add(key, weight);
                }

                return weights;
            }
        }
        public double Get(K a, K b)
        {
            int aPos = Keys.ToList().IndexOf(a);
            int bPos = Keys.ToList().IndexOf(b);

            if (aPos != -1 && bPos != -1)
                return Arr[aPos, bPos];
            else
                throw new Exception("Ключи в матрице не найдены");
        }

        public Matrix(K[] keys)
        {
            Arr = new double[keys.Length,keys.Length];
            Keys = keys;
        }


        public virtual void Output()
        {

        }
    }
    public class CriteriaMatrix : Matrix<Criteria>
    {
        public override string ToString() => $"Матрица {Keys.Length} критериев";

        public CriteriaMatrix(IEnumerable<Criteria> criterias) : base(criterias.ToArray())
        {
            int crCount = criterias.Count();
            for (int x = 0; x < crCount; x++)
            {
                Criteria a = criterias.ElementAt(x);
                for (int y = 0; y < crCount; y++)
                {
                    Criteria b = criterias.ElementAt(y);
                    double val = (double)a.Importance / b.Importance;
                    Arr[x, y] = val;
                }
            }
        }

        public override void Output()
        {
            Console.WriteLine($"{ToString()}");
            Console.WriteLine(Print.GetPrintText("Критерии",$"{Keys.Length}",false));
            foreach (var item in Weights)
            {
                Console.WriteLine(Print.GetPrintText($"{item.Key.Name}",$"{Math.Round(item.Value,4)}",true));
            }

            Console.WriteLine();
        }
    }
    public class AlternativesMatrix : Matrix<Alternative>
    {
        public override string ToString() => $"Матрица сравнения {Keys.Length} альтернатив по критерию {Criteria.Name}";
        public Criteria Criteria { get; set; }

        public AlternativesMatrix(Criteria cr, Alternative[] alternatives) : base(alternatives)
        {
            Criteria = cr;
            int altCount = alternatives.Count();
            for (int x = 0; x < altCount; x++)
            {
                Alternative a = alternatives.ElementAt(x);
                for (int y = 0; y < altCount; y++)
                {
                    Alternative b = alternatives.ElementAt(y);
                    double val = (double)a.Criterias[Criteria] / b.Criterias[Criteria];
                    Arr[x, y] = val;
                }
            }
        }

        public override void Output()
        {
            Console.WriteLine($"{ToString()}");
            Console.WriteLine(Print.GetPrintText("Критерий",$"{Criteria.Name} - {Criteria.Importance}",true));
            Console.WriteLine(Print.GetPrintText("Альтернативы",$"{Keys.Length}",false));
            foreach (var item in Weights)
            {
                Console.WriteLine(Print.GetPrintText($"{item.Key.Name}",$"{Math.Round(item.Value,4)}",true));
            }

            Console.WriteLine();
        }
    }
}
