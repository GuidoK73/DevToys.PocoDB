using DevToys.PocoDB.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PocoDBConsoleAppTest.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DevToys.PocoDB.UnitTests
{ 
    [TestClass]
    public class TestRandomData
    {
        [TestMethod()]
        public void TestInsertRandomData()
        {
            var operation = new DbCommandOperation<InsertCompanyRandom>("Local");

            InsertCompanyRandom parameters = new InsertCompanyRandom() { };

            for (int ii = 0; ii < 50; ii++)
            {
                operation.ExecuteNonQuery(parameters);
                int newId = parameters.Id;
            }

            
        }

        [TestMethod]
        public void BulkInsertCompanyRandom()
        {
            BulkInsertOperation<BulkCompanyRandom> operation = new BulkInsertOperation<BulkCompanyRandom>("Local", 512);
            operation.Insert(1000);
        }

    }
}
