using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSCriterias.Logic;
using DSSCriterias.Logic.Criterias;
namespace DSSCriterias.Tests
{
    [TestClass]
    public class StatGameTest
    {
        private readonly double[,] MatrixExample;
        public StatGameTest()
        {
            MatrixExample = new double[,]
            {
                { 9, 9, 9 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };
        }


        //Проверка на обновление результатов критериев и выбранных альтернатив
        [TestMethod]
        //При изменении значения
        public void UpdateValue()
        {
            //assign
            StatGame game = new StatGame("", MtxStat.CreateFromArray(MatrixExample));
            ICriteria criteria = game.Report.GetCriteria<CriteriaMaxMax>();
            double firstResult = criteria.Result;
            var firstRank = game.Report.AlternativeRanks;

            //act
            for (int i = 0; i < 3; i++)
            {
                game.Mtx.Set(2, i, 15);
            }

            //assert
            Assert.IsTrue(criteria.Result != firstResult, "Критерий не обновлен после изменения значения матрицы");
            Assert.IsTrue(game.Report.AlternativeRanks != firstRank, "Отчет по критериям не обновлен после изменения значения матрицы");
        }

        [TestMethod]
        //При изменении вероятности исхода
        public void UpdateChance()
        {
            //assign
            StatGame game = new StatGame("", MtxStat.CreateFromArray(MatrixExample), new Situation() { Chances = StateChances.Riscs() });
            ICriteria criteria = game.Report.GetCriteria<CriteriaGerr>();
            double firstResult = criteria.Result;
            var firstRank = game.Report.AlternativeRanks;

            //act
            game.SetChance(0, 0.5);

            //assert
            Assert.IsTrue(criteria.Result != firstResult, "Критерий не обновлен после изменения вероятности исхода");
            Assert.IsTrue(game.Report.AlternativeRanks != firstRank, "Отчет по критериям не обновлен после изменения вероятности исхода");
        }

        [TestMethod]
        //При изменении структуры матрицы
        public void UpdateStructure()
        {
            //assign
            StatGame game = new StatGame("", MtxStat.CreateFromArray(MatrixExample));
            var criteria = game.Report.GetCriteria<CriteriaMaxMax>();
            double firstResult = criteria.Result;
            var firstRank = game.Report.AlternativeRanks;

            //act
            game.Mtx.RemoveRow(game.Mtx.Rows.First());

            //assert
            Assert.IsTrue(firstResult != criteria.Result, "Критерий не обновлен изменения структуры матрицы");
            Assert.IsTrue(game.Report.AlternativeRanks != firstRank, "Отчет по критериям не обновлен после изменения структуры матрицы");
        }

        [TestMethod]
        //При обновлении ситуации
        public void UpdateSituation()
        {
            //assign
            StatGame game = new StatGame("", MtxStat.CreateFromArray(MatrixExample));
            game.Report.IgnoreUsage = true;
            game.SetChance(0, 0.5);
            game.SetChance(1, 0.4);
            game.SetChance(2, 0.1);
            var criteria = game.Report.GetCriteria<CriteriaGerr>();
            double firstResult = criteria.Result;
            var firstRank = game.Report.AlternativeRanks;

            //act
            game.Situation.Chances = StateChances.Riscs();

            //assert
            Assert.IsTrue(firstResult != criteria.Result, "Критерий не обновлен после шанса у исхода");
            Assert.IsTrue(game.Report.AlternativeRanks != firstRank, "Отчет по критериям не обновлен после изменения ситуации матрицы");
        }
    }
}
