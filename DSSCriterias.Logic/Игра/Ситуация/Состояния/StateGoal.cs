using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSCriterias.Logic.States;

namespace DSSCriterias.Logic
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

}

namespace DSSCriterias.Logic.States
{
    public class GoalMinRisc : StateGoal
    {
        public GoalMinRisc() : base(Goals.MinRisc)
        {
            Name = "Минимальный риск";
            AddCompare(Goals.MinRisc, 2, "Идеален для минимального риска");
            AddCompare(Goals.MaxProfit, -1, "Не слишком подходит для максимального выигрыша");
            AddCompare(Goals.RiscAllowed, 1, "Частично подходит для некоторого риска");
        }
    }
    public class GoalMaxProfit : StateGoal
    {
        public GoalMaxProfit() : base(Goals.MaxProfit)
        {
            Name = "Максимальный выигрыш";
            AddCompare(Goals.MaxProfit, 2, "Идеален для максимального выигрыша");
            AddCompare(Goals.MinRisc, -1, "Не слишком подходит для минимального риска");
            AddCompare(Goals.RiscAllowed, 1, "Частично подходит для некоторого риска");
        }
    }
    public class GoalAny : StateGoal
    {
        public GoalAny() : base(Goals.RiscAllowed)
        {
            Name = "Некоторый риск";
            AddCompare(Goals.RiscAllowed, 2, "Идеален для некоторого риска");
            AddCompare(Goals.MaxProfit, 1, "Частично подходит для максимального выигрыша");
            AddCompare(Goals.MinRisc, 0, "Не совсем подходит для минимального риска");
        }
    }
}