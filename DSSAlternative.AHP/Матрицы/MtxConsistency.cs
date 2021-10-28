using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    //Согласованность
    public static class MtxConsistency
    {
        public static double Nmax(this double[,] mtx)
        {
            return mtx.Multiply(mtx.MultiChecker()).Sum();
        }
        private static double NmaxOld(this double[,] mtx) => mtx.MultiMatrixLocalCoeffs().Sum();
        private static double[] MultiChecker(this double[,] mtx)
        {
            int Size = mtx.Rows();
            double[] rowSum = new double[Size];
            for (int r = 0; r < Size; r++)
            {
                double sum = 0;
                for (int c = 0; c < Size; c++)
                {
                    sum += mtx[r, c];
                }
                rowSum[r] = sum;
            }
            double sumAll = rowSum.Sum();

            return MtxActions.Normalise(rowSum, sumAll);
        }
        //Индекс согласованности
        public static double CI(this double[,] mtx)
        {
            return (mtx.Nmax() - mtx.Rows()) / (mtx.Rows() - 1);
        }

        //Случайный индекс согласованности
        public static double RI(this double[,] mtx)
        {
            int size = mtx.Rows();
            return RISize.ContainsKey(size) ? RISize[size] : (1.98 * (size - 2)) / mtx.Rows();
        }

        //Отношение согласованности
        public static double Cr(this double[,] mtx) => mtx.CI() / mtx.RI();


        public static bool IsCorrect(this double[,] mtx)
        {
            return mtx.IsCorrect(BorderDefault);
        }
        public static bool IsCorrect(this double[,] mtx, double border)
        {
            int Size = mtx.Rows();
            bool little = Size < 3;
            bool unknown = mtx.WithZeros();
            bool consistent = Size >= 3 && mtx.Cr() <= border;

            return little || unknown || consistent;
        }

        public static double BorderDefault { get; set; } = 0.2;

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
    }
}
