using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    //Вектор -1 глобальных коэффициентов для узла
    public class MtxGlobalCoeffs : MtxVector
    {
        public double[] GetArr()
        {
            double[] nums = new double[Rows];
            for (int i = 0; i < Rows; i++)
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
}
