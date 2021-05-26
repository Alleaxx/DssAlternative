using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBlazorEmpty.AHP;

namespace WebBlazorEmpty
{
    public class RelationSystem
    {
        protected ProblemDecizion Hierarchy { get; set; }

        public RelationSystem(ProblemDecizion main)
        {
            Hierarchy = main;
        }
    }

    public class RelationMatrixSystem : RelationSystem
    {


        public RelationMatrixSystem(ProblemDecizion main) : base(main)
        {

        }
    }
    

    //Опрос
    public class RelationQuestionSystem : RelationSystem
    {
        public RelationQuestionSystem(ProblemDecizion main) : base(main)
        {

        }
    }



    //Выставление рейтинга
    //public class RelationRatingSystem : RelationSystem
    //{
    //    public NodesCriteriaRating[] NodeRatingList { get; set; }
    //    public int MaxRating { get; set; } = 9;

    //    public RelationRatingSystem(ProblemDecizion main) : base(main)
    //    {
    //        NodeRatingList = main.HardNodes.Select(n => new NodesCriteriaRating(main,n)).ToArray();
    //    }
    //}
    //public class NodesCriteriaRating
    //{
    //    public override string ToString() => Node.ToString();

    //    public Problem Problem { get; set; }
    //    public INode Node { get; set; }
    //    public INode[] NodesNext { get; set; }

    //    public NodesCriteriaRating(Problem problem,INode node)
    //    {
    //        Problem = problem;
    //        Node = node;
    //        NodesNext = problem.CriteriasFurther[node].ToArray();
    //        for (int i = 0; i < NodesNext.Length; i++)
    //        {
    //            NodesNext[i].Rating = 1;
    //        }
    //    }

    //    public void ChangeRating(INode r, int newRating)
    //    {
    //        r.Rating = newRating;
    //        UpdateAllRatings();
            
    //    }
    //    private void UpdateAllRatings()
    //    {
    //        double min = NodesNext.Select(n => n.Rating).Min();
    //        Dictionary<INode, double> newRates = new Dictionary<INode, double>();
    //        foreach (var node in NodesNext)
    //        {
    //            newRates.Add(node, min / node.Rating);
    //        }
    //        for (int i = 0; i < NodesNext.Length; i++)
    //        {
    //            for (int a = i + 1; a < NodesNext.Length; a++)
    //            {
    //                INode first = NodesNext[i];
    //                INode sec = NodesNext[a];
    //                Problem.SetRelationBetween(Node, NodesNext[i], NodesNext[a], newRates[sec] / newRates[first]);
    //            }
    //        }
    //        Console.WriteLine("Все рейтинги обновлены");
    //    }
    //}





    public class Rating
    {
        public string Name { get; private set; }
        public int Value { get; private set; }
        public string Style { get; set; }

        public Rating(int val)
        {
            Value = val;
            switch (val)
            {
                case 1:
                    Name = "Одинаковы по значимости";
                    Style = "color:Black;font-weight:bold";
                    break;
                case 3:
                    Name = "Немного важнее";
                    Style = "color:green;font-weight:bold";
                    break;
                case 5:
                    Name = "Важнее";
                    Style = "color:blue;font-weight:bold";
                    break;
                case 7:
                    Name = "Значительно важнее";
                    Style = "color:violet;font-weight:bold";
                    break;
                case 9:
                    Name = "Абсолютно важнее";
                    Style = "color:orange;font-weight:bold";
                    break;
            }
        }
    }
}
