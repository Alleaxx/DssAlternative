using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System;
using DSSView;

namespace DSSViewTests
{
    [TestClass]
    public class AHPMatrixTest
    {
        private MatrixAHP Matrix { get; set; }
        public AHPMatrixTest()
        {
            double[,] values = new double[3, 3]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 },
            };
            Matrix = new MatrixAHP(values);
        }

        [TestMethod]
        public void SumTest()
        {
            Assert.AreEqual(Matrix.Sum, 45, "Неправильно считается сумма матрицы");
        }

        [TestMethod]
        public void SumRowsTest()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(Matrix.SumRows, new double[] { 6, 15, 24 }), "Неправильно считаются суммы строк");
        }

        [TestMethod]
        public void SumColsTest()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(Matrix.SumColumns, new double[] { 12, 15, 18 }), "Неправильно считается суммы столбцов");
        }
        

        public void NormalisedTest()
        {
            Assert.AreEqual(Matrix.Normalised, new double[,] { { 1,2,3 }, { 1,2,3 }, { 1,2,3 } }, "Матрица неправильно нормализуется");
        }
        public void CoefficientsTest()
        {
            Assert.AreEqual(Matrix.Coeffiients, new double[] { 12, 15, 18 }, "Матрица неверно расчитывает коэффициенты");
        }
    }


    [TestClass]
    public class AHPAlgoritmTest
    {
        private Problem DefaultProblem { get; set; }

        Node Place { get; set; }
        Node Reputation { get; set; }

        Node A { get; set; }
        Node B { get; set; }
        Node C { get; set; }

        Node F1 { get; set; }
        Node F2 { get; set; }

        public AHPAlgoritmTest()
        {
            DefaultProblem = new Problem();

            Place = new Node(DefaultProblem, 1, "Местоположение");
            Reputation = new Node(DefaultProblem, 1,"Репутация");
            DefaultProblem.AddInner(Reputation, Place);

            A = new Node(DefaultProblem, 2, "Универ А");
            B = new Node(DefaultProblem, 2, "Универ B");
            C = new Node(DefaultProblem, 2, "Универ C");
            DefaultProblem.AddInner(A, B, C);

            F1 = new Node(DefaultProblem, 3, "Факультет ПИ");
            F2 = new Node(DefaultProblem, 3, "Факультет БИ");
            DefaultProblem.AddInner(F1, F2);

            Node.SetRelationBetween(DefaultProblem, Reputation, Place, 5);
            Node.SetRelationBetween(Reputation, A, B, 2);
            Node.SetRelationBetween(Reputation, A, C, 3);
            Node.SetRelationBetween(Reputation, B, C, 1.5);
            Node.SetRelationBetween(Place, B, A, 2);
            Node.SetRelationBetween(Place, C, A, 5);
            Node.SetRelationBetween(Place, C, B, 2);
            Node.SetRelationBetween(A, F1, F2, 2);
            Node.SetRelationBetween(B, F1, F2, 4);
            Node.SetRelationBetween(C, F1, F2, 6);
        }

        [TestMethod]
        public void MainCriteriaCoeffTest()
        {
            Assert.AreEqual(DefaultProblem.Coefficient, 1, "Коэффициент критерия 0-го ур. неверно рассчитывается");
        }
        

        [TestMethod]
        public void CriteriaStructureLevelCountTest()
        {
            Assert.AreEqual(DefaultProblem.Dictionary.Count, 4, "Количество уровней критериев не соответствует необходимому");
        }
        [TestMethod]
        public void CriteriaStructure1Test()
        {
            Assert.AreEqual(DefaultProblem.Dictionary[1].Count, 2, "Некорректная структура критериев");
        }
        [TestMethod]
        public void CriteriaStructure2Test()
        {
            Assert.AreEqual(Reputation.Dictionary[1].Count, 3, "Некорректная структура критериев");
        }

        [TestMethod]
        public void CriteriaRequiredRelationsThisTest()
        {
            Assert.AreEqual(Reputation.GetReqRelationsThis().Count, 3, "Некорректный расчет количества необходимых для заполнения отношений");
        }

        [TestMethod]
        public void CriteriaRequiredRelationsInnerTest()
        {
            Assert.AreEqual(DefaultProblem.GetReqRelationsInner().Count, 10, "Некорректный расчет количества всех внутренних необходимых для заполнения отношений");
        }


        [TestMethod]
        public void CoefficientTest()
        {
            Assert.AreEqual(Math.Round(Reputation.Coefficient,3), 0.833, "Коэффициент критерия 1-го ур. неверно рассчитывается");
        }
        [TestMethod]
        public void Coefficient2Test()
        {
            Assert.AreEqual(Math.Round(F1.Coefficient,3), 0.751, "Коэффициент критерия 4-го ур. неверно рассчитывается");
        }



        [TestMethod]
        public void ConstistencyTest()
        {
            Assert.AreEqual(Math.Round(Reputation.Matrix.Consistency.Cr,5), 0, "Неверно рассчитывается согласованность матрицы");
        }

        [TestMethod]
        public void ChangeRelationTest()
        {
            Node.SetRelationBetween(DefaultProblem, Reputation, Place, 10);

            Assert.AreEqual(Math.Round(F2.Coefficient,3), 0.2550, "Коэффициент критерия 4-го ур. неверно рассчитывается после изменения значения в матрице сравнений");

            Node.SetRelationBetween(DefaultProblem, Reputation, Place, 5);
        }
    }
}
