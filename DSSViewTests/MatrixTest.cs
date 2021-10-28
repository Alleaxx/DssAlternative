using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System;

namespace DSSAHP.Tests
{
    [TestClass]
    public class MatrixTest
    {
        private MatrixAHP Matrix { get; set; }
        public MatrixTest()
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
        public void SumRows()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(Matrix.SumRows, new double[] { 6, 15, 24 }), "Неправильно считаются суммы строк");
        }

        [TestMethod]
        public void SumCols()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(Matrix.SumColumns, new double[] { 12, 15, 18 }), "Неправильно считается суммы столбцов");
        }
        

        public void Normalised()
        {
            Assert.AreEqual(Matrix.Normalised, new double[,] { { 1,2,3 }, { 1,2,3 }, { 1,2,3 } }, "Матрица неправильно нормализуется");
        }
        public void Coefficients()
        {
            Assert.AreEqual(Matrix.Coeffiients, new double[] { 12, 15, 18 }, "Матрица неверно расчитывает коэффициенты");
        }
    }


}
