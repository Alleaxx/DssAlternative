using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSLib.New
{
    public interface IMatrixAHP
    {
        int Size { get; }
    }
    public class MatrixAHP : IMatrixAHP
    {
        public override string ToString() => $"Матрица ({Array.GetLength(0)}x{Array.GetLength(1)})";
        
        public double[,] Array { get; set; }
        public int Size => Array.GetLength(0);

        public MatrixConsistenct Consistency { get; set; }


        public double Sum
        {
            get
            {
                double sum = 0;
                for (int y = 0; y < Size; y++)
                {
                    for (int x = 0; x < Size; x++)
                    {
                        sum += Array[x,y];
                    }
                }
                return sum;
            }
        }
        public double[] SumRows
        {
            get
            {
                double[] sumCols = new double[Size];
                for (int x = 0; x < Size; x++)
                {
                    double sumCol = 0;
                    for (int y = 0; y < Size; y++)
                    {
                        sumCol += Array[x,y];
                    }
                    sumCols[x] = sumCol;
                }
                return sumCols;

            }
        }
        public double[] SumColumns
        {
            get
            {
                double[] sumRows = new double[Size];
                for (int y = 0; y < Size; y++)
                {
                    double sumCol = 0;
                    for (int x = 0; x < Size; x++)
                    {
                        sumCol += Array[x,y];
                    }
                    sumRows[y] = sumCol;
                }
                return sumRows;
            }
        }
        public double[] Coeffiients
        {
            get
            {
                double[] test = new double[Size];
                for (int y = 0; y < Size; y++)
                {
                    test[y] = 1;
                    for (int x = 0; x < Size; x++)
                    {
                        test[y] *= Array[y, x];
                    }
                    test[y] = Math.Pow(test[y], 1 / (double)Size);
                }
                double r = test.Sum();
                double[] test2 = new double[Size];
                for (int i = 0; i < Size; i++)
                {
                    test2[i] = test[i] / r;
                }
                return test2;


                //double[,] normalised = Normalised;

                //double[] coeffs = new double[Size];
                //for (int x = 0; x < Size; x++)
                //{
                //    for (int y = 0; y < Size; y++)
                //    {
                //        coeffs[x] += normalised[x, y];
                //    }
                //}
                //return coeffs;

            }
        }

        //public double[,] Normalised
        //{
        //    get
        //    {
        //        int size = Information.Count();
        //        double[] sumCols = SumRows;

        //        double[,] normalised = new double[size,size];
        //        for (int x = 0; x < size; x++)
        //        {
        //            for (int y = 0; y < size; y++)
        //            {
        //                normalised[y, x] = Array[x,y] / sumCols[x];
        //            }
        //        }
        //        return normalised;

        //    }
        //}
        public double[,] Normalised
        {
            get
            {
                double[,] normalised = new double[Size, Size];
                for (int x = 0; x < Size; x++)
                {
                    for (int y = 0; y < Size; y++)
                    {
                        normalised[x, y] = Array[x,y] / Sum;
                    }
                }
                return normalised;
            }
        }


        public double[] RowAveragesFromNormalised
        {
            get
            {
                double[,] normalised = Normalised;

                double[] averagesRowNor = new double[Size];
                for (int x = 0; x < Size; x++)
                {
                    double sumRow = 0;
                    for (int y = 0; y < Size; y++)
                    {
                        sumRow += normalised[x,y];
                    }
                    averagesRowNor[x] = sumRow / Size;
                }
                return averagesRowNor;
            }
        }


        protected MatrixAHP()
        {

        }
        public MatrixAHP(double[,] arr)
        {
            Array = arr;
            Consistency = new MatrixConsistenct(this);
        }


        private string Get(double[,] arr)
        {
            string text = "";
            for (int i = 0; i < Size; i++)
            {
                for (int a = 0; a < Size; a++)
                {
                    text += $"{Math.Round(arr[i,a],5)} ";
                }
                text += "\n";
            }
            return text;
        }
    }
}
