using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    static class DSSExample
    {
        public static IProject CreateSampleProblem()
        {
            List<INode> nodes = new List<INode>();
            INode main = new Node(0, "Выбор места учебы",0,-1);

            INode Place = new Node(1, "Местоположение", 1, 0);
            INode Reputation = new Node(1, "Репутация", 1, 0);

            INode A = new Node(2, "Вариант А", 2, 1);
            INode B = new Node(2, "Вариант B", 2, 1);
            INode C = new Node(2, "Вариант C", 2, 1);

            INode F1 = new Node(3, "Факультет ПИ", 3, 2);
            INode F2 = new Node(3, "Факультет БИ", 3, 2);

            nodes.AddRange(new INode[] { main, Reputation, Place, A, B, C, F1, F2 });

            ITemplate template = new Template(nodes.OfType<Node>().ToArray());
            IProject project = new Project(template);
            IProblem sampleProblem = project.Problem;

            sampleProblem.SetRelationBetween(main, Reputation, Place, 5);
            sampleProblem.SetRelationBetween(Reputation, A, B, 2);
            sampleProblem.SetRelationBetween(Reputation, A, C, 3);
            sampleProblem.SetRelationBetween(Reputation, B, C, 1.5);
            sampleProblem.SetRelationBetween(Place, B, A, 2);
            sampleProblem.SetRelationBetween(Place, C, A, 5);
            sampleProblem.SetRelationBetween(Place, C, B, 2);
            sampleProblem.SetRelationBetween(A, F1, F2, 2);
            sampleProblem.SetRelationBetween(B, F1, F2, 4);
            sampleProblem.SetRelationBetween(C, F1, F2, 6);

            return project;
        }
        public static IProject CreateSampleTreeProblem()
        {
            List<INode> nodes = new List<INode>();
            INode main = new Node(0, "Выбор места учебы",        0, -1);

            INode Place = new Node(1, "Местоположение",          1, 0);

            INode Reputation = new Node(1, "Репутация",          2, 0);
            INode PlaceDistance = new Node(2, "Расстояние",      2, 1);
            INode PlaceRoute = new Node(2, "Маршрут",            2, 1);

            INode A = new Node(3, "Вариант А",                   3, 2);
            INode B = new Node(3, "Вариант B",                   3, 2);

            nodes.AddRange(new INode[] { main, Reputation, Place, PlaceDistance, PlaceRoute, A, B });

            ITemplate template = new Template(nodes.OfType<Node>().ToArray());
            IProject project = new Project(template);
            IProblem sampleProblem = project.Problem;

            sampleProblem.SetRelationBetween(main, Reputation, Place, 5);
            sampleProblem.SetRelationBetween(Reputation, A, B, 2);
            sampleProblem.SetRelationBetween(Place, B, A, 2);

            return project;
        }
        public static IProject CreateSampleTree2Problem()
        {
            List<INode> nodes = new List<INode>();
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


            nodes.AddRange(new INode[] { main, F, M, O, F1, F2, F3, M1, M2, M3, O1, O2, O3 });

            ITemplate template = new Template(nodes.OfType<Node>().ToArray());
            IProject project = new Project(template);
            IProblem sampleProblem = project.Problem;

            sampleProblem.SetRelationBetween(main, F, M, 2);
            sampleProblem.SetRelationBetween(main, F, O, 2);
            sampleProblem.SetRelationBetween(main, M, O, 2);

            sampleProblem.SetRelationBetween(F, F1, F2, 2);
            sampleProblem.SetRelationBetween(F, F1, F3, 4);
            sampleProblem.SetRelationBetween(F, F2, F3, 2);

            sampleProblem.SetRelationBetween(M, M1, M2, 5);
            sampleProblem.SetRelationBetween(M, M1, M3, 9);
            sampleProblem.SetRelationBetween(M, M2, M3, 2);

            sampleProblem.SetRelationBetween(O, O1, O2, 4);
            sampleProblem.SetRelationBetween(O, O1, O3, 8);
            sampleProblem.SetRelationBetween(O, O2, O3, 2);

            return project;
        }

    }
}
