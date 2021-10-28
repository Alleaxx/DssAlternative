using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    //Вектор коэффициентов для узла
    public class MtxCoeffs : MtxVector
    {
        public MtxCoeffs(IProblem problem, INode node)
        {
            Nodes = node.NeighborsGroup;
            var local = new MtxLocalCoeffs(problem, node);
            var global = new MtxGlobalCoeffs(node);
            var res = local.Array.Multiply(global.GetArr()); //MtxActions.MatrixMultiplication(local.Array, global.GetArr());

            Array = new double[Nodes.Length, 1];
            for (int i = 0; i < Nodes.Length; i++)
            {
                Array[i, 0] = res[i];
            }
        }
    }
}
