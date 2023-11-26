using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP.HierarchyInfo
{
    /// <summary>
    /// Предустановленные иерархии и задачи
    /// </summary>
    public static class HierarchyExamples
    {
        //Готовые задачи
        public static IProject CreateNewProblem()
        {
            INode main = new Node(0, "Выбор", "Цель", "Нет");
            INode k1 = new Node(1, "К1", "Критерии", "Цель");
            INode k2 = new Node(1, "К2", "Критерии", "Цель");
            INode a1 = new Node(2, "А1", "Альтернативы", "Критерии");
            INode a2 = new Node(2, "А2", "Альтернативы", "Критерии");

            return new Project(new INode[] { main, k1, k2, a1, a2 });
        }
        public static IProject CreateEmptyProblem()
        {
            INode main = new Node(0, "Выбор", "Цель", "Нет");
            INode a1 = new Node(1, "А1", "Альтернативы", "Цель");
            INode a2 = new Node(1, "А2", "Альтернативы", "Цель");

            return new Project(new INode[] { main, a1, a2 }, true);
        }

        //Готовые иерархии
        public static IHierarchy CreateEmptyHierarchy()
        {
            INode main = new Node(0, "???", "Цель", "Нет");

            return new HierarchyNodesList(new INode[] { main });
        }
    }
}
