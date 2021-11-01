using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSAlternative.AHP;
namespace DSSAlternative.Tests
{
    [TestClass]
    public class MatrixTest
    {
        private readonly double[,] PositiveMtx = new double[,]
        {
            { 1.00, 2.00, 4.00 },
            { 0.50, 1.00, 2.00 },
            { 0.25, 0.50, 1.00 }
        };
        private readonly double[,] ZerosMtx = new double[,]
        {
            { 1.00, 0.00, 4.00 },
            { 0.50, 1.00, 2.00 },
            { 0.25, 0.00, 1.00 }
        };
        private readonly double[,] ExampleMtx = new double[,]
        {
            { 1, 1, 3 },
            { 1, 1, 5 },
            { 1.0 / 3, 0.2, 1 }
        };
        private readonly double[,] NotConsistentMtx = new double[,]
        {
            { 1, 5, 0.2 },
            { 0.2, 1, 9 },
            { 5, 1.0 / 9, 1 }
        };

        private readonly double[,] NegativeMtx;

        //Проверка общих выводов касательно матрицы
        [TestMethod]
        public void KnownMatrix_Known()
        {
            IMatrix posMtx = new Matrix(PositiveMtx);

            bool known = !posMtx.WithZeros();

            Assert.IsTrue(known, "Известная матрица считается неизвестной");
        }
        [TestMethod]
        public void UnknownMatrix_Unknown()
        {
            IMatrix zerosMtx = new Matrix(ZerosMtx);

            bool unknown = zerosMtx.WithZeros();

            Assert.IsTrue(unknown, "Неизвестная матрица считается известной");
        }
        //Симметричность матрицы (пока не доделана)
        [TestMethod]
        public void SymmetricSmall_IsTrue()
        {
            double[,] matrix = new double[,]
            {
                { 1.0, 5.0 },
                { 0.2, 1.0 }
            };

            bool isSymm = matrix.IsSymetric();

            Assert.IsTrue(isSymm, "Симметричная матрица считается несимметрично");

        }
        [TestMethod]
        public void NonSymmetric_IsFalse()
        {
            double[,] matrix = new double[,]
            {
                { 5, 5 },
                { 0.2, 1 }
            };

            bool isSymm = matrix.IsSymetric();

            Assert.IsTrue(!isSymm, "Несимметричная матрица считается симметричной");
        }
        [TestMethod]
        public void NonSymmetric2_IsFalse()
        {
            double[,] matrix = new double[,]
            {
                { 1, 3 },
                { 0.2, 1 }
            };

            bool isSymm = matrix.IsSymetric();

            Assert.IsTrue(!isSymm, "Несимметричная матрица считается симметричной");
        }


        //Проверка согласованности
        [TestMethod]
        public void MatrixSmall_Consistent()
        {
            double[,] source = new double[,]
            {
                { 1, 2 },
                { 2, 3 }
            };
            IMatrix mtx = new Matrix(source);

            bool consistent = mtx.IsCorrect;

            Assert.IsTrue(consistent, "Известная матрица 2х2 считается непригодной");
        }
        [TestMethod]
        public void MatrixUnknown_Consistent()
        {
            IMatrix mtx = new Matrix(ZerosMtx);

            bool consistent = mtx.IsCorrect;

            Assert.IsTrue(consistent, "Неизвестная матрица считается непригодной");
        }
        [TestMethod]
        public void MatrixConsistent_Consistent()
        {
            IMatrix mtx = new Matrix(PositiveMtx);

            bool consistent = mtx.IsCorrect;

            Assert.IsTrue(consistent, "Известная согласованная матрица считается непригодной");
        }
        [TestMethod]
        public void MatrixNotConsistent_NotConsistent()
        {
            IMatrix mtx = new Matrix(NotConsistentMtx);

            bool notConsistent = !mtx.IsCorrect;

            Assert.IsTrue(notConsistent, "Известная несогласованная матрица считается согласованной");
        }
        [TestMethod]
        public void ConsistencyCr_Check()
        {
            IMatrix mtx = new Matrix(PositiveMtx);

            double Cr = mtx.Cr;

            Assert.AreEqual(0, Cr, 0.01, "Показатель согласованности Cr считается неверно");
        }
        [TestMethod]
        public void ConsistencyCr2_Check()
        {
            IMatrix mtx = new Matrix(ExampleMtx);

            double Cr = mtx.Cr;

            Assert.AreEqual(0.015, Cr, 0.05, "Показатель согласованности Cr считается неверно");
        }



