using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSSAlternative.AHP;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DSSAlternative.Tests
{
    [TestClass]
    public class RelationsTest
    {
        public IRelations Relations { get; set; }
        public IRelations RelationsAdvanced { get; set; }
        
        [TestInitialize]
        public void CreateRelations()
        {
            Relations = CreateDefault();
            RelationsAdvanced = CreateAdvanced();

            IRelations CreateDefault()
            {
                INode main = new Node("Цель");
                INode k1 = new Node(1, "К1", 1, 0);
                INode k2 = new Node(1, "К2", 1, 0);
                INode a1 = new Node(2, "А1", 2, 1);
                INode a2 = new Node(2, "А2", 2, 1);
                INode a3 = new Node(2, "А3", 2, 1);
                INode b1 = new Node(3, "В1", 3, 2);
                INode b2 = new Node(3, "В2", 3, 2);

                IHierarchy hier = new HierarchyNodes(main, k1, k2, a1, a2, a3, b1, b2);
                IRelations relations = new Relations(hier);

                relations.Set(main, k1, k2, 5);
                
                relations.Set(k1, a1, a2, 2);
                relations.Set(k1, a1, a3, 3);
                relations.Set(k1, a2, a3, 1.5);

                relations.Set(k2, a1, a2, 0.5);
                relations.Set(k2, a1, a3, 0.2);
                relations.Set(k2, a2, a3, 0.5);

                relations.Set(a1, b1, b2, 2);
                relations.Set(a2, b1, b2, 4);
                relations.Set(a3, b1, b2, 6);

                return relations;
            }
            IRelations CreateAdvanced()
            {
                INode main = new Node("Цель");
                INode k1 = new Node(1, "К1", 1, 0);
                INode k2 = new Node(1, "К2", 2, 0);
                INode v1 = new Node(2, "K1", 2, 1);
                INode v2 = new Node(2, "K2", 2, 1);
                INode a1 = new Node(3, "A1", 3, 2);
                INode a2 = new Node(3, "A2", 3, 2);

                IHierarchy hier = new HierarchyNodes(main, k1, k2, a1, a2, v1, v2);
                IRelations relations = new Relations(hier);

                relations.Set(main, k1, k2, 3);
                relations.Set(k1, v1, v2, 3);

                relations.Set(k2, a1, a2, 7);
                relations.Set(v1, a1, a2, 7);
                relations.Set(v2, a1, a2, 1);

                return relations;
            } 
        }



        //Кол-во всех отношений соответствует ожидаемому
        //Число созданных критериев ожидаемое
        [TestMethod]
        public void RelationsCountBase_Check()
        {
            int expected = 34;

            int count = Relations.SelectMany(c => c).Count();

            Assert.AreEqual(expected, count, "Количество отношений для обычной иерархии некорректно");
        }
        [TestMethod]
        public void RelationsCountAdv_Check()
        {
            int expected = 20;

            int count = RelationsAdvanced.SelectMany(c => c).Count();

            Assert.AreEqual(expected, count, "Количество отношений для продвинутой иерархии некорректно");
        }

        [TestMethod]
        public void CriteriasCountBase_Check()
        {
            int expected = 8;

            int count = Relations.Count();

            Assert.AreEqual(expected, count, "Количество критериев для обычной иерархии некорректно");
        }
        [TestMethod]
        public void CriteriasCountAdv_Check()
        {
            int expected = 7;

            int count = RelationsAdvanced.Count();

            Assert.AreEqual(expected, count, "Количество критериев для продвинутой иерархии некорректно");
        }



        //Коэффициенты соответствуют ожиданиям в простой иерархии
        //Коэффициенты соответствуют ожиданиям в продвинутой иерархии
        [TestMethod]
        public void CoefficientsBase_Check()
        {
            double[] expected = new double[] { 1, 0.8333, 0.16666, 0.4759, 0.2733, 0.2507, 0.7508, 0.2491  };

            double[] coefficients = Relations.Select(c => c.Key).Select(n => n.Coefficient).ToArray();

            MatrixTest.CheckEqualArrays(expected, coefficients, 0.01, "Коэффициенты для обычной иерархии некорректны");
        }
        [TestMethod]
        public void CoefficeientsAdv_Check()
        {
            double[] expected = new double[] { 1, 0.75, 0.25, 0.804, 0.195, 0.5625, 0.1875 };

            double[] coefficients = RelationsAdvanced.Select(c => c.Key).Select(n => n.Coefficient).ToArray();

            MatrixTest.CheckEqualArrays(expected, coefficients, 0.01, "Коэффициенты для продвинутой иерархии некорректны");
        }


        //Коэффициенты обновляются при изменении отношения
        [TestMethod]
        public void CoefficientUpdated_True()
        {
            double[] coefficients = Relations.Select(c => c.Key).Select(n => n.Coefficient).ToArray();

            Relations.First().ElementAt(1).Value = 1;
            double[] newCoeffs = Relations.Select(c => c.Key).Select(n => n.Coefficient).ToArray();
            bool same = Enumerable.SequenceEqual(coefficients, newCoeffs);

            Assert.IsTrue(!same, "Обновление отношения не обновляет коэффициенты");
            Relations.First().ElementAt(1).Value = 9;
        }

        //некорректны при неизвестном критерии
        //некорректны при несогласованной критерии
        //корректны если всё норм
        [TestMethod]
        public void RelationsCorrect_True()
        {
            bool correct = Relations.Correct;

            Assert.IsTrue(correct, "Корректная иерархия отношений считается некорректной");
        }
        public void RelationsKnown_False()
        {
            var relations = RelationsAdvanced;

            relations.First().SetUnknown();
            bool correct = relations.Correct;

            Assert.IsTrue(!correct, "Неизвестное отношение продолжает оставаться известным");
        }
        [TestMethod]
        public void RelationsConsistent_False()
        {
            var relations = Relations;
            var criteria = relations.ElementAt(1);
            var relation = criteria.ElementAt(2);
            double oldValue = relation.Value;

            relation.Value = 0.5;
            bool correct = relations.Correct;

            Assert.IsTrue(!correct, "Несогласованное отношение продолжает оставаться известным");
            relation.Value = oldValue;
        }

        //Заполняются из шаблона ожидаемым образом
        public void SetFromTemplates_Check()
        {

        }



        //Изменить зависимость в расчетах матриц от сгруппированных отношений

    }
}
