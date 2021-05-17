using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class MatrixAHP : DSSLib.New.MatrixAHP
    {
        public MatrixAHP(IGrouping<Node, NodeRelation>[] grouped)
        {
            Array = new double[grouped.Length, grouped.Length];
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    Array[x,y] = grouped[x].ElementAt(y).Value; 
                }
            }
            Consistency = new DSSLib.New.MatrixConsistenct(this);
        }

        public MatrixAHP(double[,] arr) : base(arr)
        {

        }
    }
}
