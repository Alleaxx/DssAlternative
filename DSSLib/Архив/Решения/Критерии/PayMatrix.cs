using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSSLib
{
    public class PayMatrix : Matrix<Alternative,Case>
    {
        public override string ToString() => $"Матрица" + (Safe ? " минимального риска" : " максимального выигрыша");

        private bool Safe { get; set; }

        #region Возвращение различных значений из матрицы
        /// <summary>
        /// Возвращает отношение альтернативы-исхода по указанным координатам столбца-строки
        /// </summary>
        /// <param name="r">Номер строки</param>
        /// <param name="c">Номер столбца</param>
        /// <returns>Отношение альтернативы-исхода</returns>
        public new AltCaseRelation Get(int r, int c) => new AltCaseRelation(Keys[r], Values[c], Arr[r,c]);


        /// <summary>
        /// Возвращает минимальное значение из указанной колонки
        /// </summary>
        /// <param name="c">Номер колонки</param>
        /// <returns>Минимальное значение из колокни</returns>
        private AltCaseRelation GetMinFromColumn(int c)
        {
            AltCaseRelation min = Get(0, c);
            for (int r = 0; r < Rows; r++)
            {
                if (Arr[r, c] <= min.Result)
                    min = Get(r, c);
            }
            return min;
        }

        /// <summary>
        /// Возвращает минимальное значение из указанной строки
        /// </summary>
        /// <param name="r">Номер строки</param>
        /// <returns>Минимальное значение из строки</returns>
        private AltCaseRelation GetMinFromRow(int r)
        {
            AltCaseRelation min = Get(r, 0);
            for (int c = 0; c < Columns; c++)
            {
                if (Arr[r, c] <= min.Result)
                    min = Get(r, c);
            }
            return min;
        }

        /// <summary>
        /// Возвращает максимальное значение из указанной колонки
        /// </summary>
        /// <param name="c">Номер колонки</param>
        /// <returns>Максимальное значение</returns>
        private AltCaseRelation GetMaxFromColumn(int c)
        {
            AltCaseRelation max = Get(0, c);
            for (int r = 0; r < Rows; r++)
            {
                if (Arr[r, c] >= max.Result)
                    max = Get(r, c);
            }
            return max;
        }

        /// <summary>
        /// Возвращает максимальное значение из указанной строки
        /// </summary>
        /// <param name="r">Номер строки</param>
        /// <returns>Максимальное значение</returns>
        private AltCaseRelation GetMaxFromRow(int r)
        {
            AltCaseRelation max = Get(r, 0);
            for (int c = 0; c < Columns; c++)
            {
                if (Arr[r, c] >= max.Result)
                    max = Get(r, c);
            }
            return max;
        }

        /// <summary>
        /// Возвращает массив максимальных / минимальных значений матрицы по столбцам или строкам
        /// </summary>
        /// <param name="cr">Столбцы / строки</param>
        /// <param name="func">Функция, возвращающая значение (минимум / максимум) из строки или столбца</param>
        /// <returns>Массив значений матрицы</returns>
        private AltCaseRelation[] GetFromArr(int cr, Func<int, AltCaseRelation> func)
        {
            AltCaseRelation[] res = new AltCaseRelation[cr];
            for (int a = 0; a < cr; a++)
            {
                res[a] = func?.Invoke(a);
            }
            return res;
        }

        /// <summary>
        /// Получает максимальные / минимальные значения из массива, сформированного по максимальным / минимальным значениям из строк или столбцов
        /// </summary>
        /// <param name="cr">Строки или столбцы</param>
        /// <param name="func">Функция, которая возвращает мин/макс значение из строк / столбцов матрицы</param>
        /// <param name="funcEnd">Функция, применяющая к итоговому массиву и возвращающая мин-макс значения</param>
        /// <returns></returns>
        private AltCaseRelation[] GetFromOne(int cr, Func<int, AltCaseRelation> func, Func<IEnumerable<AltCaseRelation>,AltCaseRelation[]> funcEnd)
        {
            AltCaseRelation[] res = new AltCaseRelation[Rows];
            for (int a = 0; a < cr; a++)
            {
                res[a] = func?.Invoke(a);
            }
            return funcEnd.Invoke(res);
        }


        /// <summary>
        /// Возвращает минимальные значения из массива отношений альтернатив и исходов
        /// </summary>
        /// <param name="relations">Список отношений альтернатив-исходов</param>
        /// <returns>Минимальные значения</returns>
        private AltCaseRelation[] GetMinFromArr(IEnumerable<AltCaseRelation> relations)
        {
            double min = relations.Select(r => r.Result).Min();
            return relations.Where(r => r.Result == min).ToArray();
        }
        /// <summary>
        /// Возвращает максимальные значения из массива отношений альтернатив и исходов
        /// </summary>
        /// <param name="relations">Список отношений альтернатив-исходов</param>
        /// <returns>Максимальные значения</returns>
        private AltCaseRelation[] GetMaxFromArr(IEnumerable<AltCaseRelation> relations)
        {
            double max = relations.Select(r => r.Result).Max();
            return relations.Where(r => r.Result == max).ToArray();
        }

        #endregion


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
                    double maxRow = GetMaxFromRow(x).Result;
                    if (maxRow > max)
                        max = maxRow;
                }
                for (int x = 0; x < altCount; x++)
                {
                    for (int y = 0; y < casesCount; y++)
                    {
                        Arr[x, y] = Arr[x, y] * -1 + max;
                    }
                }
            }

            Criterias = new CriteriaGame[]
            {
                new CriteriaGame(CriteriaGame.Types.Wald, GetWaldBest()),
                new CriteriaGame(CriteriaGame.Types.Azart, GetAzartBest()),
                new CriteriaGame(CriteriaGame.Types.Baies, GetBaiesLaplasBest(false)),
                new CriteriaGame(CriteriaGame.Types.Laplas, GetBaiesLaplasBest(true)),
                new CriteriaGame(CriteriaGame.Types.Savige, GetSavigeBest()),

                new CriteriaGame(CriteriaGame.Types.Gurvits, GetGurvitsBest(),GurvitsCoeff),
                new CriteriaGame(CriteriaGame.Types.HojaLeman, GetLemanBest(),LemanCoeff),
                new CriteriaGame(CriteriaGame.Types.Multi, GetMultuBest()),
                new CriteriaGame(CriteriaGame.Types.Gerr, GetGerrBest()),
            };
        }


        //Критерии платежной матрицы
        public CriteriaGame[] Criterias { get; set; }

        #region Получение критериев
        double LemanCoeff { get; set; } = 0.6; //коэффициент доверия к информации
        double GurvitsCoeff { get; set; } = 0.4; //коэффициент пессимизма


        public AltCaseRelation[] GetWaldBest()
        {
            return GetFromOne(Rows, GetMinFromRow, GetMaxFromArr);               
        }
        public AltCaseRelation[] GetAzartBest()
        {
            return GetFromOne(Rows, GetMaxFromRow, GetMaxFromArr);
        }
        public AltCaseRelation[] GetBaiesLaplasBest(bool laplas)
        {
            AltCaseRelation[] averages = new AltCaseRelation[Rows];
            for (int r = 0; r < Rows; r++)
            {
                double sum = 0;
                for (int c = 0; c < Columns; c++)
                {
                    if (!laplas)
                        sum += Arr[r, c] * Values[c].Chance;
                    else
                        sum += Arr[r, c];
                }
                if (!laplas)
                    averages[r] = new AltCaseRelation(Keys[r], null, sum);
                else
                    averages[r] = new AltCaseRelation(Keys[r], null, sum / Columns);
            }
            return GetMaxFromArr(averages);
        }
        public AltCaseRelation[] GetSavigeBest()
        {
            AltCaseRelation[] maxInColumns = GetFromArr(Columns, GetMaxFromColumn);

            AltCaseRelation[] maxInRowsAfter = new AltCaseRelation[Rows];
            double[,] newArr = new double[Rows, Columns];
            for (int r = 0; r < Rows; r++)
            {
                double maxe = double.MinValue;
                for (int c = 0; c < Columns; c++)
                {
                    newArr[r, c] = maxInColumns[c].Result - Arr[r, c];
                    if (newArr[r, c] > maxe)
                        maxe = newArr[r, c];
                }
                maxInRowsAfter[r] = new AltCaseRelation(Keys[r], null, maxe);
            }
            return GetMinFromArr(maxInRowsAfter);
        }
        public AltCaseRelation[] GetGurvitsBest()
        {
            AltCaseRelation[] gurv = new AltCaseRelation[Rows];
            for (int r = 0; r < Rows; r++)
            {
                double maximum = GetMaxFromRow(r).Result;
                double minimum = GetMinFromRow(r).Result;
                gurv[r] = new AltCaseRelation(Keys[r], null, minimum * GurvitsCoeff + maximum * (1 - GurvitsCoeff));
            }
            return GetMinFromArr(gurv);
        }
        public AltCaseRelation[] GetLemanBest()
        {
            AltCaseRelation[] coeff = new AltCaseRelation[Rows];
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
                coeff[r] = new AltCaseRelation(Keys[r],null, sum * LemanCoeff + (1 - LemanCoeff) * min);
            }
            return GetMaxFromArr(coeff);
        }
        public AltCaseRelation[] GetMultuBest()
        {
            AltCaseRelation[] multi = new AltCaseRelation[Rows];
            for (int r = 0; r < Rows; r++)
            {
                double sum = Arr[r,0];
                for (int c = 1; c < Columns; c++)
                {
                    sum *= Arr[r, c];
                }
                multi[r] = new AltCaseRelation(Keys[r],null,sum);
            }
            return GetMaxFromArr(multi);
        }
        public AltCaseRelation[] GetGerrBest()
        {
            double[,] newArr = new double[Rows,Columns];
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    newArr[r, c] = Arr[r,c] > 0 ? Arr[r, c] / Values[c].Chance : Arr[r,c] * Values[c].Chance;
                }
            }

            AltCaseRelation[] minInRows = new AltCaseRelation[Rows];
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
                minInRows[r] = new AltCaseRelation(Keys[r],null,m);
            }
            return GetMaxFromArr(minInRows);
        }
        #endregion


        public override void Output()
        {
            Console.WriteLine($"{ToString()}");

            Console.Write("Альтернативы / исходы".PadRight(30));
            Values.ToList().ForEach(k => Console.Write($"{k.Name} [{k.Chance}]".PadRight(20)));
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

            Criterias.ToList().ForEach(c => PrintResult(c.Name, c.Results));


            void PrintResult(string descr, AltCaseRelation[] arr)
            {
                Console.Write($"- {descr}".PadRight(30));
                foreach (var best in arr)
                {
                    Console.Write($"[{best.Key.ID}: {Math.Round(best.Result,3)}] ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }

    //Критерии платежной матрицы
    public class CriteriaGame : Criteria
    {
        public enum Types
        {
            Wald,
            Azart,
            Baies,
            Laplas,
            Savige,
            Gurvits,
            HojaLeman,
            Multi,
            Gerr
        }
        private static Dictionary<Types, string> Names { get; set; } = new Dictionary<Types, string>
        {
            [Types.Wald] = "Критерий Вальда",
            [Types.Azart] = "Критерий азартного ирока",
            [Types.Baies] = "Критерий Байеса",
            [Types.Laplas] = "Критерий Лапласа",
            [Types.Savige] = "Критерий Сэвиджа",
            [Types.Gurvits] = "Критерий Гурвица",
            [Types.HojaLeman] = "Критерий Ходжа-Лемана",
            [Types.Multi] = "Критерий произведений",
            [Types.Gerr] = "Критерий Гермейера",

        };



        public Types Type { get; set; }
        public AltCaseRelation[] Results { get; set; }
        public double Coeff { get; set; }


        public CriteriaGame(Types type,AltCaseRelation[] relations, double c = 0)
        {
            Name = Names[type];
            Results = relations;
            Coeff = c;
            if (c != 0)
                Name += $" {Coeff}";
        }
    }
}
