using DevToys.PocoDB.Enums;
using DevToys.PocoDB.Factory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace DevToys.PocoDB.Operations
{
    /// <summary>
    /// Class for executing sql statements and mapping them to dynamic ExpandoObjects.
    /// All property names will be cleaned from non letters.
    /// underscores are kept, spaces changed to underscore!.
    ///
    /// Limitations:
    /// -    Output Parameters cannot be retrieved from procedures.
    /// </summary>
    /// <typeparam name=""></typeparam>
    public sealed class DynamicSqlOperation : BaseDataOperation
    {
        private string[] _ReaderColumns = null;

        /// <param name="configConnectionName">Points to ConnectionString Configuration in section DevToys.PocoDB in App.Config</param>
        public DynamicSqlOperation(string configConnectionName) : base(configConnectionName) { }

        /// <param name="config">Use in memory created configuration instead of using App.Config declaration.</param>
        public DynamicSqlOperation(ConnectionConfig config) : base(config) { }

        /// <param name="configConnectionName">Points to ConnectionString Configuration in section DevToys.PocoDB in App.Config</param>
        public DynamicSqlOperation(DbConnectionStringBuilder connectionString, string configConnectionName) : base(connectionString, configConnectionName) { }

        public void ExecuteNonQuery(string sql, dynamic parameters, CommandType commandType) => Execute(sql, parameters, ExecType.NonQuery, commandType, 30);

        public void ExecuteNonQuery(string sql, dynamic parameters, CommandType commandType, int commandTimeOut) => Execute(sql, parameters, ExecType.NonQuery, commandType, commandTimeOut);

        public void ExecuteNonQuery(DbConnection connection, DbTransaction transaction, string sql, dynamic parameters, CommandType commandType) => Execute(connection, transaction, sql, parameters, ExecType.NonQuery, commandType, 30);

        public void ExecuteNonQuery(DbConnection connection, DbTransaction transaction, string sql, dynamic parameters, CommandType commandType, int commandTimeOut) => Execute(connection, transaction, sql, parameters, ExecType.NonQuery, commandType, commandTimeOut);

        public IEnumerable<dynamic> ExecuteReader(string commandText) => ExecuteReader(commandText, CommandType.Text, 30, new IDbDataParameter[0]);

        public IEnumerable<dynamic> ExecuteReader(string commandText, CommandType commandtype) => ExecuteReader(commandText, commandtype, 30, new IDbDataParameter[0]);

        public IEnumerable<dynamic> ExecuteReader(string commandText, CommandType commandtype, int commandtimeout) => ExecuteReader(commandText, commandtype, commandtimeout, new IDbDataParameter[0]);

        public IEnumerable<dynamic> ExecuteReader(string commandText, CommandType commandtype, int commandtimeout, params IDbDataParameter[] parameters)
        {
            using (DbConnection connection = ConnectionFactory.Instance.Create(Config.ConnectionType, Config.ConnectionString))
            {
                connection.Open();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandType = commandtype;
                    command.CommandText = commandText;
                    command.CommandTimeout = commandtimeout;

                    if (parameters != null)
                        command.Parameters.AddRange(parameters);

                    RaisePreExecute(connection, command);

                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dynamic dataobject = ReadDataRow(reader);
                            yield return dataobject; // returns only when requested by ienumerable.
                        }
                    }
                }
                connection.Close();
            }
        }

        /// <summary>
        /// Reads the sql into a list of T objects
        /// Mapping will be done by DBField attributes or by overriding the ReadDataRow method.
        /// </summary>
        /// <param name="commandText">script to execute</param>
        /// <param name="parameters">dynamic parameters will be translated to DbParameter</param>
        public IEnumerable<dynamic> ExecuteReader(string commandText, CommandType commandtype, int commandtimeout, ExpandoObject parameters)
        {
            using (DbConnection connection = ConnectionFactory.Instance.Create(Config.ConnectionType, Config.ConnectionString))
            {
                connection.Open();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandType = commandtype;
                    command.CommandText = commandText;
                    command.CommandTimeout = commandtimeout;

                    SetParameters(command, parameters);

                    RaisePreExecute(connection, command);

                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dynamic dataobject = ReadDataRow(reader);
                            yield return dataobject; // returns only when requested by ienumerable.
                        }
                    }

                    //GetOutputParameters(command, parameters);
                }
                connection.Close();
            }
        }

        public IEnumerable<dynamic> ExecuteReader(DbConnection connection, DbTransaction transaction, string commandText) => ExecuteReader(connection, transaction, commandText, CommandType.Text, 30, new IDbDataParameter[0]);

        public IEnumerable<dynamic> ExecuteReader(DbConnection connection, DbTransaction transaction, string commandText, CommandType commandtype) => ExecuteReader(connection, transaction, commandText, commandtype, 30, new IDbDataParameter[0]);

        public IEnumerable<dynamic> ExecuteReader(DbConnection connection, DbTransaction transaction, string commandText, CommandType commandtype, int commandtimeout) => ExecuteReader(connection, transaction, commandText, commandtype, commandtimeout, new IDbDataParameter[0]);

        public IEnumerable<dynamic> ExecuteReader(DbConnection connection, DbTransaction transaction, string commandText, CommandType commandtype, int commandtimeout, params IDbDataParameter[] parameters)
        {
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandType = commandtype;
                command.CommandText = commandText;
                command.CommandTimeout = commandtimeout;
                command.Transaction = transaction;

                if (parameters != null)
                    command.Parameters.AddRange(parameters);

                RaisePreExecute(connection, command);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dynamic dataobject = ReadDataRow(reader);
                        yield return dataobject; // returns only when requested by ienumerable.
                    }
                }
            }
        }

        public object ExecuteScalar(string sql, dynamic parameters, CommandType commandType) => Execute(sql, parameters, ExecType.Scalar, commandType, 30);

        public object ExecuteScalar(string sql, dynamic parameters, CommandType commandType, int commandTimeOut) => Execute(sql, parameters, ExecType.Scalar, commandType, commandTimeOut);

        public object ExecuteScalar(DbConnection connection, DbTransaction transaction, string sql, dynamic parameters, CommandType commandType) => Execute(connection, transaction, sql, parameters, ExecType.Scalar, commandType, 30);

        public object ExecuteScalar(DbConnection connection, DbTransaction transaction, string sql, dynamic parameters, CommandType commandType, int commandTimeOut) => Execute(connection, transaction, sql, parameters, ExecType.Scalar, commandType, commandTimeOut);

        public dynamic ExecuteSingleReader(string commandText) => ExecuteSingleReader(commandText, CommandType.Text, 30, null);

        public dynamic ExecuteSingleReader(string commandText, CommandType commandtype) => ExecuteSingleReader(commandText, commandtype, 30, null);

        public dynamic ExecuteSingleReader(string commandText, CommandType commandtype, int commandtimeout) => ExecuteSingleReader(commandText, commandtype, commandtimeout, null);

        public dynamic ExecuteSingleReader(string commandText, CommandType commandtype, int commandtimeout, params IDbDataParameter[] parameters) => ExecuteReader(commandText, commandtype, commandtimeout, parameters).FirstOrDefault();

        public dynamic ExecuteSingleReader(DbConnection connection, DbTransaction transaction, string commandText) => ExecuteSingleReader(connection, transaction, commandText, CommandType.Text, 30, null);

        public dynamic ExecuteSingleReader(DbConnection connection, DbTransaction transaction, string commandText, CommandType commandtype) => ExecuteSingleReader(connection, transaction, commandText, commandtype, 30, null);

        public dynamic ExecuteSingleReader(DbConnection connection, DbTransaction transaction, string commandText, CommandType commandtype, int commandtimeout) => ExecuteSingleReader(connection, transaction, commandText, commandtype, commandtimeout, null);

        public dynamic ExecuteSingleReader(DbConnection connection, DbTransaction transaction, string commandText, CommandType commandtype, int commandtimeout, params IDbDataParameter[] parameters) => ExecuteReader(connection, transaction, commandText, commandtype, commandtimeout, parameters).FirstOrDefault();

        private object Execute(string sql, dynamic parameters, ExecType execType, CommandType commandType, int commandTimeOut)
        {
            object _result = null;

            using (DbConnection connection = ConnectionFactory.Instance.Create(Config.ConnectionType, Config.ConnectionString))
            {
                connection.Open();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.CommandType = commandType;
                    command.CommandTimeout = commandTimeOut;

                    SetParameters(command, parameters);

                    RaisePreExecute(connection, command);

                    if (execType == ExecType.Scalar)
                        _result = command.ExecuteScalar();
                    else
                        command.ExecuteNonQuery();
                }
                connection.Close();
            }
            return _result;
        }

        private object Execute(DbConnection connection, DbTransaction transaction, string sql, dynamic parameters, ExecType execType, CommandType commandType, int commandTimeOut)
        {
            object _result = null;

            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.CommandType = commandType;
                command.CommandTimeout = commandTimeOut;
                command.Transaction = transaction;

                SetParameters(command, parameters);

                RaisePreExecute(connection, command);

                if (execType == ExecType.Scalar)
                    _result = command.ExecuteScalar();
                else
                    command.ExecuteNonQuery();
            }
            return _result;
        }

        /// <summary>
        /// Initializes the instance.
        /// </summary>
        private void Init(IDataReader reader)
        {
            if (_ReaderColumns != null)
                return;

            _ReaderColumns = DataUtils.GetReaderColumns(reader);
        }

        /// <summary>
        /// Reads a datarow and converts it to TObject
        /// </summary>
        private dynamic ReadDataRow(IDataReader reader)
        {
            Init(reader);
            // Create new object
            dynamic dataobject;
            // create a new base object so we can invoke base methods.
            var dataobject2 = new ExpandoObject();
            var dataobject3 = dataobject2 as IDictionary<string, Object>;
            foreach (string name in _ReaderColumns)
                dataobject3.Add(DataUtils.CleanString(name), reader[name]);

            dataobject = dataobject3;

            return dataobject;
        }

        private void SetParameters(DbCommand command, dynamic parameters)
        {
            if (parameters != null)
            {
                foreach (PropertyInfo property in parameters.GetType().GetProperties())
                {
                    DbParameter _dbParam = command.CreateParameter();
                    _dbParam.ParameterName = property.Name;
                    _dbParam.Value = property.GetValue(parameters, null);
                    command.Parameters.Add(_dbParam);
                }
            }
        }
    }
}