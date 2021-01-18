using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{

    /// <summary>
    /// Матрица
    /// </summary>
    class Data
    {
        public double[,] Matrix { get; set; }
        public int Rows => Matrix.GetLength(0);
        public int Cols => Matrix.GetLength(1);

        public InfoData Information { get; set; }

        public InfoCriterias InfoCriterias { get; set; }



        public Data MainMatrix { get; set; }
        public bool IsSafeDependent => MainMatrix != null;
        public Data SafeData { get; set; }

        public Data(int rows, int cols, InfoData info = null)
        {
            Matrix = new double[rows,cols];
            for (int i = 0; i < Rows; i++)
            {
                for (int a = 0; a < Cols; a++)
                {
                    Matrix[i,a] = 0;
                }
            }

            Information = info;
            if(Information == null)
            {
                Information = new InfoData($"Матрица {rows}x{cols}",rows,cols);
            }

            InfoCriterias = new InfoCriterias(this);
            SafeData = new Data(this);
        }
        public Data(Data data)
        {
            MainMatrix = data;
            UpdateFromMain();
            Information = new InfoData("Матрица наименьшего риска", Rows, Cols);
            InfoCriterias = new InfoCriterias(this);
        }
        protected virtual void UpdateFromMain()
        {
            Matrix = new double[MainMatrix.Rows, MainMatrix.Cols];

            double max = double.MinValue;

            for (int x = 0; x < Rows; x++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (MainMatrix.Matrix[x, c] > max)
                        max = MainMatrix.Matrix[x, c];
                }
            }

            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Cols; y++)
                {
                    Matrix[x, y] = MainMatrix.Matrix[x, y] * -1 + max;
                }
            }

            if(InfoCriterias != null)
                InfoCriterias.Update();
        }


        public virtual void SetValue(Coords coords, double value)
        {
            Matrix[coords.X, coords.Y] = value;

            if (SafeData != null)
                SafeData.UpdateFromMain();
            if (InfoCriterias != null)
                InfoCriterias.Update();
        }
    }

    /// <summary>
    /// Дополнительная информация о матрице
    /// </summary>
    class InfoData
    {
        public string Name { get; set; }
        public string[] Rows { get; set; }
        public string[] Cols { get; set; }

        public InfoData(string name, int rows, int cols)
        {
            Name = name;
            Rows = new string[rows];
            for (int i = 0; i < rows; i++)
            {
                Rows[i] = $"Строка {i}";
            }
            Cols = new string[cols];
            for (int i = 0; i < cols; i++)
            {
                Cols[i] = $"Столбец {i}";
            }
        }
    }

}
