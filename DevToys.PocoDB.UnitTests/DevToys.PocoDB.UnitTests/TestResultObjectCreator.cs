using System;
using DevToys.PocoDB.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevToys.PocoDB.UnitTests
{ 
    [TestClass]
    public class TestResultObjectCreator
    {
        [TestMethod]
        public void TestResultObjectCreation()
        {
            var operation = new ResultObjectCreator("Local");
            var _result = operation.ExecuteSingleReader("select * from dbo.Company");
        }
    }
}
