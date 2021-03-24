using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class SingleChoiceSystemRelations : NotifyObj, IAdviceSystem
    {
        public override string ToString() => "Система рейтинга критериев";

        private Problem Problem { get; set; }    
        public CriteriaChoice[] Criterias { get; set; }

        public CriteriaChoice Selected
        {
            get => selected;
            set
            {
                selected = value;
                OnPropertyChanged();
            }
        }
        private CriteriaChoice selected;


        public void SetProblem(Problem problem)
        {
            if(problem != Problem)
            {
                Problem = problem;
                Criterias = Problem.AllCriterias.Where(cr => cr.Inner.Count > 0).Select(cr => new CriteriaChoice(cr)).ToArray();
            }
        }
        public void ClearProblem()
        {
            SetProblem(new Problem());
        }

    }
    public class CriteriaChoice
    {
        public override string ToString() => Node.ToString();

        //Методика 1:
        //Распределяем внутренние критерии по рейтингу от лучшего к худшему
        //Спрашиваем про разницу между соседями

        //Методика 2:
        //Спрашиваем худший - лучший - средний
        //Спрашиваем про разницу между соседями

        public Node Node { get; set; }
        public NodeRating[] NodeRatings { get; set; }

        public CriteriaChoice(Node node)
        {
            Node = node;
            NodeRatings = node.Inner.Select(cr => new NodeRating(cr,1)).ToArray();
            for (int i = 0; i < NodeRatings.Length; i++)
            {
                NodeRatings[i].Rating = 1;
                NodeRatings[i].RatingChanged += CriteriaChoice_RatingChanged;
            }

        }

        private void CriteriaChoice_RatingChanged(NodeRating obj)
        {
            double min = NodeRatings.Select(n => n.Rating).Min();
            Dictionary<Node, double> newRates = new Dictionary<Node, double>();
            foreach (var item in NodeRatings)
            {
                newRates.Add(item.Node, min / item.Rating);
            }
            for (int i = 0; i < NodeRatings.Length; i++)
            {
                for (int a = i + 1; a < NodeRatings.Length; a++)
                {
                    NodeRating first = NodeRatings[i];
                    NodeRating sec = NodeRatings[a];
                    Node.SetRelationBetween(Node, NodeRatings[i].Node, NodeRatings[a].Node, newRates[sec.Node] / newRates[first.Node]);
                }
            }
        }
    }


}
