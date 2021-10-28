using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSCriterias.Logic.States;

namespace DSSCriterias.Logic
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

}

namespace DSSCriterias.Logic.States
{
    public class UsageOneTime : StateUsage
    {
        public UsageOneTime() : base(Usages.OneTime)
        {
            Name = "Однократно";
            AddCompare(Usages.OneTime, 2, "Идеален для однократного применения");
            AddCompare(Usages.Couple, 0, "Не слишком подходит для нескольких применений");
            AddCompare(Usages.Any, -2, "Не подходит для многократного применения");
        }
    }
    public class UsageAnyTimes : StateUsage
    {
        public UsageAnyTimes() : base(Usages.Any)
        {
            Name = "Многократно";
            AddCompare(Usages.Any, 2, "Идеален для многократного применения");
            AddCompare(Usages.OneTime, 0, "Не слишком подходит для однократного применения");
            AddCompare(Usages.Couple, 1, "Можно использовать и для нескольких применений");
        }
    }
    public class UsageCouple : StateUsage
    {
        public UsageCouple() : base(Usages.Couple)
        {
            Name = "Немного";
            AddCompare(Usages.Couple, 2, "Идеален для нескольких применений");
            AddCompare(Usages.OneTime, 0, "Не совсем подходит для единственного применения");
            AddCompare(Usages.Any, 1, "Вполне можно использовать для многократного применения");
        }
    }
}