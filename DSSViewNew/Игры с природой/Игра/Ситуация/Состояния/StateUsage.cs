using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSView.States;

namespace DSSView
{
    public enum Usages
    {
        OneTime, ManyTimes, Couple
    }

    public abstract class StateUsage : State<Usages>
    {
        public static StateUsage Get(Usages usages)
        {
            switch (usages)
            {
                case Usages.OneTime:
                    return new UsageOneTime();
                case Usages.ManyTimes:
                    return new UsageManyTimes();
                case Usages.Couple:
                    return new UsageCouple();
                default:
                    throw new Exception("Такого использования нет");
            }
        }

        protected StateUsage(Usages usage) : base(usage)
        {

        }
    }

    namespace States
    {
        public class UsageOneTime : StateUsage
        {
            public UsageOneTime() : base(Usages.OneTime)
            {
                Name = "Однократно";
            }
        }
        public class UsageManyTimes : StateUsage
        {
            public UsageManyTimes() : base(Usages.ManyTimes)
            {
                Name = "Многократно";
            }
        }
        public class UsageCouple : StateUsage
        {
            public UsageCouple() : base(Usages.Couple)
            {
                Name = "Несколько";
            }
        }
    }
}
