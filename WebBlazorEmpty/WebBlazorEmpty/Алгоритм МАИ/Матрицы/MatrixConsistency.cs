using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBlazorEmpty.AHP
{
    public class MatrixConsistenct
    {
        public override string ToString() => "Согласованность матрицы";
        private MatrixAHP Matrix { get; set; }
        private int Size => Matrix.Size;


        public double Nmax => NmaxAlpha;
        public double NmaxOld => MultiMatrixLocalCoeffs.Sum();
        public double NmaxAlpha => MatrixAHP.MatrixMultiplication(Matrix.Array, MultiChecker()).Sum();




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


        public bool IsCorrect(double border = BorderConsistenct)
        {
            if (Size < 3)
                return true;
            else if (Size >= 3 && Cr <= border)
                return true;
            else
                return false;
        }

        public const double BorderConsistenct = 0.15;


        //Порог 0.1
        public double[] MultiMatrixLocalCoeffs => MatrixAHP.MatrixMultiplication(Matrix.Array, Matrix.Coeffiients);


        public double[] MultiChecker()
        {
            double[] rowSum = new double[Size]; 
            for (int r = 0; r < Size; r++)
            {
                double sum = 0;
                for (int c = 0; c < Size; c++)
                {
                    sum += Matrix.Array[r, c];
                }
                rowSum[r] = sum;
            }
            double sumAll = rowSum.Sum();

            return MatrixAHP.Normalise(rowSum, sumAll);
        }

        public MatrixConsistenct(MatrixAHP matrix)
        {
            Matrix = matrix;
        }
    }
}
