using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSLib
{
    public class MatrixConsistenct
    {
        public override string ToString() => "Согласованность матрицы";
        private MatrixAHP Matrix { get; set; }
        private int Size => Matrix.Size;



        public double Nmax => MultiCheck.Sum();

        //Индекс согласованности
        public double CI => (Nmax - Size) / (Size - 1);

        //Стохастический индекс согласованности
        public double RI => RISize.ContainsKey(Size) ? RISize[Size] : (1.98 * (Size - 2)) / Size;
        private static Dictionary<int, double> RISize { get; set; } = new Dictionary<int, double>
        {
            [1] = 0,
            [2] = 0,
            [3] = 0.58,
            [4] = 0.9,
            [5] = 1.12,
            [6] = 1.24,
            [7] = 1.32,
            [8] = 1.41,
            [9] = 1.45,
            [10] = 1.49,
            [11] = 1.51,
            [12] = 1.48,
            [13] = 1.56,
            [14] = 1.57,
            [15] = 1.59,
        };


        //Коэффициент согласованности (отношение согласованности)
        public double Cr => CI / RI;


        public string Report
        {
            get
            {
                if (IsCorrect)
                    return "Матрица согласована";
                else
                    return "Матрица НЕ согласована";
            }
        }
        public bool IsCorrect
        {
            get
            {
                if (Size < 3)
                    return true;
                else if (Size >= 3 && Cr <= BorderConsistenct)
                    return true;
                else
                    return false;

            }
        }

        public static double BorderConsistenct { get; set; } = 0.15;


        //Порог 0.1
        public double[] MultiCheck
        {
            get
            {
                double[] coeffs = Matrix.Coeffiients;
                double[] sumCols = Matrix.SumColumns;
                double[] multiCheck = new double[Size];
                //for (int y = 0; y < Size; y++)
                //{
                //    double res = 0;
                //    for (int x = 0; x < Size; x++)
                //    {
                //        res += (Matrix.Array[y,x] * coeffs[x]);
                //    }
                //    multiCheck[y] = res;
                //}

                for (int i = 0; i < Size; i++)
                {
                    multiCheck[i] = sumCols[i] * coeffs[i];
                }


                return multiCheck;
            }
        }


        public MatrixConsistenct(MatrixAHP matrix)
        {
            Matrix = matrix;
        }
    }
}
