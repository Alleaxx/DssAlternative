using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP.MatrixMethods
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

        public static double[][] ConvertToArray(this double[,] matrix)
        {
            List<double[]> cols = new List<double[]>();
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                List<double> row = new List<double>();
                for (int a = 0; a < matrix.GetLength(1); a++)
                {
                    row.Add(matrix[i, a]);
                }
                cols.Add(row.ToArray());
            }
            return cols.ToArray();
        }

    } 
}
