using Microsoft.VisualStudio.TestTools.UnitTesting;

using DSSView;
using DSSView.Criterias;
using System;
using DSSLib;
using System.Collections.Generic;

namespace DSSCriterias.Tests
{

    class PremadeStatGame : IStatGame
    {
        public override string ToString()
        {
            return Name;
        }

        public event Action OnInfoUpdated;
        public event Action OnStructureChanged;

        public string Name { get; set; }
        public double[,] Arr { get; private set; }

        public double[] Chances { get; private set; }
        public double GetChance(int col)
        {
            return Chances[col];
        }

        public Alternative[] Alternatives { get; set; }
        public Alternative GetRow(int r)
        {
            return Alternatives[r];
        }


        public Situation Situation { get; private set; }

        public PremadeStatGame(string name, Alternative[] alts, double[,] arr, double[] chances)
        {
            Name = name;
            Arr = arr;
            Chances = chances;
            Alternatives = alts;
            Situation = new Situation();
        }
    }

    [TestClass]
    public class CriteriasTest
    {
        public IStatGame PositiveMtxGame { get; set; }
        public IStatGame NegativeMtxGame { get; set; }
        public IStatGame MixedMtxGame { get; set; }
        public IStatGame ChancesMtxGame { get; set; }
        public CriteriasTest()
        {
            double third = (double)1 / 3;
            Alternative[] alts = new Alternative[]
            {
                new Alternative("A1"), new Alternative("A2"), new Alternative("A3")
            };
            PositiveMtxGame = new PremadeStatGame("Положительная матрица", alts,
                new double[3, 3] {
                    { 1, 6, 5 },
                    { 7, 3, 4 },
                    { 8, 2, 4 },
                },
                new double[] { third, third, third });
            
            NegativeMtxGame = new PremadeStatGame("Отрицательная матрица", alts,
                new double[3, 3] {
                    { -5, -1, -7 },
                    { -4, -5, -6 },
                    { -1, -3, -9 },
                },
                new double[] { third, third, third });
            
            MixedMtxGame = new PremadeStatGame("Смешанная матрица", alts,
                new double[3, 3] {
                    { 1, 2, -3 },
                    { 4, 3, -2 },
                    { 6, 3, -6 },
                },
                new double[] { third, third, third });
            
            ChancesMtxGame = new PremadeStatGame("Вероятностная матрица", alts,
                new double[3, 3] {
                    { 8, 2, 3 },
                    { 4, 3, 6 },
                    { 7, 1, 9 },
                },
                new double[] { 0.3, 0.5, 0.2 });
            ChancesMtxGame.Situation.Chances = StateChances.Riscs();
        }


        //Баейса + - ~ %
        [TestMethod]
        public void BaiesCriteria()
        {
            ICriteria criteria = new CriteriaBaies(PositiveMtxGame);
            double expected = 4.6666;
            Assert.AreEqual(expected, criteria.Result, 0.01, $"{criteria.Name} некорректен с положительной матрицей ({criteria.Result} != {expected})");
            
            criteria = new CriteriaBaies(NegativeMtxGame);
            expected = -4.3333;
            Assert.AreEqual(expected, criteria.Result, 0.01, $"{criteria.Name} некорректен с отрицательной матрицей ({criteria.Result} != {expected})");

            criteria = new CriteriaBaies(MixedMtxGame);
            expected = 1.66666;
            Assert.AreEqual(expected, criteria.Result, 0.01, $"{criteria.Name} некорректен со смешанной матрицей ({criteria.Result} != {expected})");

            criteria = new CriteriaBaies(ChancesMtxGame);
            expected = 4.4;
            Assert.AreEqual(expected, criteria.Result, 0.01, $"{criteria.Name} некорректен с вероятностной ({criteria.Result} != {expected})");
        }
        
        
        //Лапласа + - ~
        [TestMethod]
        public void LaplasCriteria()
        {
            Dictionary<IStatGame, double> Values = new Dictionary<IStatGame, double>()
            {
                [PositiveMtxGame] = 4.6666,
                [NegativeMtxGame] = -4.3333,
                [MixedMtxGame] = 1.666
            };
            foreach (var check in Values)
            {
                ICriteria criteria = new CriteriaLaplas(check.Key);
                double expected = check.Value;
                Assert.AreEqual(expected, criteria.Result, 0.01, $"{criteria.Name} некорректен с матрицей {check.Key} ({criteria.Result} != {expected})");
            }
        }

