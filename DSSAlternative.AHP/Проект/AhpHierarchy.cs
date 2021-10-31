using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public static class AhpHierarchy
    {
        public static IProject CreateNewProblem()
        {
            INode main = new Node("Выбор");
            INode k1 = new Node(1, "К1", 1, 0);
            INode k2 = new Node(1, "К2", 1, 0);
            INode a1 = new Node(2, "А1", 2, 1);
            INode a2 = new Node(2, "А2", 2, 1);

            return new Project(new INode[] { main, k1, k2, a1, a2 });
        }
    }
}
