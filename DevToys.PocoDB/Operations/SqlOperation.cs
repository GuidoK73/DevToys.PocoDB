using DevToys.PocoDB.Enums;
using DevToys.PocoDB.Factory;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace DevToys.PocoDB.Operations
{
    /// <summary>
    /// Class for executing sql statements.
    /// </summary>
    public sealed class SqlOperation : BaseDataOperation
    {
        /// <param name="configConnectionName">Points to ConnectionString Configuration in section DevToys.PocoDB in App.Config</param>
        public SqlOperation(string configConnectionName) : base(configConnectionName)
        { }

        /// <param name="config">Use in memory created configuration instead of using App.Config declaration.</param>
        public SqlOperation(ConnectionConfig config) : base(config)
        { }

        /// <param name="configConnectionName">Points to ConnectionString Configuration in section DevToys.PocoDB in App.Config</param>
        public SqlOperation(DbConnectionStringBuilder connectionString, string configConnectionName) : base(connectionString, configConnectionName)
        { }

        #region Non Transaction

        public void ExecuteNonQuery(string commandText) => ExecuteNonQuery(commandText, null);

        public void ExecuteNonQuery(string commandText, params IDbDataParameter[] parameters) => Execute(commandText, CommandType.Text, ExecType.NonQuery, parameters);

        public void ExecuteNonQuery(string commandText, CommandType commandType, params IDbDataParameter[] parameters) => Execute(commandText, commandType, ExecType.NonQuery, parameters);

        public object ExecuteScalar(string commandText, CommandType commandType) => ExecuteScalar(commandText, commandType, null);

        public T ExecuteScalar<T>(string commandText, CommandType commandType) => (T)ExecuteScalar(commandText, commandType, null);

        public object ExecuteScalar(string commandText) => ExecuteScalar(commandText, CommandType.Text, null);

        public T ExecuteScalar<T>(string commandText) => (T)ExecuteScalar(commandText, CommandType.Text, null);

        public object ExecuteScalar(string commandText, params IDbDataParameter[] parameters) => Execute(commandText, CommandType.Text, ExecType.Scalar, parameters);

        public object ExecuteScalar(string commandText, CommandType commandType, params IDbDataParameter[] parameters) => Execute(commandText, commandType, ExecType.Scalar, parameters);

        public T ExecuteScalar<T>(string commandText, params IDbDataParameter[] parameters) => (T)ExecuteScalar(commandText, parameters);

        public T ExecuteScalar<T>(string commandText, CommandType commandType, params IDbDataParameter[] parameters) => (T)ExecuteScalar(commandText, commandType, parameters);

        private object Execute(string commandText, CommandType commandType, ExecType execType, params IDbDataParameter[] parameters)
        {
            using (DbConnection connection = ConnectionFactory.Instance.Create(Config.ConnectionType, Config.ConnectionString))
            {
                connection.Open();
                object _result = null;
                using (DbCommand command = connection.CreateCommand())
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    command.CommandType = commandType;
                    command.CommandText = commandText;

                    RaisePreExecute(connection, command);

                    if (execType == ExecType.Scalar)
                        _result = command.ExecuteScalar();
                    else
                        command.ExecuteNonQuery();
                }
                connection.Close();
                return _result;
            }
        }

        #endregion

        #region Transactional

        public void ExecuteNonQuery(DbConnection connection, DbTransaction transaction, string commandText) => ExecuteNonQuery(connection, transaction, commandText, null);

        public void ExecuteNonQuery(DbConnection connection, DbTransaction transaction, string commandText, params IDbDataParameter[] parameters) => Execute(connection, transaction, commandText, CommandType.Text, ExecType.NonQuery, parameters);

        public void ExecuteNonQuery(DbConnection connection, DbTransaction transaction, string commandText, CommandType commandType, params IDbDataParameter[] parameters) => Execute(connection, transaction, commandText, commandType, ExecType.NonQuery, parameters);

        public object ExecuteScalar(DbConnection connection, DbTransaction transaction, string commandText, CommandType commandType) => ExecuteScalar(connection, transaction, commandText, commandType, null);

        public T ExecuteScalar<T>(DbConnection connection, DbTransaction transaction, string commandText, CommandType commandType) => (T)ExecuteScalar(connection, transaction, commandText, commandType, null);

        public object ExecuteScalar(DbConnection connection, DbTransaction transaction, string commandText) => ExecuteScalar(connection, transaction, commandText, CommandType.Text, null);

        public T ExecuteScalar<T>(DbConnection connection, DbTransaction transaction, string commandText) => (T)ExecuteScalar(connection, transaction, commandText, CommandType.Text, null);

        public object ExecuteScalar(DbConnection connection, DbTransaction transaction, string commandText, params IDbDataParameter[] parameters) => Execute(connection, transaction, commandText, CommandType.Text, ExecType.Scalar, parameters);

        public object ExecuteScalar(DbConnection connection, DbTransaction transaction, string commandText, CommandType commandType, params IDbDataParameter[] parameters) => Execute(connection, transaction, commandText, commandType, ExecType.Scalar, parameters);

        public T ExecuteScalar<T>(DbConnection connection, DbTransaction transaction, string commandText, params IDbDataParameter[] parameters) => (T)ExecuteScalar(connection, transaction, commandText, parameters);

        public T ExecuteScalar<T>(DbConnection connection, DbTransaction transaction, string commandText, CommandType commandType, params IDbDataParameter[] parameters) => (T)ExecuteScalar(connection, transaction, commandText, commandType, parameters);

        private object Execute(DbConnection connection, DbTransaction transaction, string commandText, CommandType commandType, ExecType execType, params IDbDataParameter[] parameters)
        {
            object _result = null;
            using (DbCommand command = connection.CreateCommand())
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                command.CommandType = commandType;
                command.CommandText = commandText;
                command.Transaction = transaction;

                RaisePreExecute(connection, command);

                if (execType == ExecType.Scalar)
                    _result = command.ExecuteScalar();
                else
                    command.ExecuteNonQuery();
            }
            return _result;
        }

        #endregion

    }

    /// <summary>
    /// Class for executing sql statements and mapping them to objects.
    /// 
    /// Note: Initialization occurs on each first call after object creation, in case of multiple execution declare on class level.
    /// </summary>
    public sealed class SqlOperation<TRESULTOBJECT> : BaseDataOperation<TRESULTOBJECT>
        where TRESULTOBJECT : class, new()
    {
        /// <param name="connectionTypeName">Points to ConnectionString Configuration in section DevToys.PocoDB in App.Config</param>
        public SqlOperation(string connectionTypeName) : base(connectionTypeName)
        { }

        /// <param name="connectionTypeName">Points to ConnectionString Configuration in section DevToys.PocoDB in App.Config</param>
        public SqlOperation(DbConnectionStringBuilder connectionString, string connectionTypeName) : base(connectionString, connectionTypeName)
        { }

        #region Non Transactional

        public IEnumerable<TRESULTOBJECT> ExecuteReader(string commandText) => ExecuteReader(commandText, CommandType.Text, null);

        public IEnumerable<TRESULTOBJECT> ExecuteReader(string commandText, CommandType commandType, params IDbDataParameter[] parameters)
        {
            using (DbConnection connection = ConnectionFactory.Instance.Create(Config.ConnectionType, Config.ConnectionString))
            {
                connection.Open();
                using (DbCommand command = connection.CreateCommand())
                {
                    var _resultSet = ExecuteReader(connection, null, commandText, commandType, parameters);
                    foreach (TRESULTOBJECT result in _resultSet)
                        yield return result;
                }
                connection.Close();
            }
        }

        public TRESULTOBJECT ExecuteSingleReader(string commandText) => ExecuteSingleReader(commandText, null);

        public TRESULTOBJECT ExecuteSingleReader(string commandText, params IDbDataParameter[] parameters) => ExecuteReader(commandText, CommandType.Text, parameters).FirstOrDefault();

        #endregion

        #region Transactional

        public IEnumerable<TRESULTOBJECT> ExecuteReader(DbConnection connection, DbTransaction transaction, string commandText) => ExecuteReader(connection, transaction, commandText, CommandType.Text, null);

        public IEnumerable<TRESULTOBJECT> ExecuteReader(DbConnection connection, DbTransaction transaction, string commandText, CommandType commandType, params IDbDataParameter[] parameters)
        {
            using (DbCommand command = connection.CreateCommand())
            {
                if (parameters != null)
                    command.Parameters.AddRange(parameters);

                command.CommandText = commandText;
                command.CommandType = commandType;
                command.Transaction = transaction;

                RaisePreExecute(connection, command);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TRESULTOBJECT dataobject = ReadDataRow(reader);
                        yield return dataobject; // returns only when requested by ienumerable.
                    }
                }
            }
        }

        public TRESULTOBJECT ExecuteSingleReader(DbConnection connection, DbTransaction transaction, string commandText) => ExecuteSingleReader(connection, transaction, commandText, null);

        public TRESULTOBJECT ExecuteSingleReader(DbConnection connection, DbTransaction transaction, string commandText, params IDbDataParameter[] parameters) => ExecuteReader(connection, transaction, commandText, CommandType.Text, parameters).FirstOrDefault();

        #endregion

    }
}