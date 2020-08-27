using DevToys.PocoDB.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PocoDBConsoleAppTest.Data;
using System.Collections.Generic;
using System.Linq;
 
namespace DevToys.PocoDB.UnitTests
{
    [TestClass]
    public class TestCommandOperation
    {
        [TestMethod]
        public void GetCompanyById()
        {
            var operation = new DbCommandOperation<Company, GetCompanyById>("Local");
            Company _result = operation.ExecuteSingleReader(new GetCompanyById() { Id = 1 });
        }

        [TestMethod]
        public void GetCompanyById_Sql()
        {
            var operation = new DbCommandOperation<Company, GetCompanyById_Sql>("Local");
            Company _result = operation.ExecuteSingleReader(new GetCompanyById_Sql() { Id = 1 });
        }

        [TestMethod]
        public void InsertCompany()
        {
            var operation = new DbCommandOperation<InsertCompanyByProcedure>("Local");

            InsertCompanyByProcedure parameters = new InsertCompanyByProcedure()
            {
                Adress = "",
                Country = "NLD",
                HouseNumber = "",
                Name = "A Company Name",
                ZipCode = "4624JC",
                CompanyType = CompanyType.NV,
                EncryptedText = "Guido Kleijer2"
            };

            operation.ExecuteNonQuery(parameters);

            int newId = parameters.Id;
        }


        [TestMethod]
        public void InsertCompany2()
        {
            var operation = new DbCommandOperation<InsertCompanyBySqlStatement>("Local");

            InsertCompanyBySqlStatement parameters = new InsertCompanyBySqlStatement()
            {
                Adress = "",
                Country = "NLD",
                HouseNumber = "",
                Name = "A Company Name",
                ZipCode = "4624JC",
                CompanyType = CompanyType.NV,
                EncryptedText = "Guido Kleijer"
            };

            operation.ExecuteNonQuery(parameters);

            int newId = parameters.Id;
        }


        [TestMethod]
        public void SelectAll()
        {
            var operation = new DbCommandOperation<Company, GetCompanyAll>("Local");
            IEnumerable<Company> _result = operation.ExecuteReader(new GetCompanyAll() { });

            var _resultMaterialized = _result.ToList();
        }

        [TestMethod]
        public void TestDBStringArrayParameter()
        {
            var operation = new DbCommandOperation<Company, GetCompanies>("Local");
            var parameters = new GetCompanies() { Id = new List<int> { 1, 3, 6, 9 } };
            IEnumerable<Company> _result = operation.ExecuteReader(parameters);

            var _resultMaterialized = _result.ToList();
        }

        [TestMethod]
        public void TestDBStringArrayParameter2()
        {
            var operation = new DbCommandOperation<Company, GetCompanies2>("Local");
            var parameters = new GetCompanies2() { Id = new int[] { 1, 3, 6, 9 } };
            IEnumerable<Company> _result = operation.ExecuteReader(parameters);

            var _resultMaterialized = _result.ToList();
        }
    }
}