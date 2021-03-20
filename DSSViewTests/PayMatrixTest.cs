using Microsoft.VisualStudio.TestTools.UnitTesting;
using DSSView;

namespace DSSViewTests
{
    [TestClass]
    public class PayMatrixTest
    {
        [TestMethod]
        public void OkTest()
        {
            View view = View.Ex;
            View.Ex.ViewMatrix = new ViewMatrix();
            var test = View.Ex.ViewMatrix.ListMatrix;
            Assert.AreEqual(test.Count, 2, "Несоответствие!");
        }
    }
}
