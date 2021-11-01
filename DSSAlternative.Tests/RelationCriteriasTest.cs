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
    public class RelationCriteriasTest
    {
        private ICriteriaRelation Criteria { get; set; }
        
        [TestInitialize]
        public void SetCriteria()
        {
            INode main = new Node("Цель");
            INode a1 = new Node(1, "А1", 1, 0);
            INode a2 = new Node(1, "А2", 1, 0);
            INode a3 = new Node(1, "А3", 1, 0);
            INode a4 = new Node(1, "А4", 1, 0);

            IHierarchy hier = new HierarchyNodes(main, a1, a2, a3, a4);
            IRelations relations = new Relations(hier);

            relations.Set(main, a1, a2, 5);
            relations.Set(main, a1, a3, 3);
            relations.Set(main, a1, a4, 9);
            relations.Set(main, a2, a3, 3);
            relations.Set(main, a2, a4, 1);
            relations.Set(main, a3, a4, 1);

            Criteria = relations[main];
        }

        //Число отношений ожидаемое
        //Матрица отношений соответствует ожиданиям
        //Требуемые отношения у критерия соответствуют ожидаемым

        [TestMethod]
        public void RelationsCount_Check()
        {
            int count = Criteria.Count();

            Assert.AreEqual(16, count, "Количество отношений не соответствует ожидаемому");
        }

        [TestMethod]
        public void Matrix_Check()
        {
            double[,] expected = new double[,]
            {
                { 1, 5, 3, 9 },
                { 1.0 / 5, 1, 3, 1 },
                { 1.0 / 3, 1.0 / 3, 1, 1 },
                { 1.0 / 9, 1, 1, 1 }
            };

            double[,] mtx = Criteria.Mtx.Array;

            CollectionAssert.AreEquivalent(expected, mtx, "Матрица не соответствует ожиданиям");
        }

        [TestMethod]
        public void Required_Check()
        {
            var expected = new INodeRelation[]
            {
                Criteria.First(r => r.From.Name == "А1" && r.To.Name == "А2"),
                Criteria.First(r => r.From.Name == "А1" && r.To.Name == "А3"),
                Criteria.First(r => r.From.Name == "А1" && r.To.Name == "А4"),
                Criteria.First(r => r.From.Name == "А2" && r.To.Name == "А3"),
                Criteria.First(r => r.From.Name == "А2" && r.To.Name == "А4"),
                Criteria.First(r => r.From.Name == "А3" && r.To.Name == "А4"),
            };

            var required = Criteria.Required.ToList();

            CollectionAssert.AreEquivalent(expected, required, "Список только требуемых отношений не соответствует ожидаемому");
        }
    }
}
