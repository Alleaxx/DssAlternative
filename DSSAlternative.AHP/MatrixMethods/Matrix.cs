﻿using DSSAlternative.AHP.HierarchyInfo;
using DSSAlternative.AHP.Relations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP.MatrixMethods
{
    /// <summary>
    /// Интерфейс матрицы с возможностью изменения значений
    /// </summary>
    public interface IMatrix
    {
        int Rows { get; }
        int Cols { get; }

        double[,] Array { get; }
        double[] Coeffiients { get; }


        double Cr { get; }
        bool IsCorrect { get; }
        bool Consistent { get; }

        bool WithZeros();
        string GetText();

        void Change(INode a, INode b, double value);
        void Change(int x, int y, double value);
    }

    /// <summary>
    /// Матрица с возможностью изменения значений
    /// </summary>
    public class Matrix : IMatrix 
    {
        public override string ToString()
        {
            return $"Матрица ({Rows}x{Cols})";
        }
        
        public double[,] Array { get; set; }
        public int Rows => Array.Rows();
        public int Cols => Array.Cols();


        public double Cr => Array.Cr();
        public bool IsCorrect => Array.IsCorrect();
        public bool Consistent => Array.IsCorrect();

        public double[] Coeffiients => Array.LocalCoeffs();

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

        public bool WithZeros() => Array.WithZeros();
        public string GetText() => Array.Text();

        //Матрица отношений (которая редактируется в интерфейсе)
        public static IMatrix CreateRelations(IRelationsGrouped problem, INode node)
        {
            var rels = problem.RelationsGroupedMain(node);
            int length = rels.Count();

            var array = new double[length, length];
            for (int r = 0; r < length; r++)
            {
                var row = rels.ElementAt(r);
                for (int c = 0; c < length; c++)
                {
                    array[r, c] = row.ElementAt(c).Value;
                }
            }
            return new Matrix(array);
        }

        //Матрица -1 локальных коэффициентов для узла
        public static IMatrix CreateLocalCoeffs(IRelationsGrouped problem, INode node)
        {
            var nodes = node.NodeControllers();
            double[,] array = null;
            if (nodes.Any())
            {
                var coeffs = nodes.Select(n => CreateRelations(problem, n).Coeffiients).ToArray();
                int rows = coeffs.First().Length;
                int cols = coeffs.Length;

                array = new double[rows, cols];
                for (int c = 0; c < cols; c++)
                {
                    var coeffsNow = coeffs[c];
                    for (int r = 0; r < rows; r++)
                    {
                        array[r, c] = coeffsNow[r];
                    }
                }
            }
            else
            {
                array = new double[1, 1];
                array[0, 0] = 1;
            }


            return new Matrix(array);
        }


        public void Change(INode a, INode b, double value)
        {
            int x = a.OrderIndexGroup();
            int y = b.OrderIndexGroup();
            Change(x, y, value);
        }
        public void Change(int x, int y, double value)
        {
            Array[x, y] = value;
            Array[y, x] = 1 / value;
        }
    }
}
