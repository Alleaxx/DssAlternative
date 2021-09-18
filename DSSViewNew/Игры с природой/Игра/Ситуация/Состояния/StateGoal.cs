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
        MinRisc, MaxProfit, Any
    }

    public abstract class StateGoal : State<Goals>
    {
        public static StateGoal Get(Goals riscs)
        {
            switch (riscs)
            {
                case Goals.MaxProfit:
                    return new GoalMaxProfit();
                case Goals.MinRisc:
                    return new GoalMinRisc();
                case Goals.Any:
                    return new GoalAny();
                default:
                    throw new Exception("Такой цели нет");
            }
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
            public GoalAny() : base(Goals.Any)
            {
                Name = "Всё равно";
            }
        }
    }
}
