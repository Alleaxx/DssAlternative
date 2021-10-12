using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface IMatrix
    {
        int RSize { get; }
        int CSize { get; }

        double[,] Array { get; }
        double[] Coeffiients { get; }
        IConsistency Consistency { get; }
        bool WithZeros();

        string GetText();
    }
    public class Matrix : IMatrix 
    {
        public override string ToString() => $"Матрица ({RSize}x{CSize})";
        
        public double[,] Array { get; set; }
        public int RSize => Array.GetLength(0);
        public int CSize => Array.GetLength(1);

        public IConsistency Consistency { get; protected set; }

        public double[] Coeffiients => MtxActions.LocalCoefficients(Array);

        public bool WithZeros()
        {
            for (int x = 0; x < RSize; x++)
            {
                for (int y = 0; y < CSize; y++)
                {
                    if (Array[x, y] == 0)
                        return true;
                }
            }
            return false;
        }


        protected Matrix()
        {
            Consistency = new MatrixConsistenct(this);
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
            Consistency = new MatrixConsistenct(this);
        }


        public string GetText()
        {
            string text = "";
            for (int x = 0; x < RSize; x++)
            {
                text += "\n";
                for (int y = 0; y < CSize; y++)
                {
                    text += $"{Math.Round(Array[x, y], 3),-7}";
                }
            }
            return text;
        }
    }
}
