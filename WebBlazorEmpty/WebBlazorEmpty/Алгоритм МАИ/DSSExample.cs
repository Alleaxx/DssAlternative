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
            INode main = new Node(0, "Выбор места учебы", 0, -1);

            INode Place = new Node(1, "Местоположение", 1, 0);

            INode Reputation = new Node(1, "Репутация", 2, 0);

            INode A = new Node(2, "Вариант А", 3, 1);
            INode B = new Node(2, "Вариант B", 3, 2);
            INode G = new Node(2, "Вариант G", 3, 1);
            INode H = new Node(2, "Вариант H", 3, 2);

            nodes.AddRange(new INode[] { main, Reputation, Place, A, B, G, H });

            ITemplate template = new Template(nodes.OfType<Node>().ToArray());
            IProject project = new Project(template);
            IProblem sampleProblem = project.Problem;

            sampleProblem.SetRelationBetween(main, Reputation, Place, 5);
            sampleProblem.SetRelationBetween(Reputation, A, B, 2);
            sampleProblem.SetRelationBetween(Place, B, A, 2);

            return project;
        }

    }
}
