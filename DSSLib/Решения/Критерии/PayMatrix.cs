using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace DSSLib
{
    public class PayMatrix : Matrix<Alternative,Case>
    {
        public override string ToString() => $"Матрица" + (Safe ? " минимального риска" : " максимального выигрыша");

        private bool Safe { get; set; }

        //private AlternativeRelation<Case> GetMinFromColumn(int c)
        //{
        //    return new AlternativeRelation<Case>()
        //}
        //Минимум в строке / столбце
        //Максимум в строке / столбце
        //Минимум среди списка отношений
        //Максимум среди списка отношений

        public PayMatrix(IEnumerable<Alternative> alternatives, IEnumerable<Case> cases,bool win) : base(alternatives.ToArray(),cases.ToArray())
        {

            int altCount = alternatives.Count();
            int casesCount = cases.Count();
            Safe = !win;

            for (int x = 0; x < altCount; x++)
            {
                Alternative alternative = alternatives.ElementAt(x);
                for (int y = 0; y < casesCount; y++)
                {
                    Case caseS = cases.ElementAt(y);
                    Arr[x, y] = alternative.GetCaseProfit(caseS).Profit;

                }
            }

            if (Safe)
            {
                double max = double.MinValue;
                for (int x = 0; x < altCount; x++)
                {
                    for (int y = 0; y < casesCount; y++)
                    {
                        if (Arr[x, y] > max)
                            max = Arr[x, y];
                    }
                }
                for (int x = 0; x < altCount; x++)
                {
                    for (int y = 0; y < casesCount; y++)
                    {
                        Arr[x, y] = Arr[x, y] * -1 + max;
                    }
                }

            }

            WaldBest = GetWaldBest();
            AzartBest = GetAzartBest();
            BaiesBest = GetBaiesBest();
            LaplasBest = GetLaplasBest();
            SavigeBest = GetSavigeBest();

            LemanBest = GetLemanBest();
            MultiBest = GetMultuBest();
            GurvitsBest = GetGurvitsBest();
            GerrBest = GetGerrBest();
        }

        public List<(Alternative a, double profit)> WaldBest;
        public List<(Alternative a, double profit)> GetWaldBest()
        {
            (Alternative a, double profit)[] mins = new (Alternative a, double profit)[Rows];
            for (int r = 0; r < Rows; r++)
            {
                double min = Arr[r, 0];
                for (int c = 0; c < Columns; c++)
                {
                    if (Arr[r, c] <= min)
                    {
                        min = Arr[r, c];
                    }
                    mins[r] = (Keys[r], min);
                }
            }
            double max = mins.Select(p => p.profit).Max();
            return mins.Where(p => p.profit == max).ToList();
        }


        public List<(Alternative a, double profit)> AzartBest;
        public List<(Alternative a, double profit)> GetAzartBest()
        {
            (Alternative a, double profit)[] mins = new (Alternative a, double profit)[Rows];
            for (int r = 0; r < Rows; r++)
            {
                double maxe = Arr[r, 0];
                for (int c = 0; c < Columns; c++)
                {
                    if (Arr[r, c] >= maxe)
                    {
                        maxe = Arr[r, c];
                    }
                    mins[r] = (Keys[r],maxe);
                }
            }
            double max = mins.Select(p => p.profit).Max();
            return mins.Where(p => p.profit == max).ToList();
        }


        public List<(Alternative a, double profit)> BaiesBest;
        public List<(Alternative a, double profit)> GetBaiesBest()
        {
            (Alternative a, double profit)[] averages = new (Alternative a, double profit)[Rows];
            for (int r = 0; r < Rows; r++)
            {
                double sum = 0;
                for (int c = 0; c < Columns; c++)
                {
                    sum += Arr[r, c] * Values[c].Chance;
                }
                averages[r] = (Keys[r],sum);
            }
            double max = averages.Select(e => e.profit).Max();
            return averages.Where(a => a.profit == max).ToList();
        }


        public List<(Alternative a, double profit)> LaplasBest;
        public List<(Alternative a, double profit)> GetLaplasBest()
        {
            (Alternative a, double profit)[] averages = new (Alternative a, double profit)[Rows];
            for (int r = 0; r < Rows; r++)
            {
                double sum = 0;
                for (int c = 0; c < Columns; c++)
                {
                    sum += Arr[r, c];
                }
                averages[r] = (Keys[r],sum / Columns);
            }
            double max = averages.Select(e => e.profit).Max();
            return averages.Where(a => a.profit == max).ToList();
        }


        public List<(Alternative a, double profit)> SavigeBest;
        public List<(Alternative a, double profit)> GetSavigeBest()
        {
            (Case c, double profit)[] maxes = new (Case c, double profit)[Columns];
            for (int c = 0; c < Columns; c++)
            {
                double maxim = Arr[0,c];
                for (int r = 0; r < Rows; r++)
                {
                    if(Arr[r,c] > maxim)
                    {
                        maxim = Arr[r, c];
                    }
                }
                maxes[c] = (Values[c],maxim);
            }


            (Alternative a, double profit)[] maxes2 = new (Alternative a, double profit)[Rows];
            double[,] newArr = new double[Rows, Columns];
            for (int r = 0; r < Rows; r++)
            {
                double maxe = double.MinValue;
                for (int c = 0; c < Columns; c++)
                {
                    newArr[r, c] = maxes[c].profit - Arr[r, c];
                    if (newArr[r, c] > maxe)
                        maxe = newArr[r, c];
                }
                maxes2[r] = (Keys[r], maxe);
            }
            double min = maxes2.Select(e => e.profit).Min();
            return maxes2.Where(a => a.profit == min).ToList();
        }



        double GurvitsCoeff { get; set; } = 0.4; //коэффициент пессимизма
        public List<(Alternative a, double profit)> GurvitsBest;
        public List<(Alternative a, double profit)> GetGurvitsBest()
        {
            (Alternative a, double profit)[] gurv = new (Alternative a, double profit)[Rows];
            for (int r = 0; r < Rows; r++)
            {
                double maximum = Arr[r,0];
                double minimum = Arr[r, 0];
                for (int c = 0; c < Columns; c++)
                {
                    if(Arr[r,c] >= maximum)
                    {
                        maximum = Arr[r, c];
                    }
                    if(Arr[r,c] < minimum)
                    {
                        minimum = Arr[r, c];
                    }
                }
                gurv[r] = (Keys[r], minimum * GurvitsCoeff + maximum * (1 - GurvitsCoeff));
            }
            double max = gurv.Select(e => e.profit).Max();
            return gurv.Where(a => a.profit == max).ToList();
        }


        double LemanCoeff { get; set; } = 0.6; //коэффициент доверия к информации
        public List<(Alternative a, double profit)> LemanBest;
        public List<(Alternative a, double profit)> GetLemanBest()
        {
            (Alternative a, double profit)[] coeff = new (Alternative a, double profit)[Rows];
            for (int r = 0; r < Rows; r++)
            {
                double min = Arr[r, 0];
                double sum = 0;
                for (int c = 0; c < Columns; c++)
                {
                    if (Arr[r, c] < min)
                        min = Arr[r, c];
                    sum += Arr[r, c] * Values[c].Chance;
                }
                coeff[r] = (Keys[r], sum * LemanCoeff + (1 - LemanCoeff) * min);
            }
            double max = coeff.Select(e => e.profit).Max();
            return coeff.Where(a => a.profit == max).ToList();
        }


        public List<(Alternative a, double profit)> MultiBest;
        public List<(Alternative a, double profit)> GetMultuBest()
        {
            (Alternative a, double profit)[] multi = new (Alternative a, double profit)[Rows];
            for (int r = 0; r < Rows; r++)
            {
                double sum = Arr[r,0];
                for (int c = 1; c < Columns; c++)
                {
                    sum *= Arr[r, c];
                }
                multi[r] = (Keys[r],sum);
            }
            double max = multi.Select(e => e.profit).Max();
            return multi.Where(a => a.profit == max).ToList();
        }


        public List<(Alternative a, double profit)> GerrBest;
        public List<(Alternative a, double profit)> GetGerrBest()
        {
            double max = AzartBest.First().profit + 1;
            double[,] newArr = new double[Rows,Columns];
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    newArr[r, c] = Arr[r,c] > 0 ?  Arr[r, c] / Values[c].Chance : Arr[r,c] * Values[c].Chance;
                }
            }
            (Alternative a, double profit)[] mini = new (Alternative a, double profit)[Rows];
            for (int r = 0; r < Rows; r++)
            {
                double m = newArr[r, 0];
                for (int c = 1; c < Columns; c++)
                {
                    if(newArr[r,c] < m)
                    {
                        m = newArr[r, c];
                    }
                }
                mini[r] = (Keys[r],m);
            }

            double maxim = mini.Select(e => e.profit).Max();
            return mini.Where(a => a.profit == maxim).ToList();
        }


        public override void Output()
        {
            Console.WriteLine($"{ToString()}");

            Console.Write("Альтернативы / исходы".PadRight(30));
            Values.ToList().ForEach(k => Console.Write(k.Name.PadRight(20)));
            Console.WriteLine();
            for(int i = 0; i < Rows; i++)
            {
                Alternative alt = Keys[i];
                Console.Write(alt.Name.PadRight(30));
                for (int a = 0; a < Columns; a++)
                {
                    Case c = Values[a];
                    Console.Write(Arr[i, a].ToString().PadRight(20));
                }
                Console.WriteLine();
            }
            Console.WriteLine("Критерии:");
            Console.Write("- Критерий Вальда: ".PadRight(30));
            foreach (var best in WaldBest)
            {
                string text = $"[{best.a.ID}: {best.profit}] ";
                Console.Write(text.PadRight(15));
            }
            Console.WriteLine();

            Console.Write("- Критерий азартного игрока: ".PadRight(30));
            foreach (var best in AzartBest)
            {
                string text = $"[{best.a.ID}: {best.profit}] ";
                Console.Write(text.PadRight(15));
            }
            Console.WriteLine();

            Console.Write("- Критерий Байеса: ".PadRight(30));
            foreach (var best in BaiesBest)
            {
                string text = $"[{best.a.ID}: {best.profit}] ";
                Console.Write(text.PadRight(15));
            }
            Console.WriteLine();

            Console.Write("- Критерий Лапласа: ".PadRight(30));
            foreach (var best in LaplasBest)
            {
                string text = $"[{best.a.ID}: {best.profit}] ";
                Console.Write(text.PadRight(15));
            }
            Console.WriteLine();

            Console.Write("- Критерий Сэвиджа: ".PadRight(30));
            foreach (var best in SavigeBest)
            {
                string text = $"[{best.a.ID}: {best.profit}] ";
                Console.Write(text.PadRight(15));
            }
            Console.WriteLine();

            Console.Write($"- Критерий Гурвица {GurvitsCoeff}: ".PadRight(30));
            foreach (var best in GurvitsBest)
            {
                string text = $"[{best.a.ID}: {best.profit}] ";
                Console.Write(text.PadRight(15));
            }
            Console.WriteLine();

            Console.Write($"- Критерий Ходжа-Лемана {LemanCoeff}: ".PadRight(30));
            foreach (var best in LemanBest)
            {
                string text = $"[{best.a.ID}: {best.profit}] ";
                Console.Write(text.PadRight(15));
            }
            Console.WriteLine();

            Console.Write($"- Критерий произведений: ".PadRight(30));
            foreach (var best in MultiBest)
            {
                string text = $"[{best.a.ID}: {best.profit}] ";
                Console.Write(text.PadRight(15));
            }
            Console.WriteLine();

            Console.Write($"- Критерий Гермейера: ".PadRight(30));
            foreach (var best in GerrBest)
            {
                string text = $"[{best.a.ID}: {best.profit}] ";
                Console.Write(text.PadRight(15));
            }
            Console.WriteLine();


            Console.WriteLine();
        }

    }

}
