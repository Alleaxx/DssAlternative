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
            double[] nums = new double[Rows];
            for (int i = 0; i < Rows; i++)
            {
                nums[i] = Array[i, 0];
            }
            return nums;
        }
    }

}
