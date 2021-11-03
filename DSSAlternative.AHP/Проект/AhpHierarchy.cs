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
            INode main = new Node(0, "Выбор", "Цель", "Нет");
            INode k1 = new Node(1, "К1", "Критерии", "Цель");
            INode k2 = new Node(1, "К2", "Критерии", "Цель");
            INode a1 = new Node(2, "А1", "Альтернативы", "Критерии");
            INode a2 = new Node(2, "А2", "Альтернативы", "Критерии");

            return new Project(new INode[] { main, k1, k2, a1, a2 });
        }
    }
}
