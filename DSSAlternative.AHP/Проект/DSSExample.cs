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

            ITemplate template = new Template(nodes.OfType<Node>().ToArray());
            IProject project = new Project(template);
            IProblem problem = project.ProblemActive;

            problem.SetRelationBetween(main, Reputation, Place, 5);
            problem.SetRelationBetween(Reputation, A, B, 2);
            problem.SetRelationBetween(Reputation, A, C, 3);
            problem.SetRelationBetween(Reputation, B, C, 1.5);
            problem.SetRelationBetween(Place, B, A, 2);
            problem.SetRelationBetween(Place, C, A, 5);
            problem.SetRelationBetween(Place, C, B, 2);
            problem.SetRelationBetween(A, F1, F2, 2);
            problem.SetRelationBetween(B, F1, F2, 4);
            problem.SetRelationBetween(C, F1, F2, 6);

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

            ITemplate template = new Template(nodes);
            IProject project = new Project(template, true);
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

            ITemplate template = new Template(nodes.OfType<Node>().ToArray());
            IProject project = new Project(template);
            IProblem problem = project.ProblemActive;

            problem.SetRelationBetween(main, F, M, 2);
            problem.SetRelationBetween(main, F, O, 2);
            problem.SetRelationBetween(main, M, O, 2);

            problem.SetRelationBetween(F, F1, F2, 2);
            problem.SetRelationBetween(F, F1, F3, 4);
            problem.SetRelationBetween(F, F2, F3, 2);

            problem.SetRelationBetween(M, M1, M2, 5);
            problem.SetRelationBetween(M, M1, M3, 9);
            problem.SetRelationBetween(M, M2, M3, 2);

            problem.SetRelationBetween(O, O1, O2, 4);
            problem.SetRelationBetween(O, O1, O3, 8);
            problem.SetRelationBetween(O, O2, O3, 2);

            return project;
        }

    }
}
