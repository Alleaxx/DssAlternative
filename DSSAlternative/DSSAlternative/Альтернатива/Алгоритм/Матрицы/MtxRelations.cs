using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface IMatrixRelations : IMatrix
    {
        INode Main { get; }
        INode[] Nodes { get; }

        void Change(INode a, INode b, double rating);

    }


    //Матрица отношений для узла (сравнение нижестоящих узлов)
    public class MtxRelations : Matrix, IMatrixRelations
    {
        public INode Main { get; protected set; }
        public INode[] Nodes { get; protected set; }


        protected MtxRelations()
        {

        }
        public MtxRelations(IProblem problem, INode node)
        {
            Main = node;
            Nodes = node.LowerNodesControlled;

            var rels = problem.RelationsGroupedMain(node);
            int length = rels.Count();

            Array = new double[length, length];
            for (int i = 0; i < length; i++)
            {
                var row = rels.ElementAt(i);
                for (int a = 0; a < length; a++)
                {
                    Array[i, a] = row.ElementAt(a).Value;
                }
            }
        }

        public void Change(INode a, INode b, double value)
        {
            int x = a.OrderIndexGroup;
            int y = b.OrderIndexGroup;

            Array[x, y] = value;
            Array[y, x] = 1 / value;
        }
    }

    //Матрица -1 локальных коэффициентов для узла
    public class MtxLocalCoeffs : MtxRelations, IMatrixRelations
    {
        public MtxLocalCoeffs(IProblem problem, INode node)
        {
            Nodes = node.Criterias.GroupHier;

            if (Nodes.Length > 0)
            {
                var coeffs = Nodes.Select(n => new MtxRelations(problem, n).Coeffiients).ToArray();
                int rows = coeffs.First().Length;
                int cols = coeffs.Length;

                Array = new double[rows, cols];
                for (int c = 0; c < cols; c++)
                {
                    var coeffsNow = coeffs[c];
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

}
