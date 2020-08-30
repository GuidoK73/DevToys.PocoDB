using DevToys.PocoDB.Enums;
using DevToys.PocoDB.Factory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace DevToys.PocoDB.Operations
{
    /// <summary>
    /// Class for executing NonQuery or Scalar sql statements. Parameters are parsed by DbCommandOperation decorated with DBCommandAttribute and DBParameterAttribute attributes.
    /// 
    /// Note: Initialization occurs on each first call after object creation, in case of multiple execution declare on class level.
    /// </summary>
    /// <typeparam name="TCOMMAND">Object defining Procedure Call with or without parameters</typeparam>
    public sealed class DbCommandOperation<TCOMMAND> : BaseDataOperation where TCOMMAND : class
    {
        private DbCommandOperationHelper<TCOMMAND> _Helper;

        /// <param name="configConnectionName">Points to ConnectionString Configuration in section DevToys.PocoDB in App.Config</param>
        public DbCommandOperation(string configConnectionName) : base(configConnectionName)
        { }

        /// <param name="config">Use in memory created configuration instead of using App.Config declaration.</param>
        public DbCommandOperation(ConnectionConfig config) : base(config)
        { }

        /// <param name="configConnectionName">Points to ConnectionString Configuration in section DevToys.PocoDB in App.Config</param>
        public DbCommandOperation(DbConnectionStringBuilder connectionString, string configConnectionName) : base(connectionString, configConnectionName)
        { }

        /// <summary>
        /// When CommandOperation does not have any parameters
        /// </summary>
        public void ExecuteNonQuery()
        {
            Type _type = typeof(TCOMMAND);
            TCOMMAND _parameters = (TCOMMAND)Activator.CreateInstance(_type);
            ExecuteNonQuery(_parameters);
        }

        /// <summary>
        /// When CommandOperation does not have any parameters
        /// </summary>
        public void ExecuteNonQuery(DbConnection connection, DbTransaction transaction)
        {
            Type _type = typeof(TCOMMAND);
            TCOMMAND _parameters = (TCOMMAND)Activator.CreateInstance(_type);
            ExecuteNonQuery(connection, transaction, _parameters);
        }

        public void ExecuteNonQuery(IEnumerable<TCOMMAND> commands)
        {
            foreach (TCOMMAND command in commands)
                ExecuteNonQuery(command);
        }

        public void ExecuteNonQuery(DbConnection connection, DbTransaction transaction, IEnumerable<TCOMMAND> commands)
        {
            foreach (TCOMMAND command in commands)
                ExecuteNonQuery(connection, transaction, command);
        }

        public void ExecuteNonQuery(TCOMMAND commandObject) => Execute(commandObject, ExecType.NonQuery);

        public void ExecuteNonQuery(DbConnection connection, DbTransaction transaction, TCOMMAND commandObject) => Execute(connection, transaction, commandObject, ExecType.NonQuery);

        public object ExecuteScalar(TCOMMAND commandObject) => Execute(commandObject, ExecType.Scalar);

        public object ExecuteScalar(DbConnection connection, DbTransaction transaction, TCOMMAND commandObject) => Execute(connection, transaction, commandObject, ExecType.Scalar);

        private object Execute(TCOMMAND commandObject, ExecType execType)
        {
            Init();

            object _result = null;

            using (DbConnection connection = ConnectionFactory.Instance.Create(Config.ConnectionType, Config.ConnectionString))
            {
                connection.Open();
                _result = Execute(connection, null, commandObject, execType);
                connection.Close();
            }
            return _result;
        }

        private object Execute(DbConnection connection, DbTransaction transaction, TCOMMAND commandObject, ExecType execType)
        {
            Init();

            object _result = null;

            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = _Helper.CommandAttribute.CommandText;
                command.CommandType = _Helper.CommandAttribute.CommandType;
                command.CommandTimeout = _Helper.CommandAttribute.CommandTimeout;
                command.Transaction = transaction;

                _Helper.SetParameters(command, commandObject);

                RaisePreExecute(connection, command);

                if (execType == ExecType.Scalar)
                    _result = command.ExecuteScalar();
                else
                    command.ExecuteNonQuery();

                _Helper.GetParameters(command, commandObject);
            }
            return _result;
        }

        private void Init()
        {
            if (_Helper != null)
                return;

            _Helper = new DbCommandOperationHelper<TCOMMAND>(Config);
            _Helper.Initialize();
        }
    }

    /// <summary>
    /// Class for executing Reader or SingleReader procedures. Parameters are parsed by DbCommandOperation decorated with DBCommandAttribute and DBParameterAttribute attributes.
    /// ResultValues are based on TObject decorated with DBTableAttribute and DBFieldAttribute attributes.
    /// 
    /// Note: Initialization occurs on each first call after object creation, in case of multiple execution declare on class level.
    /// </summary>
    /// <typeparam name="TRESULTOBJECT">The Result Object Type either as enumarable or single object.</typeparam>
    /// <typeparam name="TCOMMAND">Object defining Procedure Call with or without parameters</typeparam>
    public sealed class DbCommandOperation<TRESULTOBJECT, TCOMMAND> : BaseDataOperation<TRESULTOBJECT> where TRESULTOBJECT : class, new() where TCOMMAND : class
    {
        private DbCommandOperationHelper<TCOMMAND> _Helper;

        /// <param name="connectionname">Reference to connection in DevToys.PocoDB config section</param>
        public DbCommandOperation(string configConnectionName) : base(configConnectionName) { }

        /// <param name="config"></param>
        public DbCommandOperation(ConnectionConfig config) : base(config) { }

        /// <param name="configConnectionName">Points to ConnectionString Configuration in section DevToys.PocoDB in App.Config</param>
        public DbCommandOperation(DbConnectionStringBuilder connectionString, string configConnectionName) : base(connectionString, configConnectionName) { }

        /// <summary>
        /// For procedures without any parameters.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TRESULTOBJECT> ExecuteReader()
        {
            Type _type = typeof(TCOMMAND);
            TCOMMAND _parameters = (TCOMMAND)Activator.CreateInstance(_type);
            return ExecuteReader(_parameters);
        }

        public IEnumerable<TRESULTOBJECT> ExecuteReader(DbConnection connection, DbTransaction transaction)
        {
            Type _type = typeof(TCOMMAND);
            TCOMMAND _parameters = (TCOMMAND)Activator.CreateInstance(_type);
            return ExecuteReader(connection, transaction, _parameters);
        }

        /// <summary>
        /// Exexutes a procedure, parameters are specified by DbCommandOperation object marked with DBCommandAttribute and DBParameterAttributes
        /// returns TObject, TObject must be marked with DBFieldAttributes and must match the procedure result.
        /// </summary>
        /// <param name="commandName">Reference to attribute's commandName.</param>
        public IEnumerable<TRESULTOBJECT> ExecuteReader(TCOMMAND commandObject)
        {
            using (DbConnection connection = ConnectionFactory.Instance.Create(Config.ConnectionType, Config.ConnectionString))
            {
                connection.Open();
                var _resultSet = ExecuteReader(connection, null, commandObject);
                foreach (TRESULTOBJECT result in _resultSet)
                    yield return result;

                connection.Close();
            }
        }

        public IEnumerable<TRESULTOBJECT> ExecuteReader(DbConnection connection, DbTransaction transaction, TCOMMAND commandObject)
        {
            Init();

            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = _Helper.CommandAttribute.CommandText;
                command.CommandType = _Helper.CommandAttribute.CommandType;
                command.CommandTimeout = _Helper.CommandAttribute.CommandTimeout;
                command.Transaction = transaction;

                _Helper.SetParameters(command, commandObject);

                RaisePreExecute(connection, command);

                IDataReader _reader = command.ExecuteReader();

                _Helper.GetParameters(command, commandObject);

                while (_reader.Read())
                {
                    TRESULTOBJECT _dataobject = ReadDataRow(_reader);
                    yield return _dataobject; // returns only when requested by ienumerable.
                }
            }
        }

        /// <summary>
        /// Exexutes a procedure, parameters are specified by DbCommandOperation object marked with DBCommandAttribute and DBParameterAttributes
        /// returns TObject, TObject must be marked with DBFieldAttributes and must match the procedure result.
        /// </summary>
        /// <param name="commandObject"></param>
        public TRESULTOBJECT ExecuteSingleReader(TCOMMAND commandObject) => ExecuteReader(commandObject).FirstOrDefault();

        public TRESULTOBJECT ExecuteSingleReader(DbConnection connection, DbTransaction transaction, TCOMMAND commandObject) => ExecuteReader(connection, transaction, commandObject).FirstOrDefault();

        private void Init()
        {
            if (_Helper != null)
                return;

            _Helper = new DbCommandOperationHelper<TCOMMAND>(Config);
            _Helper.Initialize();
        }
    }
}