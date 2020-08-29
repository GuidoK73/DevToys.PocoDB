using DevToys.PocoDB.Factory;
using DevToys.PocoDB.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PocoDBConsoleAppTest.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DevToys.PocoDB.UnitTests
{
    /// <summary>
    /// Use ConnectionFactory.Instance to add a Custom Connection Object.
    /// 
    /// In the config you can use the ClientName ( 'MyCustomClient' ) for the ConnectionType property.
    /// in this example we use SqlConnection as a custom DataProvider.
    /// </summary>
    [TestClass]
    public class TestCustomDataProvider
    {
        public TestCustomDataProvider()
        {
            // On application start:
            ConnectionFactory.Instance.AddType<SqlConnection>("MyCustomClient");
        }

        [TestMethod()]
        public void TestCreateNewDataProvider()
        {
            var operation = new DbCommandOperation<Company, GetCompanyAll>("LocalCustom");
            operation.PreExecute += Operation_PreExecute;
            IEnumerable<Company> _result = operation.ExecuteReader(new GetCompanyAll() { });

            var _resultMaterialized = _result.ToList();
        }

        /// PreExecute event can be used to alter Command and Connection properties prior to execution, this may be needed when using custom data providers.
        private void Operation_PreExecute(object sender, DataOperationPreExecute e)
        {
            SqlCommand command = e.Command as SqlCommand;
            SqlConnection connection = e.Connection as SqlConnection;
        }
    }
}