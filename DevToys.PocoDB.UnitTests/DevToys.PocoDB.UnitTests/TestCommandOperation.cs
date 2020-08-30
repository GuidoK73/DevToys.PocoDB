using DevToys.PocoDB.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PocoDBConsoleAppTest.Data;
using System;
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
            // NOTE:
            // Each construction of DbCommandOperation means it does initializion on first call.
            // when using multiple times it's best to set the declaration on class level instead of method level.

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
                HouseNumber = "181",
                Name = "A Company Name",
                ZipCode = "4624JC",
                CompanyType = CompanyType.NV,
                Text = "My Name 2"
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
                HouseNumber = "180",
                Name = "A Company Name",
                ZipCode = "4624JC",
                CompanyType = CompanyType.NV,
                Text = "My Name"
            };

            operation.ExecuteNonQuery(parameters);

            int newId = parameters.Id;
        }


        [TestMethod]
        public void SelectAll()
        {
            var operation = new DbCommandOperation<Company, GetCompanyAll>("Local");
            IEnumerable<Company> _result = operation.ExecuteReader(new GetCompanyAll() { });

            StopWatch _watch = new StopWatch();

            _watch.Start();

            var _resultMaterialized = _result.ToList();

            _watch.Stop();

            Console.WriteLine(_watch.Duration);
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