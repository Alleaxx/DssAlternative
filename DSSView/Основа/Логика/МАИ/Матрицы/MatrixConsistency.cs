using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
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
        public double RI => (1.98 * (Size - 2)) / Size;

        //Коэффициент согласованности
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
                else if (Size >= 3 && Cr <= 0.1)
                    return true;
                else
                    return false;

            }
        }


        //Порог 0.1
        public double[] MultiCheck
        {
            get
            {
                double[] averagesRowNor = Matrix.Coeffiients;

                double[] multiCheck = new double[Size];
                for (int y = 0; y < Size; y++)
                {
                    double res = 0;
                    for (int x = 0; x < Size; x++)
                    {
                        res += (Matrix.Array[y,x] * averagesRowNor[x]);
                    }
                    multiCheck[y] = res;
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
