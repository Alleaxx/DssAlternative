using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface IProblemNode : IStyled
    {
        INode Node { get; }
        string Href { get; }
    }

    public class ProblemNode : IProblemNode
    {
        public INode Node { get; set; }
        private IProblem Problem { get; set; }

        public string Href => $"node/{Node.Level}/{Node.Name}";

        public ProblemNode(IProblem problem, INode node)
        {
            Problem = problem;
            Node = node;
        }

        public string GetClass() => "";

        public string GetStyle() => "";
    }
}
