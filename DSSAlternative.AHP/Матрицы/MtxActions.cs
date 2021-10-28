using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public static class MtxActions
    {
        //Действия с матрицами
        public static double[] GeometricMultiVector(this double[,] mtx)
        {
            int size = mtx.GetLength(0);

            double[] vector = new double[size];
            for (int r = 0; r < size; r++)
            {
                double multi = 1;
                for (int c = 0; c < size; c++)
                {
                    multi *= mtx[r, c];
                }
                vector[r] = multi;

            }
            return vector;
        }
        public static double[] Multiply(this double[,] mtx, double[] vector)
        {
            int rsize = mtx.GetLength(0);
            int vsize = vector.Length;
            double[] results = new double[rsize];
            for (int r = 0; r < rsize; r++)
            {
                double res = 0;
                for (int c = 0; c < vsize; c++)
                {
                    res += mtx[r, c] * vector[c];
                }
                results[r] = res;
            }
            return results;
        }
        public static double[] LocalCoeffs(this double[,] mtx)
        {
            int size = mtx.GetLength(0);
            var geometricMulti = mtx.GeometricMultiVector();

            var multiPowed = new double[size];
            for (int i = 0; i < size; i++)
            {
                multiPowed[i] = Math.Pow(geometricMulti[i], 1 / (double)size);
            }
            var coeffs = Normalise(multiPowed, multiPowed.Sum());

            return coeffs;
        }
        public static double[] MultiMatrixLocalCoeffs(this double[,] mtx)
        {
            var local = mtx.LocalCoeffs();
            return mtx.Multiply(local);
        }

        private static double[] Normalise2(this double[] mtx, double? max = null)
        {
            return Normalise(mtx, max);
        }
        public static double[] Normalise(double[] vector, double? max = null)
        {
            int size = vector.Length;
            max ??= size;

            var results = new double[size];
            vector.CopyTo(results, 0);
            for (int i = 0; i < size; i++)
            {
                results[i] = vector[i] / max.Value;
            }
            return results;
        }

    } 
}
