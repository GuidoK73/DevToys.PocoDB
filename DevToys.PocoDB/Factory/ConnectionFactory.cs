using System;
using System.Collections.Generic;
using System.Data.Common;


namespace DevToys.PocoDB.Factory
{
    /// <summary>
    /// Default Connections: SqlClient, OleDb, Odbc
    /// </summary>
    public sealed class ConnectionFactory
    {
        private static ConnectionFactory _Instance;

        private Dictionary<string, Type> connectionTypes = new Dictionary<string, Type>();

        private ConnectionFactory() => Init();

        public static ConnectionFactory Instance => _Instance ?? (_Instance = new ConnectionFactory());

        public DbConnection Create(string name, string connectionstring)
        {
            if (!connectionTypes.ContainsKey(name))
            {
                throw new DataException($"The ClientType '{name}' does not exist. Please check Connection in app.config. Or add the desired DBConnection to the Connection Factory with the given name: '{name}'");
            }

            return Activator.CreateInstance(connectionTypes[name], connectionstring) as DbConnection;
        }

        private void Init()
        {
            if (connectionTypes.Count > 0)
                return;

            ConnectionFactoryInitializer _connectionFactoryInitializer = new ConnectionFactoryInitializer();
            connectionTypes = _connectionFactoryInitializer.Init();
        }

        /// <param name="name">Name to use as reference ConnectionType in configuration.</param>
        public void AddType<TCONNECTION>(string name) where TCONNECTION : DbConnection
        {
            if (connectionTypes.ContainsKey(name))
                throw new DataException($"The ClientType '{name}' does already exist. Use another name for '{typeof(TCONNECTION).Name}'.");

            connectionTypes.Add(name, typeof(TCONNECTION));
        }
    }
}