using DevToys.PocoDB.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PocoDBConsoleAppTest.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
 
namespace DevToys.PocoDB.UnitTests
{
    [TestClass]
    public class TestCommandOperationBinaryData
    {
        [TestMethod]
        public void InsertBinary()
        {
            var operation = new DbCommandOperation<InsertPhoto>("Local");
            IEnumerable<InsertPhoto> commands = GetFiles().Select(p => new InsertPhoto() { Photo = File.ReadAllBytes(p.FullName), Name = p.Name });
            operation.ExecuteNonQuery(commands);
        }

        private string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private IEnumerable<FileInfo> GetFiles()
        {
            string path = AssemblyDirectory;
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            string photosDir = Path.Combine(directoryInfo.Parent.Parent.FullName, "Photos");
            return Directory.GetFiles(photosDir).ToList().Select(p => new FileInfo(p));
        }
    }
}