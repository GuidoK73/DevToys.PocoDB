using DevToys.PocoDB.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PocoDBConsoleAppTest.Data;
using System.Collections.Generic;

namespace DevToys.PocoDB.UnitTests
{
    /// <summary>
    /// Wrapper arround System.Data.SqlClient.SqlBulkCopy
    /// </summary>
    [TestClass] 
    public class TestBulkInsert
    {
        [TestMethod]
        public void BulkInsertCompany()
        {
            List<BulkCompany> _data = new List<BulkCompany>();

            for (int ii = 0; ii < 5000; ii++)
                _data.Add(new BulkCompany() { Name = "Guido", ZipCode = "4624JC", CompanyType = CompanyType.LLC });

            BulkInsertOperation<BulkCompany> operation = new BulkInsertOperation<BulkCompany>("Local", 512);
            operation.Progress += Operation_Progress;

            operation.Insert(_data);
        }

        private static void Operation_Progress(object sender, BulkInsertEventArgs e)
        {
        }


        [TestMethod]
        public void BulkInsertCompanyRandom()
        {
            BulkInsertOperation<BulkCompanyRandom> operation = new BulkInsertOperation<BulkCompanyRandom>("Local", 512);
            operation.Progress += Operation_Progress;

            operation.Insert(1000);
        }

    }
}