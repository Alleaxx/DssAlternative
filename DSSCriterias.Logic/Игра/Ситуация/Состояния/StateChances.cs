using DSSLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSCriterias.Logic.States;

namespace DSSCriterias.Logic
{
    public enum Chances
    {
        Riscs, Unknown
    }

    public abstract class StateChances : State<Chances>
    {
        private static readonly Dictionary<Chances, StateChances> Values = new Dictionary<Chances, StateChances>()
        {
            [Chances.Riscs] = new ChancesRiscs(),
            [Chances.Unknown] = new ChancesUnknown()
        };
        public static StateChances Get(Chances chance)
        {
            return Values[chance];
        }
        public static StateChances Unknown()
        {
            return new ChancesUnknown();
        }
        public static StateChances Riscs()
        {
            return new ChancesRiscs();
        }


        public double GetChance(IEnumerable<Case> cases, int pos)
        {
            return GetChance(cases, cases.ElementAt(pos));
        }
        public virtual double GetChance(IEnumerable<Case> cases, Case c)
        {
            return DefaultMod(cases);
        }


        public double SumCases(IEnumerable<Case> cases)
        {
            return cases.Sum(c => GetChance(cases, c));
        }
        public bool IsOk(IEnumerable<Case> cases)
        {
            return SumCases(cases) == 1;
        }

        protected StateChances(Chances chances) : base(chances)
        {
        }

        protected double DefaultMod(IEnumerable<Case> cases)
        {
            return (double)1 / cases.Count();
        }
    }

}

namespace DSSCriterias.Logic.States
{
    public class ChancesRiscs : StateChances
    {
        public ChancesRiscs() : base(Chances.Riscs)
        {
            Name = "Риски";
            AddCompare(Chances.Riscs, 3, "Предназначен для условий риска");
            AddCompare(Chances.Unknown, -10, "Не применяется в условиях неопределенности");
        }

        public override double GetChance(IEnumerable<Case> cases, Case c)
        {
            double sum = cases.Sum(cas => cas.Chance);
            if (sum == 0)
            {
                return DefaultMod(cases);
            }
            else
            {
                return c.Chance / sum;
            }
        }
    }
    public class ChancesUnknown : StateChances
    {
        public ChancesUnknown() : base(Chances.Unknown)
        {
            Name = "Неопределенность";
            AddCompare(Chances.Unknown, 3, "Предназначен для условий неопределенности");
            AddCompare(Chances.Riscs, -10, "Не применяется в условиях риска");
        }
    }
}