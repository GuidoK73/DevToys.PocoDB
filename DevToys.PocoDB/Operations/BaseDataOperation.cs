using DevToys.PocoDB.Attributes;
using DevToys.PocoDB.Encryption;
using DevToys.PocoDB.Enums;
using DevToys.PocoDB.Factory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace DevToys.PocoDB.Operations
{ 
    public abstract class BaseDataOperation
    {
        /// <param name="configConnectionName">Points to ConnectionString Configuration in section DevToys.PocoDB in App.Config</param>
        protected BaseDataOperation(string configConnectionName) => Config = DataConfiguration.Instance.Get(configConnectionName);

        protected BaseDataOperation() => Config = null;

        protected BaseDataOperation(ConnectionConfig config) => Config = config;

        /// <param name="configConnectionName">Points to ConnectionString Configuration in section DevToys.PocoDB in App.Config</param>
        protected BaseDataOperation(DbConnectionStringBuilder connectionString, string configConnectionName) => Config = new ConnectionConfig() { ConnectionString = connectionString.ToString(), ConnectionType = configConnectionName };

        /// <summary>Raised prior to command execution. Use it to set provider specific properties for command and connection if nececary. </summary>
        public event EventHandler<DataOperationPreExecute> PreExecute = delegate { };

        protected ConnectionConfig Config { get; private set; }

        public DbConnection CreateConnection()
        {
            return ConnectionFactory.Instance.Create(Config.ConnectionType, Config.ConnectionString);
        }

        /// <summary>Call before invoking command.Execute etc. </summary>
        protected void RaisePreExecute(DbConnection connection, DbCommand command) => PreExecute?.Invoke(this, new DataOperationPreExecute() { Connection = connection, Command = command });
    }

    /// <typeparam name="TRESULTOBJECT">The Result Object Type either as single object.</typeparam>
    public abstract class BaseDataOperation<TRESULTOBJECT> : BaseDataOperation
        where TRESULTOBJECT : class, new()
    {
        private Dictionary<string, DBFieldAttribute> _Attributes = new Dictionary<string, DBFieldAttribute>();
        private bool _Initialized = false;
        private Dictionary<string, PropertyInfo> _Properties = new Dictionary<string, PropertyInfo>();

        /// <param name="configConnectionName">Reference to connection in DevToys.PocoDB config section</param>
        protected BaseDataOperation(string configConnectionName) : base(configConnectionName) { }

        /// <param name="config"></param>
        protected BaseDataOperation(ConnectionConfig config) : base(config) { }

        protected BaseDataOperation() : base()
        {
        }

        /// <param name="configConnectionName">Points to ConnectionString Configuration in section DevToys.PocoDB in App.Config</param>
        protected BaseDataOperation(DbConnectionStringBuilder connectionString, string configConnectionName) : base(connectionString, configConnectionName) { }

        protected TRESULTOBJECT ReadDataRow(DataTable table, DataRow row)
        {
            Init(table);

            // Create new object
            TRESULTOBJECT _result = new TRESULTOBJECT();

            foreach (KeyValuePair<string, PropertyInfo> kvp in _Properties)
            {
                DBFieldAttribute _attribute = _Attributes[kvp.Key];
                SetPropertyValue(kvp.Value, _result, row[kvp.Key], _attribute.ReaderDefaultValue, _attribute.StrictMapping, _attribute.Decrypt);
            }

            return _result;
        }

        /// <summary>Reads a datarow and converts it to TObject</summary>
        protected TRESULTOBJECT ReadDataRow(IDataReader reader)
        {
            Init(reader);

            // Create new object
            TRESULTOBJECT _result = new TRESULTOBJECT();
            // create a new base object so we can invoke base methods.

            foreach (KeyValuePair<string, PropertyInfo> kvp in _Properties)
            {
                DBFieldAttribute _attribute = _Attributes[kvp.Key];
                SetPropertyValue(kvp.Value, _result, reader[kvp.Key], _attribute.ReaderDefaultValue, _attribute.StrictMapping, _attribute.Decrypt);
            }
            return _result;
        }

        private void Init()
        {
            if (_Initialized)
                return;

            _Attributes = typeof(TRESULTOBJECT)
                .GetProperties().Where(p => p.GetCustomAttribute<DBFieldAttribute>() != null)
                .Select(p => new { Key = p.GetCustomAttribute<DBFieldAttribute>().Field, Value = p.GetCustomAttribute<DBFieldAttribute>() })
                .ToDictionary(p => p.Key, p => p.Value);

            _Properties = typeof(TRESULTOBJECT)
                .GetProperties().Where(p => p.GetCustomAttribute<DBFieldAttribute>() != null)
                .Select(p => new { Key = p.GetCustomAttribute<DBFieldAttribute>().Field, Value = p })
                .ToDictionary(p => p.Key, p => p.Value);
        }

        private void Init(DataTable datatable)
        {
            if (_Initialized)
                return;

            Init();

            List<string> _readerFieldNames = new List<string>();

            foreach (DataColumn column in datatable.Columns)
                _readerFieldNames.Add(column.ColumnName);

            InitValidateFieldNames(_readerFieldNames);

            _Initialized = true;
        }

        private void Init(IDataReader reader)
        {
            if (_Initialized)
                return;

            Init();

            List<string> _readerFieldNames = DataUtils.GetReaderColumns(reader);

            InitValidateFieldNames(_readerFieldNames);
            _Initialized = true;
        }

        private void InitValidateFieldNames(List<string> _readerFieldNames)
        {
            foreach (string column in _Properties.Keys)
            {
                if (_readerFieldNames.Where(p => p.Equals(column, StringComparison.OrdinalIgnoreCase)).FirstOrDefault() == null)
                {
                    string message = string.Format("Column '{0}' does not exist in the result DataSet.", column);
                    throw new DataException(message);
                }
            }
        }

        private void SetPropertyValue(PropertyInfo propertyInfo, TRESULTOBJECT dataobject, object value, object defaultvalue, StrictMapping strictField, bool fieldDecrypt)
        {
            value = FieldEncryption.Decrypt(value, Config.FieldEncryptionPasswordEncrypted, fieldDecrypt);

            if (value.GetType() == typeof(DBNull))
            {
                if (propertyInfo.PropertyType != typeof(string))
                {
                    if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) == null)
                        throw new DataException("Property {0} cannot contain null value", propertyInfo.Name);
                }

                propertyInfo.SetValue(dataobject, defaultvalue, null);
                return;
            }

            if (propertyInfo.PropertyType.IsEnum)
            {
                propertyInfo.SetValue(dataobject, Enum.Parse(propertyInfo.PropertyType, value.ToString()), null);
                return;
            }

            if ((Config.StrictMapping && strictField == StrictMapping.ByConfigSetting) || strictField == StrictMapping.True)
            {
                if (propertyInfo.PropertyType == value.GetType())
                    propertyInfo.SetValue(dataobject, value, null);
                else
                    throw new DataException("Property '{0}': Type cannot be mapped from '{1}' to '{2}' ", propertyInfo.Name, value.GetType(), propertyInfo.PropertyType);
            }
            else
            {
                if (propertyInfo.PropertyType == value.GetType())
                    propertyInfo.SetValue(dataobject, value, null);
                else
                    propertyInfo.SetValue(dataobject, Convert.ChangeType(value, propertyInfo.PropertyType, Config.CultureInfo), null);
            }
        }
    }
}