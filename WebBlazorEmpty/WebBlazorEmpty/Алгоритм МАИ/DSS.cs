using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

using Microsoft.AspNetCore.Components;

namespace WebBlazorEmpty.AHP
{
    public class DSS
    {
        private static IProject CreateSampleProblem()
        {

            List<INode> nodes = new List<INode>();
            INode main = new Node(0, "Выбор места учебы");

            INode Place = new Node(1, "Местоположение");
            INode Reputation = new Node(1, "Репутация");

            INode A = new Node(2, "Универ А");
            INode B = new Node(2, "Универ B");
            INode C = new Node(2, "Универ C");

            INode F1 = new Node(3, "Факультет ПИ");
            INode F2 = new Node(3, "Факультет БИ");

            nodes.AddRange(new INode[] { main ,Reputation, Place, A, B, C, F1, F2 });

            IProject project = new Project(nodes);
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
        static DSS()
        {
            Ex = new DSS();
        }


        public static DSS Ex { get; private set; }

        public event Action<IProject> ProjectChanged;

        public List<IHierarchy> Templates { get; set; }
        //Все проблемы
        public List<IProject> Problems { get; private set; } = new List<IProject>();
        public IProject Find(string name)
        {
            return Problems.Find(p => p.Problem.MainGoal.Name == name);
        }

        //Выбранная проблема
        public IProject Project { get; private set; }
        public IProblem Problem => Project.Problem;


        public void SelectProblem(IProject project)
        {
            Project = project;
            ProjectChanged?.Invoke(project);
        }

        public void AddProblem(IEnumerable<INode> nodes)
        {
            Problems.Add(new Project(nodes));
        }
        public void AddSample()
        {
            Problems.Add(CreateSampleProblem());
        }
    }
}
