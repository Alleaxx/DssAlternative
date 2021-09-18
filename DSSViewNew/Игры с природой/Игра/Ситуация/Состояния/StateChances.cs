using DSSLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSView.States;

namespace DSSView
{
    public enum Chances
    {
        Riscs, Unknown
    }

    public abstract class StateChances : State<Chances>
    {
        public static StateChances Get(Chances chance)
        {
            switch (chance)
            {
                case Chances.Riscs:
                    return new ChancesRiscs();
                case Chances.Unknown:
                    return new ChancesUnknown();
                default:
                    throw new Exception("Таких условий не бывает");
            }
        }


        public abstract double GetChance(IEnumerable<Case> cases, Case c);
        public abstract double GetChance(IEnumerable<Case> cases, int pos);
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

    namespace States
    {
        public class ChancesRiscs : StateChances
        {
            public ChancesRiscs() : base(Chances.Riscs)
            {
                Name = "Риски";
            }


            public override double GetChance(IEnumerable<Case> cases, int pos)
            {
                return GetChance(cases, cases.ElementAt(pos));
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
            }

            public override double GetChance(IEnumerable<Case> cases, Case c)
            {
                return DefaultMod(cases);
            }

            public override double GetChance(IEnumerable<Case> cases, int pos)
            {
                return DefaultMod(cases);
            }
        }
    }
}
