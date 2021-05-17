using System;
using System.Collections.Generic;
using System.Text;

namespace DSSLib
{
    public class DecisionTree :  Decision
    {
        public override string ToString() => $"Составление дерева решений";


        public Dictionary<Alternative,double> AltWay { get; set; }
        public Dictionary<TreeCase,double> AllWays { get; set; }
        
        
        //Проверить на наличие у альтернатив всех указанных критериев в выборе
        public static DecisionCheckResult CheckAll(Problem problem)
        {
            //DecisionCheckResult check = CheckBasic(problem);
            //if (!check.Success)
            //    return check;
            return new DecisionCheckResult() { Success = true };
        }
        public static bool IsSolvable(Problem problem) => CheckAll(problem).Success;



        public DecisionTree(Problem problem) : base(problem)
        {
            AltWay = new Dictionary<Alternative, double>();
            Solve();
        }
        protected override void Solve()
        {
            foreach (Alternative alternative in Problem.Alternatives)
            {
                SolveAlternative(null,alternative);
            }
        }
        private void SolveAlternative(TreeCase way,Alternative alternative)
        {
            double mX = 0;


            //way.AlterWay.Add(alternative);
            foreach (Case caseC in alternative.AcceptCases)
            {
                if(caseC.AvailAlts.Count == 0)
                    mX += caseC.Benefit * caseC.Chance;

            }
            AltWay.Add(alternative, mX);
        }
        
        
        public override void Output()
        {
            Console.WriteLine(ToString());
            Console.WriteLine("Решения");
            foreach (var item in AltWay)
            {
                Console.WriteLine($"{item.Key.Name}: {item.Value}");
            }
            Console.WriteLine("Все исходы");
            foreach (var item in AllWays)
            {
                Console.WriteLine($"{item.Key}: {item.Value}");
            }
        }
    }
    public class TreeCase
    {
        public List<IId> AlterWay { get; set; }
    }

}
