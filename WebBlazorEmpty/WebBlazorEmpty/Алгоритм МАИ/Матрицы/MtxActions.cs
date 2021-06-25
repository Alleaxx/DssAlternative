using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    static class MtxActions
    {
        public static double[] GeometricMultiVector(double[,] mtx)
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

        public static double[] MatrixMultiplication(double[,] mtx, double[] vector)
        {
            int rsize = mtx.GetLength(0);
            int size = vector.Length;
            double[] results = new double[rsize];
            for (int r = 0; r < rsize; r++)
            {
                double res = 0;
                for (int c = 0; c < size; c++)
                {
                    res += mtx[r, c] * vector[c];
                }
                results[r] = res;
            }
            return results;
        }

        public static double[] Normalise(double[] vector, double? max = null)
        {
            int size = vector.Length;
            max = max ?? size;

            var results = new double[size];
            vector.CopyTo(results, 0);
            for (int i = 0; i < size; i++)
            {
                results[i] = vector[i] / max.Value;
            }
            return results;
        }

        public static double[] LocalCoefficients(double[,] mtx)
        {
            int size = mtx.GetLength(0);
            var geometricMulti = GeometricMultiVector(mtx);

            var multiPowed = new double[size];
            for (int i = 0; i < size; i++)
            {
                multiPowed[i] = Math.Pow(geometricMulti[i], 1 / (double)size);
            }
            var coeffs = Normalise(multiPowed, multiPowed.Sum());

            return coeffs;
        }
    }

}
