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
    public class RelationNodeTest
    {
        private (INodeRelation A, INodeRelation B) CreateMirroredRelations()
        {
            INode main = new Node();
            INode node1 = new Node();
            INode node2 = new Node();

            INodeRelation rel1 = new NodeRelation(main, node1, node2, 0);
            INodeRelation rel2 = new NodeRelation(main, node2, node1, 0);

            return (rel1, rel2);
        }


        //Зеркальные отношения меняются зеркально
        //Зеркальные отношения устанавливаются неизвестными зеркально
        [TestMethod]
        public void MirrorChange_Check()
        {
            var Pair = CreateMirroredRelations();

            Pair.A.Mirrored = Pair.B;
            Pair.B.Mirrored = Pair.A;

            Pair.A.Value = 5;

            Assert.AreEqual(Pair.B.Value, 1.0 / 5.0, "Отношения меняются не зеркально");
        }
        [TestMethod]
        public void MirrorUnknownChange_Check()
        {
            var Pair = CreateMirroredRelations();

            Pair.A.Mirrored = Pair.B;
            Pair.B.Mirrored = Pair.A;

            Pair.A.Value = 5;
            Pair.B.Value = 0;

            Assert.AreEqual(Pair.A.Value, 0, "Отношения устанавливаются неизвестными не вместе");
            Assert.AreEqual(Pair.B.Value, 0, "Отношения устанавливаются неизвестными не вместе");
        }


        //Отношения сами к себе не меняются
        [TestMethod]
        public void SelfChange_Check()
        {
            INode main = new Node();
            INode node = new Node();
            INodeRelation relation = new NodeRelation(main, node);

            relation.Value = 5;

            Assert.AreEqual(relation.Value, 1, "Отношение само к себе меняет свое значение");
        }
        [TestMethod]
        public void SelfUnknown_Check()
        {
            INode main = new Node();
            INode node = new Node();
            INodeRelation relation = new NodeRelation(main, node);

            relation.Value = 0;

            Assert.AreEqual(relation.Value, 1, "Отношение само к себе не может стать неизвестным");
        }


        [TestMethod]
        public void ReflectRelation_Check()
        {
            var Pair = CreateMirroredRelations();

            Pair.A.Mirrored = Pair.B;
            Pair.B.Mirrored = Pair.A;

            Pair.A.Value = 5;
            Pair.B.Reflect();

            Assert.AreEqual(Pair.B.Value, 5.0, "Отношения меняются не зеркально");
        }

    }
}
