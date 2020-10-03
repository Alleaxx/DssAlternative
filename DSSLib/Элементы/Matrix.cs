using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSSLib
{
    /// <summary>
    /// Матрица
    /// TODO
    /// - Представление нормализованной матрицы
    /// - Проверка на согласованность
    /// </summary>
    /// <typeparam name="K"></typeparam>
    public abstract class Matrix<K,V> : IOutput
    {
        public double[,] Arr { get; protected set; }
        protected int Rows => Arr.GetUpperBound(0) + 1;
        protected int Columns => Arr.Length / Rows;

        public K[] Keys { get; protected set; }
        public V[] Values { get; protected set; }
        
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
                for (int x = 0; x < Rows; x++)
                {
                    K key = Keys[x];
                    double sumRow = 0;
                    for (int y = 0; y < Columns; y++)
                    {
                        sumRow += Arr[x, y];
                    }
                    double weight = sumRow / sumTotal;
                    weights.Add(key, weight);
                }

                return weights;
            }
        }
        public double Get(K a, V b)
        {
            int aPos = Keys.ToList().IndexOf(a);
            int bPos = Values.ToList().IndexOf(b);

            if (aPos != -1 && bPos != -1)
                return Arr[aPos, bPos];
            else
                throw new Exception("Ключи в матрице не найдены");
        }

        public Matrix(K[] keys, V[] values)
        {
            Arr = new double[keys.Length,values.Length];
            Keys = keys;
            Values = values;
        }


        public virtual void Output()
        {

        }
    }
    public abstract class Matrix<K> : Matrix<K,K>
    {
        public Matrix(K[] keys) : base(keys,keys)
        {

        }
    }

    /// <summary>
    /// Матрица сравнения критериев
    /// </summary>
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

    /// <summary>
    /// Матрица сравнения альтернатив по критерию
    /// </summary>
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
                    double val = (double)a.GetCriteriaPriority(Criteria).Priority / b.GetCriteriaPriority(Criteria).Priority;
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
