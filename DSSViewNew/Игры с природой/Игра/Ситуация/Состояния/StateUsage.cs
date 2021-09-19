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
        OneTime, Couple, Any
    }

    public abstract class StateUsage : State<Usages>
    {
        private static readonly Dictionary<Usages, StateUsage> Values = new Dictionary<Usages, StateUsage>()
        {
            [Usages.Couple] = new UsageCouple(),
            [Usages.Any] = new UsageAnyTimes(),
            [Usages.OneTime] = new UsageOneTime()
        };
        public static StateUsage Get(Usages usage)
        {
            return Values[usage];
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
        public class UsageAnyTimes : StateUsage
        {
            public UsageAnyTimes() : base(Usages.Any)
            {
                Name = "Многократно";
            }
        }
        public class UsageCouple : StateUsage
        {
            public UsageCouple() : base(Usages.Couple)
            {
                Name = "Немного";
            }
        }
    }
}
