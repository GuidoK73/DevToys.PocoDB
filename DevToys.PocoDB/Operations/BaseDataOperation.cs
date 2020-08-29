using DevToys.PocoDB.Attributes;
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
        private DBFieldAttribute[] _Attributes;
        private bool _Initialized = false;

        //private Dictionary<string, DBFieldAttribute> _Attributes = new Dictionary<string, DBFieldAttribute>();
        //private Dictionary<string, PropertyInfo> _Properties = new Dictionary<string, PropertyInfo>();
        private PropertyInfo[] _Properties;

        /// <param name="configConnectionName">Reference to connection in DevToys.PocoDB config section</param>
        protected BaseDataOperation(string configConnectionName) : base(configConnectionName) { }

        /// <param name="config"></param>
        protected BaseDataOperation(ConnectionConfig config) : base(config) { }

        protected BaseDataOperation() : base()
        {
        }

        /// <param name="configConnectionName">Points to ConnectionString Configuration in section DevToys.PocoDB in App.Config</param>
        protected BaseDataOperation(DbConnectionStringBuilder connectionString, string configConnectionName) : base(connectionString, configConnectionName) { }

        /// <summary>Reads a datarow and converts it to TObject</summary>
        protected TRESULTOBJECT ReadDataRow(IDataReader reader)
        {
            Init(reader);

            // Create new object
            TRESULTOBJECT _result = new TRESULTOBJECT();
            // create a new base object so we can invoke base methods.
            for (int ii = 0; ii < reader.FieldCount; ii++)
            {
                var _attribute = _Attributes[ii];
                var _property = _Properties[ii];
                SetPropertyValue(_property, _result, reader[ii], _attribute.ReaderDefaultValue, _attribute.StrictMapping);
            }

            return _result;
        }

        private void Init(IDataReader reader)
        {
            if (_Initialized)
                return;

            string[] _readerFieldNames = DataUtils.GetReaderColumnsArray(reader);

            Dictionary<string, DBFieldAttribute> _attributes = typeof(TRESULTOBJECT)
                    .GetProperties().Where(p => p.GetCustomAttribute<DBFieldAttribute>() != null)
                    .Select(p => new { Key = p.GetCustomAttribute<DBFieldAttribute>().Field.ToLower(), Value = p.GetCustomAttribute<DBFieldAttribute>() })
                    .ToDictionary(p => p.Key, p => p.Value);

            Dictionary<string, PropertyInfo> _properties = typeof(TRESULTOBJECT)
                .GetProperties().Where(p => p.GetCustomAttribute<DBFieldAttribute>() != null)
                .Select(p => new { Key = p.GetCustomAttribute<DBFieldAttribute>().Field.ToLower(), Value = p })
                .ToDictionary(p => p.Key, p => p.Value);

            InitValidateFieldNames(_readerFieldNames, _properties);

            _Attributes = new DBFieldAttribute[_readerFieldNames.Length];
            _Properties = new PropertyInfo[_readerFieldNames.Length];

            for (int ii = 0; ii < _readerFieldNames.Length; ii++)
            {
                string _name = _readerFieldNames[ii].ToLower();
                _Attributes[ii] = _attributes[_name];
                _Properties[ii] = _properties[_name];
            }

            _Initialized = true;
        }

        private void InitValidateFieldNames(string[] _readerFieldNames, Dictionary<string, PropertyInfo> properties)
        {
            foreach (string column in properties.Keys)
            {
                if (_readerFieldNames.Where(p => p.ToLower().Equals(column, StringComparison.OrdinalIgnoreCase)).FirstOrDefault() == null)
                {
                    string message = string.Format("Column '{0}' does not exist in the result DataSet.", column);
                    throw new DataException(message);
                }
            }
        }

        private void SetPropertyValue(PropertyInfo propertyInfo, TRESULTOBJECT dataobject, object value, object defaultvalue, StrictMapping strictField)
        {
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