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
    public class ProjectTest
    {
        //При создании проекта в редактируемой и текущей иерархии разные узлы
        //При обновлении с редактируемой на текущую иерархии разные узлы
        [TestMethod]
        public void DifferentNodes_Check()
        {
            IProject project = AhpHierarchy.CreateNewProblem();

            bool same = project.HierarchyActive.First() == project.HierarchyEditing.First();

            Assert.IsTrue(!same, "При создании проекта редактируемая и активная иерархия ссылаются на одни и те же узлы");
        }
        [TestMethod]
        public void DifferentNodesUpdate_Check()
        {
            IProject project = AhpHierarchy.CreateNewProblem();

            project.HierarchyEditing.AddNode(new Node());
            project.UpdateHierarchy();
            bool same = project.HierarchyActive.First() == project.HierarchyEditing.First();

            Assert.IsTrue(!same, "При обновлении с редактируемой иерархии в проекте они ссылаются на одинаковые узлы");
        }

    }
}
