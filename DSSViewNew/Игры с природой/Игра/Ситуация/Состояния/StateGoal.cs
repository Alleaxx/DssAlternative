using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSView.States;

namespace DSSView
{
    public enum Goals
    {
        MinRisc, MaxProfit, RiscAllowed
    }

    public abstract class StateGoal : State<Goals>
    {
        private static readonly Dictionary<Goals, StateGoal> Values = new Dictionary<Goals, StateGoal>()
        {
            [Goals.MaxProfit]   = new GoalMaxProfit(),
            [Goals.RiscAllowed] = new GoalAny(),
            [Goals.MinRisc]     = new GoalMinRisc(),
        };
        public static StateGoal Get(Goals chance)
        {
            return Values[chance];
        }

        protected StateGoal(Goals riscs) : base(riscs)
        {

        }
    }

    namespace States
    {
        public class GoalMinRisc : StateGoal
        {
            public GoalMinRisc() : base(Goals.MinRisc)
            {
                Name = "Минимальный риск";
            }
        }
        public class GoalMaxProfit : StateGoal
        {
            public GoalMaxProfit() : base(Goals.MaxProfit)
            {
                Name = "Максимальный выигрыш";
            }
        }
        public class GoalAny : StateGoal
        {
            public GoalAny() : base(Goals.RiscAllowed)
            {
                Name = "Некоторый риск";
            }
        }
    }
}
