using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DevToys.PocoDB.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PocoDBConsoleAppTest.Data;


namespace DevToys.PocoDB.UnitTests
{ 
    /// <summary>
    /// SqlOperation is similar to it's result as DBCommandOperation, the difference is the input.
    /// with SqlOperation you do not use DBCommand classes but just sql queries. For correct use it's adviced to use the SqlParameter always.
    /// </summary>
    [TestClass]
    public class TestSqlOperation
    {
        [TestMethod]
        public void SqlSelectOperation()
        {
            // The DynamicOperation allows you to use dynamic result and dynamic parameters.
            var operation = new SqlOperation<Company>("Local");

            string sql = "select Id, [name], Adress, Country, ZipCode, HouseNumber, CompanyType from dbo.Company";

            IEnumerable<Company> _result = operation.ExecuteReader(sql, CommandType.Text).ToList();

            var _resultMaterialized = _result.ToList();

        } 

        [TestMethod]
        public void SqlSelectOperationSingleReader()
        {
            // The DynamicOperation allows you to use dynamic result and dynamic parameters.
            var operation = new SqlOperation<Company>("Local");

            string sql = "select Id, [name], Adress, Country, ZipCode, HouseNumber, CompanyType from dbo.Company where Id = @Id";

            SqlParameter parameter = new SqlParameter() { ParameterName = "Id", Value = 1 };
            
            Company _result = operation.ExecuteSingleReader(sql, parameter);
        }

        [TestMethod]
        public void SqlInsertOperation()
        {
            // The DynamicOperation allows you to use dynamic result and dynamic parameters.

            var operation = new SqlOperation("Local");

            SqlParameter idParameter = new SqlParameter()
            {
                ParameterName = "id",
                Value = 0,
                Direction = ParameterDirection.InputOutput
            };

            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter() { ParameterName = "name", Value = "A Company Name" },
               new SqlParameter() { ParameterName = "Adress", Value = "" },
               new SqlParameter() { ParameterName = "Country", Value = "" },
               new SqlParameter() { ParameterName = "ZipCode", Value = "" },
               new SqlParameter() { ParameterName = "HouseNumber", Value = "" },
               new SqlParameter() { ParameterName = "CompanyType", Value = 1 },
               new SqlParameter() { ParameterName = "Text", Value = 1 },
               idParameter
            };

            operation.ExecuteNonQuery("dbo.InsertCompany", CommandType.StoredProcedure, parameters);

            int Id = (int)idParameter.Value;

            // To Decrypt output parameter values you can use: operation.DecryptValue(MyStringOrBinaryObjectValue);
        }
    }
}
