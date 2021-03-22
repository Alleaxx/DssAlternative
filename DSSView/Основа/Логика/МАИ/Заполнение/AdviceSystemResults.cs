using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class AdviceSystemResults : NotifyObj
    {

        private Problem Problem { get; set; }
        public List<NodeLevelResults> Results
        {
            get
            {
                var results = new List<NodeLevelResults>();
                foreach (var item in Problem.Dictionary)
                {
                    results.Add(new NodeLevelResults(item.Key, item.Value));
                }
                return results;
            }
        }

        public void SetProblem(Problem problem)
        {
            if (Problem != null)
                Problem.CoefficientUpdated -= Problem_CoefficientUpdated;

            Problem = problem;
            OnPropertyChanged(nameof(Results));
            Problem.CoefficientUpdated += Problem_CoefficientUpdated;
        }

        private void Problem_CoefficientUpdated(Node obj)
        {
            OnPropertyChanged(nameof(Results));
        }
    }

    public class NodeLevelResults
    {
        public int Level { get; set; }
        public NodeRating[] Nodes { get; set; }

        public NodeRating Best => Nodes.First();

        public NodeLevelResults(int level, IEnumerable<Node> nodes)
        {
            Level = level;
            Nodes = nodes.Select(n => new NodeRating(nodes.ToArray(), n)).OrderBy(n => n.Rating).Reverse().ToArray();
        }
    }

    public class NodeRating : NotifyObj
    {
        public event Action<NodeRating> RatingChanged;
        public Node Node { get; set; }

        public int Rating
        {
            get => rating;
            set
            {
                rating = value;
                OnPropertyChanged();
                RatingChanged?.Invoke(this);
            }
        }
        private int rating;

        public NodeRating(Node node, int val)
        {
            Node = node;
            rating = val;
        }
        public NodeRating(Node[] nodes, Node node)
        {
            Node = node;
            Rating = (int)Math.Round(node.Coefficient * 100);
        }
    }
}
