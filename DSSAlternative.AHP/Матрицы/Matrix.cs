using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface IMatrix
    {
        int Rows { get; }
        int Cols { get; }

        double[,] Array { get; }
        double[] Coeffiients { get; }
        bool WithZeros();


        double Cr { get; }
        bool IsCorrect { get; }

        string GetText();
    }
    public class Matrix : IMatrix 
    {
        public override string ToString() => $"Матрица ({Rows}x{Cols})";
        
        public double[,] Array { get; set; }
        public int Rows => Array.Rows();
        public int Cols => Array.Cols();


        public double Cr => Array.Cr();
        public bool IsCorrect => Array.IsCorrect();

        public double[] Coeffiients => Array.LocalCoeffs();
        public bool WithZeros() => Array.WithZeros();
        public string GetText() => Array.Text();

        protected Matrix()
        {

        }
        public Matrix(double[,] arr)
        {
            int rows = arr.GetLength(0);
            int cols = arr.GetLength(1);

            Array = new double[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int a = 0; a < cols; a++)
                {
                    Array[i, a] = arr[i,a];
                }
            }
        }
    }
}
