using System;

namespace DevToys.PocoDB.Attributes
{
    /// <summary>
    /// For bulk insert operations Table name and Schema name are case sensitive!.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DBBulkInsertAttribute : Attribute
    {
        public DBBulkInsertAttribute(string tablename)
        {
            TableName = tablename;
        }

        public string TableName { get; private set; }

        public override string ToString() => TableName;
    }
}