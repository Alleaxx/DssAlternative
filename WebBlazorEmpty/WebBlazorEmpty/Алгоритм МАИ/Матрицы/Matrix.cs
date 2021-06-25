using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface IMatrix
    {
        int Size { get; }
        double[,] Array { get; }
        double[] Coeffiients { get; }
        IConsistency Consistency { get; }
        bool WithZeros();
    }
    public class Matrix : IMatrix 
    {
        public override string ToString() => $"Матрица ({Array.GetLength(0)}x{Array.GetLength(1)})";
        
        public double[,] Array { get; set; }
        public int Size => Array.GetLength(0);

        public IConsistency Consistency { get; protected set; }

        public double[] Coeffiients => MtxActions.LocalCoefficients(Array);

        public bool WithZeros()
        {
            for (int x = 0; x < Array.GetLength(0); x++)
            {
                for (int y = 0; y < Array.GetLength(1); y++)
                {
                    if (Array[x, y] == 0)
                        return true;
                }
            }
            return false;
        }


        protected Matrix()
        {

        }
        public Matrix(double[,] arr)
        {
            Array = new double[arr.GetLength(0), arr.GetLength(1)];
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int a = 0; a < arr.GetLength(1); a++)
                {
                    Array[i, a] = arr[i,a];
                }
            }
            Consistency = new MatrixConsistenct(this);
        }


        public virtual string GetText()
        {
            string text = "";
            for (int x = 0; x < Array.GetLength(0); x++)
            {
                text += "\n";
                for (int y = 0; y < Array.GetLength(1); y++)
                {
                    text += $"{Math.Round(Array[x, y], 3),-7}";
                }
            }
            return text;
        }
    }



    public interface IMatrixRelations : IMatrix
    {
        INode Main { get; set; }
        INode[] Nodes { get; set; }

    }


    //Матрица отношений для узла (сравнение нижестоящих узлов)
    public class MtxRelations : Matrix, IMatrixRelations
    {
        public INode Main { get; set; }
        public INode[] Nodes { get; set; }


        public MtxRelations()
        {

        }
        public MtxRelations(IProblem problem, INode node)
        {
            Main = node;
            Nodes = problem.Hierarchy.Where(n => n.Contents.Contains(node)).ToArray();
            var rels = problem.RelationsAll.Where(r => r.Main == Main).GroupBy(r => r.From);
            int length = rels.Count();

            Array = new double[length,length];
            for (int i = 0; i < length; i++)
            {
                var row = rels.ElementAt(i);
                for (int a = 0; a < length; a++)
                {
                    Array[i, a] = row.ElementAt(a).Value;
                }
            }

            Consistency = new MatrixConsistenct(this);
        }
    }

    //Матрица -1 локальных коэффициентов для узла
    public class MtxLocalCoeffs : MtxRelations, IMatrixRelations
    {
        public MtxLocalCoeffs(IProblem problem, INode node)
        {
            Nodes = node.Criterias.Group;
            if(Nodes.Length > 0)
            {
                var coeffs = Nodes.Select(n => new MtxRelations(problem, n).Coeffiients);
                int rows = coeffs.First().Length;
                int cols = coeffs.Count();

                Array = new double[rows, cols];
                for (int c = 0; c < cols; c++)
                {
                    var coeffsNow = coeffs.ElementAt(c);
                    for (int r = 0; r < rows; r++)
                    {
                        Array[r, c] = coeffsNow[r];
                    }
                }
            }
            else
            {
                Array = new double[1, 1];
                Array[0, 0] = 1;
            }
        }
    }



    interface IVectorNode : IMatrixRelations
    {
        double Get(INode node);
        double[] GetVector();
    }
    public class MtxVector : MtxRelations, IVectorNode
    {
        public double Get(INode node) => Array[Nodes.ToList().IndexOf(node), 0];

        public double[] GetVector()
        {
            double[] nums = new double[Size];
            for (int i = 0; i < Size; i++)
            {
                nums[i] = Array[i, 0];
            }
            return nums;
        }
    }

    //Вектор -1 глобальных коэффициентов для узла
    public class MtxGlobalCoeffs : MtxVector
    {
        public double[] GetArr()
        {
            double[] nums = new double[Size];
            for (int i = 0; i < Size; i++)
            {
                nums[i] = Array[i, 0];
            }
            return nums;
        }
        public MtxGlobalCoeffs(IProblem problem, INode node)
        {
            Nodes = node.Contents.ToArray();
            if(Nodes.Length > 0)
            {
                Array = new double[Nodes.Length, 1];
                for (int i = 0; i < Nodes.Length; i++)
                {
                    Array[i, 0] = Nodes[i].Coefficient;
                }
            }
            else
            {
                Array = new double[1, 1];
                Array[0, 0] = 1;
            }
        }

    }
    //Вектор коэффициентов для узла
    public class MtxCoeffs : MtxVector
    {
        public MtxCoeffs(IProblem problem, INode node)
        {
            Nodes = node.NeighborsComp;
            var local = new MtxLocalCoeffs(problem, node);
            var global = new MtxGlobalCoeffs(problem, node);
            var res = MtxActions.MatrixMultiplication(local.Array, global.GetArr());
            Console.WriteLine(node);
            res.ToList().ForEach(r => Console.Write(r + "  "));
            Console.WriteLine($"\nSize [{Nodes.Length},{1}]");
            Array = new double[Nodes.Length, 1];
            for (int i = 0; i < Nodes.Length; i++)
            {
                Array[i, 0] = res[i];
            }
        }
    }
}
