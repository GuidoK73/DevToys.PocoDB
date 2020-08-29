using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace DevToys.PocoDB.Factory
{
    internal class ConnectionFactoryInitializer
    {
        public ConnectionFactoryInitializer() { }
        public Dictionary<string, Type> Init()
        {
            var _types = new Dictionary<string, Type>
            {
                { ConnectionFactoryDefaultTypes.SqlClient, typeof(SqlConnection) },
                { ConnectionFactoryDefaultTypes.OleDb, typeof(OleDbConnection) },
                { ConnectionFactoryDefaultTypes.Odbc, typeof(OdbcConnection) }
            };

            // Add new types for other DB Drivers here!
            return _types;
        }
    }
}
