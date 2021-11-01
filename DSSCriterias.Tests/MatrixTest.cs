using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSCriterias.Logic;
namespace DSSCriterias.Tests
{
    [TestClass]
    public class MatrixTest
    {
        //Создание
        [TestMethod]
        public void Create()
        {
            var mtx = MtxStat.CreateFromSize(3, 3);

            Assert.AreEqual(3, mtx.RowsCount, "В новой матрице 3х3 не 3 строки");
            Assert.AreEqual(3, mtx.ColsCount, "В новой матрице 3х3 не 3 столбца");
        }
        [TestMethod]
        public void CreateFromSize()
        {
            var mtx = MtxStat.CreateFromSize(3, 3);
            double[,] expected = new double[,]
            {
                { 0, 0, 0 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };
            CollectionAssert.AreEqual(expected, mtx.Values);
        }
        [TestMethod]
        public void CreateFromArray()
        {
            double[,] expected = new double[,]
            {
                { 1, 4, 7 },
                { 2, 5, 8 },
                { 3, 6, 9 }
            };
            var mtx = MtxStat.CreateFromArray(expected);
            CollectionAssert.AreEqual(expected, mtx.Values);
        }
        [TestMethod]
        public void CreateEmpty()
        {
            var mtx = MtxStat.CreateEmpty();
            Assert.IsTrue(mtx.IsEmpty);
        }


        //Установка значений
        [TestMethod]
        public void SetValue()
        {
            var mtx = MtxStat.CreateFromSize(3, 3);
            mtx.Set(1, 1, 3);

            Assert.AreEqual(3, mtx.Get(1, 1), "Не установилось значение в ячейку [1,1]");
        }
        [TestMethod]
        public void SetValue2()
        {
            var mtx = MtxStat.CreateFromSize(2, 2);
            mtx.Set(0, 0, 5);
            double[,] expected = new double[,]
            {
                { 5, 0 },
                { 0, 0 },
            };
            CollectionAssert.AreEqual(expected, mtx.Values);
        }
        [TestMethod]
        public void SetValueException()
        {
            var mtx = MtxStat.CreateFromSize(2, 2);
            Assert.ThrowsException<InvalidOperationException>(() => mtx.Set(5, 5, 5), "При попытке доступа к несуществующей ячейке нет исключения");
        }


        //Удаление строк и столбцов
        [TestMethod]
        public void RemoveColLast()
        {
            var mtx = MtxStat.CreateFromSize(3, 3);
            mtx.Set(0, 0, 5);
            mtx.Set(1, 1, 5);
            mtx.Set(2, 2, 5);
            mtx.RemoveCol(2);
            double[,] expected = new double[,]
            {
                { 5, 0 },
                { 0, 5 },
                { 0, 0 }
            };
            CollectionAssert.AreEqual(expected, mtx.Values);
        }
        [TestMethod]
        public void RemoveRowLast()
        {
            var mtx = MtxStat.CreateFromSize(3, 3);
            mtx.Set(0, 0, 5);
            mtx.Set(1, 1, 5);
            mtx.Set(2, 2, 5);
            mtx.RemoveRow(mtx.Rows.Last());
            double[,] expected = new double[,]
            {
                { 5, 0, 0 },
                { 0, 5, 0 }
            };
            CollectionAssert.AreEqual(expected, mtx.Values);
        }
        
        [TestMethod]
        public void RemoveColBetween()
        {
            var mtx = MtxStat.CreateFromSize(3, 3);
            mtx.Set(0, 0, 5);
            mtx.Set(1, 1, 5);
            mtx.Set(2, 2, 5);
            mtx.RemoveCol(1);
            double[,] expected = new double[,]
            {
                { 5, 0 },
                { 0, 0 },
                { 0, 5 }
            };
            CollectionAssert.AreEqual(expected, mtx.Values);
        }
        [TestMethod]
        public void RemoveRowBetween()
        {
            var mtx = MtxStat.CreateFromSize(3, 3);
            mtx.Set(0, 0, 5);
            mtx.Set(1, 1, 5);
            mtx.Set(2, 2, 5);
            mtx.RemoveRow(1);
            double[,] expected = new double[,]
            {
                { 5, 0, 0 },
                { 0, 0, 5 }
            };
            CollectionAssert.AreEqual(expected, mtx.Values);
        }
        

        //Добавление
        [TestMethod]
        public void AddColLast()
        {
            var mtx = MtxStat.CreateFromSize(2, 2);
            mtx.Set(0, 0, 1);
            mtx.Set(0, 1, 2);
            mtx.Set(1, 0, 3);
            mtx.Set(1, 1, 4);
            mtx.AddColEnd();
            double[,] expected = new double[,]
            {
                { 1, 2, 0 },
                { 3, 4, 0 }
            };
            CollectionAssert.AreEqual(expected, mtx.Values);
        }
        [TestMethod]
        public void AddRowLast()
        {
            var mtx = MtxStat.CreateFromSize(2, 2);
            mtx.Set(0, 0, 1);
            mtx.Set(0, 1, 2);
            mtx.Set(1, 0, 3);
            mtx.Set(1, 1, 4);
            mtx.AddRowEnd();
            double[,] expected = new double[,]
            {
                { 1, 2 },
                { 3, 4 },
                { 0, 0 }
            };
            CollectionAssert.AreEqual(expected, mtx.Values);
        }
        
        [TestMethod]
        public void AddColBetween()
        {
            var mtx = MtxStat.CreateFromSize(2, 2);
            mtx.Set(0, 0, 1);
            mtx.Set(0, 1, 2);
            mtx.Set(1, 0, 3);
            mtx.Set(1, 1, 4);
            mtx.AddColBefore(mtx.Cols[1]);
            double[,] expected = new double[,]
            {
                { 1, 0, 2 },
                { 3, 0, 4 }
            };
            CollectionAssert.AreEqual(expected, mtx.Values);
        }
        [TestMethod]
        public void AddRowBetween()
        {
            var mtx = MtxStat.CreateFromSize(2, 2);
            mtx.Set(0, 0, 1);
            mtx.Set(0, 1, 2);
            mtx.Set(1, 0, 3);
            mtx.Set(1, 1, 4);
            mtx.AddRowBefore(mtx.Rows[1]);
            double[,] expected = new double[,]
            {
                { 1, 2 },
                { 0, 0 },
                { 3, 4 }
            };
            CollectionAssert.AreEqual(expected, mtx.Values);
        }
    }
}
