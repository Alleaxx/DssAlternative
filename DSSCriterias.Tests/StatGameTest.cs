using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSView;
namespace DSSCriterias.Tests
{
    [TestClass]
    public class StatGameTest
    {
        //Проверить обновление результата критериев при обновлении значения матрицы
        [TestMethod]
        public void UpdateValue()
        {
            double[,] gameMtx = new double[,]
            {
                { 9, 9, 9 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };
            StatGame game = new StatGame("", MtxStat.CreateFromArray(gameMtx));
            ICriteria criteria = game.Report.CriteriasConsider.First(c => c is DSSView.Criterias.CriteriaMaxMax);
            double firstResult = criteria.Result;

            for (int i = 0; i < 3; i++)
            {
                game.Mtx.Set(2, i, 15);
            }

            Assert.IsTrue(criteria.Result != firstResult, "Критерий не обновлен после изменения значения матрицы");
        }

        [TestMethod]
        //Проверить обновление результата критериев при обновлении шанса у матрицы
        public void UpdateChance()
        {
            double[,] gameMtx = new double[,]
            {
                { 9, 9, 9 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };
            StatGame game = new StatGame("", MtxStat.CreateFromArray(gameMtx));
            game.Situation.Chances = StateChances.Riscs();
            ICriteria criteria = game.Report.CriteriasConsider.First(c => c is DSSView.Criterias.CriteriaGerr);
            double firstResult = criteria.Result;

            game.Mtx.Cols[0].Chance = 0.5;

            Assert.IsTrue(criteria.Result != firstResult, "Критерий не обновлен после изменения вероятности исхода");
        }

        [TestMethod]
        //Проверить обновление результата критериев при изменении структуры матрицы
        public void UpdateStructure()
        {
            double[,] gameMtx = new double[,]
            {
                { 9, 9, 9 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };
            StatGame game = new StatGame("", MtxStat.CreateFromArray(gameMtx));
            var criteria = game.Report.CriteriasConsider.First(c => c is DSSView.Criterias.CriteriaMaxMax);
            double firstResult = criteria.Result;

            game.Mtx.RemoveRow(game.Mtx.Rows.First());

            Assert.IsTrue(firstResult != criteria.Result, "Критерий не обновлен изменения структуры матрицы");
        }

        [TestMethod]
        //Проверить обновление результата критериев при изменении ситуации матрицы
        public void UpdateSituation()
        {
            double[,] gameMtx = new double[,]
            {
                { 9, 9, 9 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };
            StatGame game = new StatGame("", MtxStat.CreateFromArray(gameMtx));
            game.Report.IgnoreUsage = true;

            var criteria = game.Report.CriteriasConsider.First(c => c is DSSView.Criterias.CriteriaGerr);
            game.Mtx.Cols[0].Chance = 0.5;
            game.Mtx.Cols[1].Chance = 0.4;
            game.Mtx.Cols[2].Chance = 0.1;
            double firstResult = criteria.Result;

            game.Situation.Chances = StateChances.Riscs();
            Assert.IsTrue(firstResult != criteria.Result, "Критерий не обновлен после шанса у исхода");
        }


        //Проверить, что нулевые вероятности при рисках равны 1 / n
    }
}
