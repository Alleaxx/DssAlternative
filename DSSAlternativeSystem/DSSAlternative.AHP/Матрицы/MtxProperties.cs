using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    //Свойства матрицы
    public static class MtxProperties
    {
        public static bool IsSymetric(this double[,] mtx, bool ignoreNulls = true)
        {
            for (int rc = 0; rc < mtx.Rows(); rc++)
            {
                if (mtx[rc, rc] != 1)
                {
                    return false;
                }
            }

            int toCheckRows = (int)(Math.Ceiling((double)((double)mtx.Rows() / 2)));
            for (int r = 0; r < toCheckRows; r++)
            {
                for (int c = 1; c < mtx.Cols() - r; c++)
                {
                    int fromRow = r;
                    int toRow = mtx.Rows() - 1 - r;
                    int fromCol = c;
                    int toCol = mtx.Cols() - 1 - c;

                    double firstVal = mtx[fromRow, fromCol];
                    double secVal = mtx[toRow, toCol];

                    if (ignoreNulls && (firstVal == 0 || secVal == 0))
                    {
                        continue;
                    }
                    else if (firstVal != 1 / secVal)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public static bool WithZeros(this double[,] mtx)
        {

            for (int x = 0; x < mtx.Rows(); x++)
            {
                for (int y = 0; y < mtx.Cols(); y++)
                {
                    if (mtx[x, y] == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static int Rows(this double[,] mtx)
        {
            return mtx.GetLength(0);
        }
        public static int Cols(this double[,] mtx)
        {
            return mtx.GetLength(1);
        }

        public static string Text(this double[,] Array)
        {
            string text = "";
            for (int x = 0; x < Array.Rows(); x++)
            {
                text += "\n";
                for (int y = 0; y < Array.Cols(); y++)
                {
                    text += $"{Math.Round(Array[x, y], 3),-7}";
                }
            }
            return text;
        }
    }
}
