using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBlazorEmpty.AHP
{
    public interface IConsistency
    {
        bool IsCorrect(double border);
        bool IsCorrect();
        double Cr { get; }

        public double Nmax { get; }
        public double NmaxAlpha { get; }

        public double CI { get; }


        //Стохастический индекс согласованности
        public double RI { get; }

        double[] MultiMatrixLocalCoeffs { get; }


    }
    public class MatrixConsistenct : IConsistency
    {
        public override string ToString() => "Согласованность матрицы";
        private IMatrix Mtx { get; set; }
        private int Size => Mtx.Size;


        public double Nmax => NmaxAlpha;
        public double NmaxOld => MultiMatrixLocalCoeffs.Sum();
        public double NmaxAlpha => Matrix.MatrixMultiplication(Mtx.Array, MultiChecker()).Sum();




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

        public bool IsCorrect() => IsCorrect(BorderConsistenct);
        public bool IsCorrect(double border)
        {
            if (Size < 3)
                return true;
            else if (Mtx.WithZeros())
                return true;
            else if (Size >= 3 && Cr <= border)
                return true;
            else
                return false;
        }

        public static double BorderConsistenct { get; set; } = 0.2;


        //Порог 0.1
        public double[] MultiMatrixLocalCoeffs => AHP.Matrix.MatrixMultiplication(Mtx.Array, Mtx.Coeffiients);


        public double[] MultiChecker()
        {
            double[] rowSum = new double[Size]; 
            for (int r = 0; r < Size; r++)
            {
                double sum = 0;
                for (int c = 0; c < Size; c++)
                {
                    sum += Mtx.Array[r, c];
                }
                rowSum[r] = sum;
            }
            double sumAll = rowSum.Sum();

            return AHP.Matrix.Normalise(rowSum, sumAll);
        }

        public MatrixConsistenct(IMatrix matrix)
        {
            Mtx = matrix;
        }
    }
}
