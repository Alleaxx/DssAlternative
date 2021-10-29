using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public class VectorMtx
    {
        public override string ToString()
        {
            return $"[{string.Join(';', Vector)}] {string.Join<INode>(',', Nodes)}";
        }

        private readonly INode[] Nodes;
        private readonly double[] Vector;
        private readonly Dictionary<INode, double> Dictionary;

        public double this[INode node]
        {
            get { return Dictionary[node]; }
        }

       
        private VectorMtx(INode[] nodes, double[] vector)
        {
            Nodes = nodes;
            Vector = vector;
            Dictionary = new Dictionary<INode, double>();
            for (int i = 0; i < nodes.Length; i++)
            {
                Dictionary.Add(nodes[i], vector[i]);
            }
        }

        //Вектор -1 глобальных коэффициентов для узла
        public static VectorMtx CreateGlobalCoeffs(INode node)
        {
            INode[] nodes = node.Criterias().ToArray();
            double[] vector = new double[] { 1 };
            if (nodes.Length > 0)
            {
                vector = nodes.Select(n => n.Coefficient).ToArray();
            }

            return new VectorMtx(nodes, vector);
        }

        //Вектор коэффициентов для узла (на одном уровне)
        public static VectorMtx CreateCoeffs(IRelationsGrouped problem, INode node)
        {
            var nodes = node.NeighborsGroupIndex().ToArray();

            var local = Matrix.CreateLocalCoeffs(problem, node);
            var global = CreateGlobalCoeffs(node);
            var res = local.Array.Multiply(global.Vector);

            return new VectorMtx(nodes, res);
        }
    }
}
