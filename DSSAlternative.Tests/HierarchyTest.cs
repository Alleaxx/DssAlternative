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
    public class HierarchyTest
    {
        private IHierarchy CreateCorrectHier()
        {
            INode goal = new Node(0, "ГЦ", 0, -1);
            INode K1 = new Node(1, "ГЦ", 1, 0);
            INode K2 = new Node(1, "ГЦ", 1, 0);
            INode A1 = new Node(2, "ГЦ", 2, 1);
            INode A2 = new Node(2, "ГЦ", 2, 1);
            return new HierarchyNodes(goal, K1, K2, A1, A2);
        }
        
        
        
        //Проверка на корректность иерархии
        //Некорректна без цели
        //Некорректна при <2 элементах на уровень
        //Некорректна при разрыве уровней
        [TestMethod]
        public void Hier_Correct()
        {
            IHierarchy hier = CreateCorrectHier();

            bool correct = hier.Correctness.IsCorrect;

            Assert.IsTrue(correct, "Корректная иерархия считается некорректной");
        }
        [TestMethod]
        public void HierNoGoal_NotCorrect()
        {
            IHierarchy hier = new HierarchyNodes(
                new Node(1, "К1", 1, 0),
                new Node(1, "К2", 1, 0),
                new Node(2, "A1", 2, 1),
                new Node(2, "A2", 2, 1)
            );

            bool correct = hier.Correctness.IsCorrect;

            Assert.IsTrue(!correct, "Иерархия без цели считается корректной");
        }
        [TestMethod]
        public void HierOneElement_NotCorrect()
        {
            IHierarchy hier = new HierarchyNodes(
                new Node(0, "ГЦ", 0, -1),
                new Node(1, "К1", 1, 0),
                new Node(2, "A1", 2, 1),
                new Node(2, "A2", 2, 1)
            );

            bool correct = hier.Correctness.IsCorrect;

            Assert.IsTrue(!correct, "Иерархия с одним элементом на уровне считается корректной");
        }
        [TestMethod]
        public void HierLevelDiff_NotCorrect()
        {
            IHierarchy hier = new HierarchyNodes(
                new Node(0, "ГЦ", 0, -1),
                new Node(1, "К1", 1, 0),
                new Node(1, "К2", 1, 0),
                new Node(3, "A1", 2, 1),
                new Node(3, "A2", 2, 1)
            );

            bool correct = hier.Correctness.IsCorrect;

            Assert.IsTrue(!correct, "Иерархия с разрывами в уровнях считается корректной");
        }
        [TestMethod]
        public void HierOnlyGoal_NotCorrect()
        {
            IHierarchy hier = new HierarchyNodes(
                new Node(0, "ГЦ", 0, -1)
            );

            bool correct = hier.Correctness.IsCorrect;

            Assert.IsTrue(!correct, "Иерархия с одной целью считается корректной");
        }


        //Обновление корректности при изменении узла
        //Обновление корректности при обновлении узла
        [TestMethod]
        public void HierNodeChange_NotCorrect()
        {
            IHierarchy hier = CreateCorrectHier();

            hier.First().Level = 10;
            bool correct = hier.Correctness.IsCorrect;

            Assert.IsTrue(!correct, "Иерархия после изменения элемента не изменила состояние");
        }
        [TestMethod]
        public void HierNodeMove_NotCorrect()
        {
            IHierarchy hier = CreateCorrectHier();

            hier.First().RemoveFromHierarchy();
            bool correct = hier.Correctness.IsCorrect;

            Assert.IsTrue(!correct, "Иерархия после удаления элемента не изменила состояния");
        }


        //Проверка, что узел принадлежит иерархии
        [TestMethod]
        public void HierNodeBelongs_True()
        {
            var hier = CreateCorrectHier();

            bool owns = hier.First().Hierarchy == hier;

            Assert.IsTrue(owns, "Узел из иерархии не указан принадлежащим к ней");
        }


        //При установке иерархии
        //Должен удалиться из старой
        //Должен появиться в новой
        [TestMethod]
        public void HierNodeAddAppear_Correct()
        {
            var hier1 = CreateCorrectHier();
            var node = new Node();

            node.SetHierarchy(hier1);
            bool added = hier1.Contains(node) && node.Hierarchy == hier1;

            Assert.IsTrue(added, "При установке иерархии узла он в ней не появился");
        }
        [TestMethod]
        public void HierNodeRemoved_Correct()
        {
            var hier1 = CreateCorrectHier();
            var node = hier1.Last();

            node.RemoveFromHierarchy();
            bool removed = !hier1.Contains(node);

            Assert.IsTrue(removed, "При удалении узла из иерархии он в ней остался");
        }
        [TestMethod]
        public void HierNodeMoving_Correct()
        {
            var hier1 = CreateCorrectHier();
            var hier2 = CreateCorrectHier();
            var node = hier1.Last();

            node.SetHierarchy(hier2);
            bool added = hier2.Contains(node) && !hier1.Contains(node);

            Assert.IsTrue(added, "Узел некорректно сменяет иерархию");
        }



        //Проверка на критерии для узла
        //Проверка на контролируемые узлы для узла
        [TestMethod]
        public void NodeCriterias_Check()
        {
            IHierarchy hier = CreateCorrectHier();
            INode K1 = hier.ElementAt(1);
            var expected = new INode[] { hier.MainGoal };

            var criterias = K1.Criterias();

            Assert.IsTrue(Enumerable.SequenceEqual(expected, criterias), "Критерии для узла определяются неверно");
        }
        [TestMethod]
        public void NodeControlled_Check()
        {
            IHierarchy hier = CreateCorrectHier();
            INode K1 = hier.ElementAt(1);
            var expected = hier.Alternatives;

            var controlled = K1.Controlled();

            Assert.IsTrue(Enumerable.SequenceEqual(expected, controlled), "Подконтрольные узлы для критерия определяются неверно");
        }
        [TestMethod]
        public void NodeNeighbors_Check()
        {
            IHierarchy hier = CreateCorrectHier();
            INode K1 = hier.ElementAt(1);
            var expected = hier.Criterias;

            var controlled = K1.NeighborsLevel();

            Assert.IsTrue(Enumerable.SequenceEqual(expected, controlled), "Соседи узла определяются неверно");
        }


        //Проверка порядкового номера в группе
        [TestMethod]
        public void NodeIndexGroup_Check()
        {
            IHierarchy hier = CreateCorrectHier();
            INode K2 = hier.ElementAt(2);
            int expected = 1;

            int index = K2.OrderIndexGroup();

            Assert.AreEqual(expected, index, "Порядковый номер по индексу для узла определяется неверно");
        }
        [TestMethod]
        public void NodeIndexLevel_Check()
        {
            IHierarchy hier = CreateCorrectHier();
            INode A1 = hier.ElementAt(3);
            int expected = 0;

            int index = A1.OrderIndexLevel();

            Assert.AreEqual(expected, index, "Порядковый номер по уровням для узла определяется неверно");
        }



        //Иерархии равны, если они фактически равны
        //Иерархии не равны, если различаются именем
        //Иерархии не равны, если различаются уровнем
        //Иерархии не равны, если различаются группами
        //Иерархии не равны, если различаются принадлежностями
        [TestMethod]
        public void HierEqual_IsTrue()
        {
            INode a1 = new Node(), a2 = new Node(1, "Цель", 0, 5);
            INode b1 = new Node(), b2 = new Node(1, "Цель", 0, 5);
            IHierarchy A = new HierarchyNodes(a1, a2);
            IHierarchy B = new HierarchyNodes(b1, b2);

            bool equal = HierarchyNodes.CompareEqual(A, B);

            Assert.IsTrue(equal, "Фактически равные коллекции считаются неравными");
        }
        [TestMethod]
        public void HierLevelEqual_IsFalse()
        {
            INode a1 = new Node(), a2 = new Node(1, "Цель", 0, 5);
            INode b1 = new Node(), b2 = new Node(2, "Цель", 0, 5);
            IHierarchy A = new HierarchyNodes(a1, a2);
            IHierarchy B = new HierarchyNodes(b1, b2);

            bool equal = HierarchyNodes.CompareEqual(A, B);

            Assert.IsTrue(!equal, "Коллекции с различием в имени считаются эквивалентными");
        }
        [TestMethod]
        public void HierNameEqual_IsFalse()
        {
            INode a1 = new Node(), a2 = new Node(1, "Цель", 0, 5);
            INode b1 = new Node(), b2 = new Node(1, "Цель2", 0, 5);
            IHierarchy A = new HierarchyNodes(a1, a2);
            IHierarchy B = new HierarchyNodes(b1, b2);

            bool equal = HierarchyNodes.CompareEqual(A, B);

            Assert.IsTrue(!equal, "Коллекции с различием в имени считаются эквивалентными");
        }
        [TestMethod]
        public void HierGroupEqual_IsFalse()
        {
            INode a1 = new Node(), a2 = new Node(1, "Цель", 0, 5);
            INode b1 = new Node(), b2 = new Node(1, "Цель", 1, 5);
            IHierarchy A = new HierarchyNodes(a1, a2);
            IHierarchy B = new HierarchyNodes(b1, b2);

            bool equal = HierarchyNodes.CompareEqual(A, B);

            Assert.IsTrue(!equal, "Коллекции с различием в имени считаются эквивалентными");
        }
        [TestMethod]
        public void HierIndexEqual_IsFalse()
        {
            INode a1 = new Node(), a2 = new Node(1, "Цель", 0, 5);
            INode b1 = new Node(), b2 = new Node(1, "Цель", 0, 10);
            IHierarchy A = new HierarchyNodes(a1, a2);
            IHierarchy B = new HierarchyNodes(b1, b2);

            bool equal = HierarchyNodes.CompareEqual(A, B);

            Assert.IsTrue(!equal, "Коллекции с различием в имени считаются эквивалентными");
        }
    }
}
