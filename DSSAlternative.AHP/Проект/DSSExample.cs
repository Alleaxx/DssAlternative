using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSSAlternative.AHP;

namespace DSSAlternative.AHP
{
    public static class DSSExample
    {
        public static IProject CreateSampleProblem()
        {
            INode main = new Node(0, "Выбор места учебы", 0, -1);

            INode Place = new Node(1, "Местоположение", 1, 0);
            INode Reputation = new Node(1, "Репутация", 1, 0);

            INode A = new Node(2, "Вариант А", 2, 1);
            INode B = new Node(2, "Вариант B", 2, 1);
            INode C = new Node(2, "Вариант C", 2, 1);

            INode F1 = new Node(3, "Факультет ПИ", 3, 2);
            INode F2 = new Node(3, "Факультет БИ", 3, 2);

            INode[] nodes = new INode[] { main, Reputation, Place, A, B, C, F1, F2 };

            IProject project = new Project(nodes);
            IRelations relations = project.Relations;

            //relations.Set(main, Reputation, Place, 5);
            //relations.Set(Reputation, A, B, 2);
            //relations.Set(Reputation, A, C, 3);
            //relations.Set(Reputation, B, C, 1.5);
            //relations.Set(Place, B, A, 2);
            //relations.Set(Place, C, A, 5);
            //relations.Set(Place, C, B, 2);
            //relations.Set(A, F1, F2, 2);
            //relations.Set(B, F1, F2, 4);
            //relations.Set(C, F1, F2, 6);

            return project;
        }
        public static IProject CreateSampleTreeProblem()
        {
            INode main = new Node(0, "Выбор места учебы",        0, -1);

            INode Place = new Node(1, "Местоположение",          1, 0);

            INode Reputation = new Node(1, "Репутация",          2, 0);
            INode PlaceDistance = new Node(2, "Расстояние",      2, 1);
            INode PlaceRoute = new Node(2, "Маршрут",            2, 1);

            INode A = new Node(3, "Вариант А",                   3, 2);
            INode B = new Node(3, "Вариант B",                   3, 2);

            INode[] nodes = new INode[] { main, Reputation, Place, PlaceDistance, PlaceRoute, A, B };

            ITemplate template = new Template(nodes.OfType<Node>().ToArray());
            IProject project = new Project(template);

            return project;
        }
        public static IProject CreateNewProblem()
        {
            Node main = new Node(0, "Задача выбора", 0, -1);
            Node k1 = new Node(1, "К1", 1, 0);
            Node k2 = new Node(1, "К2", 1, 0);
            Node a1 = new Node(2, "А1", 2, 1);
            Node a2 = new Node(2, "А2", 2, 1);

            Node[] nodes = new Node[] { main, k1, k2, a1, a2 };

            IProject project = new Project(nodes);
            return project;
        }
        public static IProject CreateSampleTree2Problem()
        {
            INode main = new Node(0, "Риски", 0, -1);

            INode F = new Node(1, "Ф", 1, 0);
            INode M = new Node(1, "М", 2, 0);
            INode O = new Node(1, "О", 3, 0);

            INode F1 = new Node(2, "Ф1", 4, 1);
            INode F2 = new Node(2, "Ф2", 4, 1);
            INode F3 = new Node(2, "Ф3", 4, 1);

            INode M1 = new Node(2, "М1", 4, 2);
            INode M2 = new Node(2, "М2", 4, 2);
            INode M3 = new Node(2, "М3", 4, 2);

            INode O1 = new Node(2, "О1", 4, 3);
            INode O2 = new Node(2, "О2", 4, 3);
            INode O3 = new Node(2, "О3", 4, 3);

            INode[] nodes = new INode[] { main, F, M, O, F1, F2, F3, M1, M2, M3, O1, O2, O3 };

            IProject project = new Project(nodes);
            IRelations relations = project.Relations;

            //relations.Set(main, F, M, 2);
            //relations.Set(main, F, O, 2);
            //relations.Set(main, M, O, 2);

            //relations.Set(F, F1, F2, 2);
            //relations.Set(F, F1, F3, 4);
            //relations.Set(F, F2, F3, 2);

            //relations.Set(M, M1, M2, 5);
            //relations.Set(M, M1, M3, 9);
            //relations.Set(M, M2, M3, 2);

            //relations.Set(O, O1, O2, 4);
            //relations.Set(O, O1, O3, 8);
            //relations.Set(O, O2, O3, 2);

            return project;
        }

    }
}