        //Максимакса + - ~
        [TestMethod]
        public void AzartCriteria()
        {
            Dictionary<IStatGame, double> Values = new Dictionary<IStatGame, double>()
            {
                [PositiveMtxGame] = 8,
                [NegativeMtxGame] = -1,
                [MixedMtxGame] = 6
            };
            CheckCriteria(game => new CriteriaMaxMax(game), Values);
        }


        //Минимакса + - ~
        [TestMethod]
        public void MinMaxCriteria()
        {
            Dictionary<IStatGame, double> Values = new Dictionary<IStatGame, double>()
            {
                [PositiveMtxGame] = 6,
                [NegativeMtxGame] = -4,
                [MixedMtxGame] = 2
            };
            CheckCriteria(game => new CriteriaMinMax(game), Values);
        }

        //Сэвиджа + - ~
        [TestMethod]
        public void SavigeCriteria()
        {
            Dictionary<IStatGame, double> Values = new Dictionary<IStatGame, double>()
            {
                [PositiveMtxGame] = 3,
                [NegativeMtxGame] = 3,
                [MixedMtxGame] = 2
            };
            CheckCriteria(game => new CriteriaSavige(game), Values);
        }

        //Вальда + - ~
        [TestMethod]
        public void WaldCriteria()
        {
            Dictionary<IStatGame, double> Values = new Dictionary<IStatGame, double>()
            {
                [PositiveMtxGame] = 3,
                [NegativeMtxGame] = -6,
                [MixedMtxGame] = -2
            };
            CheckCriteria(game => new CriteriaWald(game), Values);
        }


        //Гермейера + - ~ %
        [TestMethod]
        public void GermeierCriteria()
        {
            Dictionary<IStatGame, double> Values = new Dictionary<IStatGame, double>()
            {
                [PositiveMtxGame] = 9,
                [NegativeMtxGame] = -2,
                [MixedMtxGame] = -0.6666,
                [ChancesMtxGame] = 6,
            };
            CheckCriteria(game => new CriteriaGerr(game), Values);
        }

        //Гурвица + - ~
        [TestMethod]
        public void GurvitsCriteria()
        {
            Dictionary<IStatGame, double> Values = new Dictionary<IStatGame, double>()
            {
                [PositiveMtxGame] = 4.6,
                [NegativeMtxGame] = -4.6,
                [MixedMtxGame] = 0.4
            };
            CheckCriteria(game => new CriteriaGurvits(game), Values);
        }

        //Лемана + - ~ %
        [TestMethod]
        public void LemanCriteria()
        {
            Dictionary<IStatGame, double> Values = new Dictionary<IStatGame, double>()
            {
                [PositiveMtxGame] = 4,
                [NegativeMtxGame] = -5.4,
                [MixedMtxGame] = 0.2,
                [ChancesMtxGame] = 3.54,
            };
            CheckCriteria(game => new CriteriaLeman(game), Values);
        }

        //Произведений + - ~
        [TestMethod]
        public void MultipleCriteria()
        {
            Dictionary<IStatGame, double> Values = new Dictionary<IStatGame, double>()
            {
                [PositiveMtxGame] = 84,
                [NegativeMtxGame] = -27,
                [MixedMtxGame] = -6
            };
            CheckCriteria(game => new CriteriaMulti(game), Values);
        }


        private void CheckCriteria(Func<IStatGame, ICriteria> func, Dictionary<IStatGame, double> checks)
        {
            foreach (var check in checks)
            {
                ICriteria criteria = func.Invoke(check.Key);
                double expected = check.Value;
                Assert.AreEqual(expected, criteria.Result, 0.01, $"{criteria.Name} некорректен с матрицей {check.Key} ({criteria.Result} != {expected})");
            }
        }


    }
}