        //Проверка действий с матрицами 
        [TestMethod]
        public void LocalCoeffsVector_Check()
        {
            double[] expected = new double[] { 0.405, 0.480, 0.113 };
            IMatrix mtx = new Matrix(ExampleMtx);

            double[] res = mtx.Coeffiients;

            CheckEqualArrays(expected, res, 0.01, "Локальные коэффициенты расчитываются неверно");
        }
        [TestMethod]
        public void MultiCoeffsArrVector_Check()
        {
            double[] expected = new double[] { 1.227, 1.455, 0.345 };
            IMatrix mtx = new Matrix(ExampleMtx);

            double[] res = mtx.Array.MultiMatrixLocalCoeffs();

            CheckEqualArrays(expected, res, 0.01, "Произведение матрицы на её вектор коэффициентов расчитывается неверно");
        }
        [TestMethod]
        public void GeometricMulti_Check()
        {
            double[] expected = new double[] { 3, 5, 0.0666 };

            var geometricMulti = ExampleMtx.GeometricMultiVector();

            CheckEqualArrays(expected, geometricMulti, 0.01, "Геометрическое произведение рассчитывается неверно");
        }
        [TestMethod]
        public void MatrixMulti_Check()
        {
            double[] expected = new double[] { 1.2, 1.4, 0.34666 };
            double[] vector = new double[] { 0.5, 0.4, 0.1 };

            var multi = ExampleMtx.Multiply(vector);

            CheckEqualArrays(expected, multi, 0.01, "Умножение матрицы на вектор производится неверно");
        }
        [TestMethod]
        public void Normalisation_Check()
        {
            double[] expected = new double[] { 1.66666, 1.33333, 0.3333 };
            double[] vector = new double[] { 5, 4, 1 };

            double[] normalised = MtxActions.Normalise(vector);

            CheckEqualArrays(expected, normalised, 0.01, "Нормализация матрицы некорректна");
        }
        [TestMethod]
        public void NormalisationParam_Check()
        {
            double[] expected = new double[] { 0.8333, 0.6666, 0.16666 };
            double[] vector = new double[] { 5, 4, 1 };

            double[] normalised = MtxActions.Normalise(vector, 6);

            CheckEqualArrays(expected, normalised, 0.01, "Нормализация матрицы с заданным параметром некорректна");
        }




        public static void CheckEqualArrays(double[] expected, double[] received, double delta, string message)
        {
            if (expected.Length != received.Length)
            {
                Assert.Fail($"{message}: Массивы не равны");
            }
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], received[i], delta, $"{message}: Массивы не равны");
            }
        }
        public static void CheckEqualArrays(double[,] expected, double[,] received, double delta, string message)
        {
            if (expected.GetLength(0) != received.GetLength(0) || expected.GetLength(1) != received.GetLength(1))
            {
                Assert.Fail($"{message}: Массивы не равны");
            }
            for (int r = 0; r < expected.GetLength(0); r++)
            {
                for (int c = 0; c < expected.GetLength(1); c++)
                {
                    Assert.AreEqual(expected[r, c], received[r, c], delta, $"{message}: Массивы не равны");
                }
            }
        }



        //Изменение значений матрицы невозможно
        //Изменение значений матрицы возвращает другую матрицу
        //...
    }
}
