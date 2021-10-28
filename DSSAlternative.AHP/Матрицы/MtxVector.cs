using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{

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
            double[] nums = new double[RSize];
            for (int i = 0; i < RSize; i++)
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
            double[] nums = new double[RSize];
            for (int i = 0; i < RSize; i++)
            {
                nums[i] = Array[i, 0];
            }
            return nums;
        }
        public MtxGlobalCoeffs(INode node)
        {
            Nodes = node.Criterias.GroupHier.ToArray();
            if (Nodes.Length > 0)
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
            Nodes = node.NeighborsGroup;
            var local = new MtxLocalCoeffs(problem, node);
            var global = new MtxGlobalCoeffs(node);
            var res = MtxActions.MatrixMultiplication(local.Array, global.GetArr());

            Array = new double[Nodes.Length, 1];
            for (int i = 0; i < Nodes.Length; i++)
            {
                Array[i, 0] = res[i];
            }
        }
    }
}
