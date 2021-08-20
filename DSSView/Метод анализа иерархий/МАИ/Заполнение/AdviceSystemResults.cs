using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class AdviceSystemResults : DSSLib.NotifyObj, IAdviceSystem
    {
        public override string ToString() => "Стандартная система оценки результатов";

        private Problem Problem { get; set; }
        public List<NodeLevelResults> Results
        {
            get
            {
                var results = new List<NodeLevelResults>();
                foreach (var item in Problem.Dictionary)
                {
                    if(item.Key != 0)
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
        public void ClearProblem()
        {
            if(Problem != null)
            {
                Problem.CoefficientUpdated -= Problem_CoefficientUpdated;
            }
            Problem = new Problem();
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

        private double[] Rating { get; set; }
        
        private double Dispersion
        {
            get
            {
                double sum = 0;
                double average = Rating.Average();
                for (int i = 0; i < Rating.Length; i++)
                {
                    sum += Math.Pow((average - Rating[i]), 2);
                }

                return sum / Rating.Length;

            }
        }
        public double StandartDeviation => Math.Sqrt(Dispersion);
        public double Variance => StandartDeviation / Rating.Average();



        public NodeRating Best => Nodes.First();
        public NodeRating[] LowPriorityNodes => Nodes.Where(n => n.Node.Coefficient < 0.15).ToArray();

        public NodeLevelResults(int level, IEnumerable<Node> nodes)
        {
            Level = level;
            Nodes = nodes.Select(n => new NodeRating(nodes.ToArray(), n)).OrderBy(n => n.Rating).Reverse().ToArray();
            Rating = nodes.OrderBy(n => n.Coefficient).Select(n => n.Coefficient).ToArray();
        }
    }

    public class NodeRating : DSSLib.NotifyObj
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
