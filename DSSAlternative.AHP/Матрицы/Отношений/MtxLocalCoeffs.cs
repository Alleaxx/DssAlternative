using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{

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
