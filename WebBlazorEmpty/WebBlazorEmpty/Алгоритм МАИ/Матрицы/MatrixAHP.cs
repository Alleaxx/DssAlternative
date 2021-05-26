using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBlazorEmpty.AHP
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


        public double[] Coeffiients => LocalCoefficients(Array);


        public static double[] GeometricMultiVector(double[,] mtx)
        {
            int size = mtx.GetLength(0);

            double[] vector = new double[size];
            for (int r = 0; r < size; r++)
            {
                double multi = 1;
                for (int c = 0; c < size; c++)
                {
                    multi *= mtx[r,c];
                }
                vector[r] = multi;

            }
            return vector;
        }
        public static double[] MatrixMultiplication(double[,] mtx, double[] vector)
        {
            int rsize = mtx.GetLength(0);
            int size = vector.Length;
            double[] results = new double[rsize];
            for (int r = 0; r < rsize; r++)
            {
                double res = 0;
                for (int c = 0; c < size; c++)
                {
                    res += mtx[r, c] * vector[c];
                }
                results[r] = res;
            }
            return results;
        }
        public static double[] Normalise(double[] vector, double? max = null)
        {
            int size = vector.Length;
            max = max ?? size;

            var results = new double[size];
            vector.CopyTo(results,0);
            for (int i = 0; i < size; i++)
            {
                results[i] = vector[i] / max.Value;
            }
            return results;
        }

        public static double[] LocalCoefficients(double[,] mtx)
        {
            int size = mtx.GetLength(0);
            var geometricMulti = GeometricMultiVector(mtx);

            var multiPowed = new double[size];
            for (int i = 0; i < size; i++)
            {
                multiPowed[i] = Math.Pow(geometricMulti[i], 1 / (double)size);
            }
            var coeffs = Normalise(multiPowed,multiPowed.Sum());

            return coeffs;
        }


        public static double[,] GetRelationMatrixForNode(Problem problem, INode node)
        {
            var grouped = problem.RelationsAll.Where(g => g.Main == node).GroupBy(r => r.From).ToArray();
            return GetArrayFromRelations(grouped);
        }
        private static double[,] GetArrayFromRelations(IGrouping<INode, INodeRelation>[] grouped)
        {
            int size = grouped.Length;
            var arr = new double[size, size];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    arr[x, y] = grouped[x].ElementAt(y).Value;
                }
            }
            return arr;
        }
        public static double[,] GetCoeffMatrixForLevel(Problem problem, int level)
        {
            var nodes = problem.Dictionary[level];
            var nodeCoeffs = nodes.Select(n => LocalCoefficients(GetRelationMatrixForNode(problem, n))).ToArray();

            int rowSize = nodes.Length;           //Кол-во столбцов, соответствует количесту узлов
            int colSize = nodeCoeffs.First().Length;   //Кол-во строк, соответствует длине вектора коэффициентов этого уровня

            var mtx = new double[colSize, rowSize];
            for (int r = 0; r < colSize; r++)
            {
                for (int c = 0; c < rowSize; c++)
                {
                    //Console.WriteLine($"[{r}][{c}] = {nodeCoeffs[c][r]}");
                    mtx[r, c] = nodeCoeffs[c][r];
                }
            }

            return mtx;
        }
        public static double[] GetGlobalCoeffs(Problem problem, int level)
        {
            var localCoeffMatrix = GetCoeffMatrixForLevel(problem, level);
            var globalCoeffMatrix = problem.Hierarchy.Where(node => node.Level == level).Select(n => n.Coefficient).ToArray();

            return MatrixMultiplication(localCoeffMatrix, globalCoeffMatrix);
        }


        public MatrixAHP(IGrouping<INode, INodeRelation>[] grouped) : this(GetArrayFromRelations(grouped))
        {

        }
        public MatrixAHP(double[,] arr)
        {
            Array = arr;
            Consistency = new MatrixConsistenct(this);
        }
    }
}
