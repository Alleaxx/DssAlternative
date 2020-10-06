using System;
using System.Collections.Generic;
using System.Text;

namespace DSSLib
{
    public class CurrencyProblem : Problem
    {
        private static Dictionary<string, Dictionary<string, decimal>> Costs = new Dictionary<string, Dictionary<string, decimal>>()
        {
            ["RU"] = new Dictionary<string, decimal>()
            {
                ["EUR"] = 80,
                ["USD"] = 70,
            },
            ["USD"] = new Dictionary<string, decimal>()
            {
                ["EUR"] = 100,
                ["RU"] = 0.014M,
            },
            ["EUR"] = new Dictionary<string, decimal>()
            {
                ["RU"] = 100,
                ["USD"] = 0.0125M,
            },
        };
        private static Dictionary<string, Dictionary<string, MoneyRelation>> Capability = new Dictionary<string, Dictionary<string, MoneyRelation>>
        {
            ["RU"] = new Dictionary<string, MoneyRelation>()
            {
                ["EUR"] = new MoneyRelation(0.8,20,5),
                ["USD"] = new MoneyRelation(0.7,20,5),
            },
            ["USD"] = new Dictionary<string, MoneyRelation>()
            {
                ["EUR"] = new MoneyRelation(0.5,20,20),
                ["RU"] = new MoneyRelation(0.5,20,20),
            },
            ["EUR"] = new Dictionary<string, MoneyRelation>()
            {
                ["RU"] = new MoneyRelation(0.5,20,20),
                ["USD"] = new MoneyRelation(0.5,20,20),
            },
        };

        public string OwnCurrency { get; set; }
        public string ForeignCurrency { get; set; }
        public decimal Sum { get; set; }
        public TimeSpan Time { get; set; }


        public CurrencyProblem(decimal sum, string val1, string val2)
        {
            Name = $"Вложение {sum} {val1} в  {val2}";
            OwnCurrency = val1;
            ForeignCurrency = val2;
            Sum = sum;

            Random random = new Random((int)(sum) + val1.Length + val2.Length);
            decimal rateIncNow = random.Next(0, (int)Capability[val1][val2].IncRateMax);
            decimal rateDecNow = random.Next(0, (int)Capability[val1][val2].DecRateMax);
            decimal incPlus = Costs[val1][val2] * rateIncNow / 100;
            decimal decMinus = Costs[val1][val2] * rateDecNow / 100;
            decimal newIncCost = Costs[val1][val2] + incPlus;
            decimal newDecCost = Costs[val1][val2] - decMinus;
            Case inc = new Case("inc", "Повышение курса", Capability[val1][val2].IncChance);
            Case dec = new Case("dec", "Понижение курса", Capability[val1][val2].DecChance);

            Cases.Add(inc);
            Cases.Add(dec);
            
            Alternatives.Add(Get(0));
            Alternatives.Add(Get(0.25M));
            Alternatives.Add(Get(0.5M));
            Alternatives.Add(Get(0.75M));
            Alternatives.Add(Get(1));

            Alternative Get(decimal prc)
            {
                decimal ownCurrency = sum * (1 - prc);
                decimal foreign = sum * prc / Costs[val1][val2];
                decimal newOwnInc = foreign * newIncCost;
                decimal newOwnDec = foreign * newDecCost;
                decimal incProfit = ownCurrency + newOwnInc;
                decimal decProfit = ownCurrency + newOwnDec;

                return new Alternative($"{prc * 100}%",$"Перевести {sum - ownCurrency} {val1} в {val2}").SetCaseProfits((inc,(int)incProfit),(dec,(int)decProfit));
            }
        }

        public override void Output()
        {
            base.Output();
        }
    }

    public class MoneyRelation
    {
        public double IncChance { get; set; }

        public decimal IncRateMin { get; set; } = 0;
        public decimal IncRateMax { get; set; }


        public double DecChance { get; set; }

        public decimal DecRateMin { get; set; } = 0;
        public decimal DecRateMax { get; set; }

        public MoneyRelation(double incChance, decimal incRateMax,decimal decRateMax)
        {
            IncChance = incChance;
            DecChance = 1 - incChance;

            IncRateMax = incRateMax;
            DecRateMax = decRateMax;
        }

    }
}
