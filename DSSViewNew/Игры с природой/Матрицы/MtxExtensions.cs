using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    //Методы расширения для матрицы (многомерного массива)
    static class MtxExtensions
    {
        public static double Rows(this double[,] arr) => arr.GetLength(0);
        public static double Cols(this double[,] arr) => arr.GetLength(1);

        public static double MinFromRow(this double[,] arr, int r)
        {
            var cols = arr.Cols();

            double min = arr[r, 0];
            for (int c = 0; c < cols; c++)
            {
                if (arr[r, c] <= min)
                    min = arr[r, c];
            }
            return min;
        }
        public static double MaxFromRow(this double[,] arr, int r)
        {
            var cols = arr.Cols();

            double max = arr[r, 0];
            for (int c = 0; c < cols; c++)
            {
                if (arr[r, c] >= max)
                    max = arr[r, c];
            }
            return max;
        }

        //Получение матрицы рисков
        public static double[,] GetRiscMatrix(this double[,] from)
        {
            int rows = from.GetLength(0);
            int cols = from.GetLength(1);
            double[] maxesInCols = new double[cols];
            for (int c = 0; c < cols; c++)
            {
                double max = double.MinValue;
                for (int r = 0; r < rows; r++)
                {
                    if (from[r, c] > max)
                        max = from[r, c];
                }
                maxesInCols[c] = max;
            }


            double[,] riscMatrix = new double[rows, cols];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    riscMatrix[r, c] = maxesInCols[c] - from[r, c];
                }
            }

            return riscMatrix;
        }
    }
}
