using DevToys.PocoDB.Attributes;
using DevToys.PocoDB.Enums;
using DevToys.PocoDB.Factory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
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

        public DbConnection CreateConnection() => ConnectionFactory.Instance.Create(Config.ConnectionType, Config.ConnectionString);

        /// <summary>Call before invoking command.Execute etc. </summary>
        protected void RaisePreExecute(DbConnection connection, DbCommand command) => PreExecute?.Invoke(this, new DataOperationPreExecute() { Connection = connection, Command = command });
    }

    /// <typeparam name="TRESULTOBJECT">The Result Object Type either as single object.</typeparam>
    public abstract class BaseDataOperation<TRESULTOBJECT> : BaseDataOperation
        where TRESULTOBJECT : class, new()
    {
        private DBFieldAttribute[] _Attributes;
        private bool _Initialized = false;
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
            var _result = new TRESULTOBJECT();
            // create a new base object so we can invoke base methods.
            for (int index = 0; index < reader.FieldCount; index++)
                SetPropertyValue(_Properties[index], _result, reader.GetValue(index), reader.GetFieldType(index), _Attributes[index].ReaderDefaultValue, _Attributes[index].StrictMapping);

            return _result;
        }

        private void Init(IDataReader reader)
        {
            if (_Initialized)
                return;

            // Get Reader Column names ordered by ordinal
            string[] _readerFieldNames = DataUtils.GetReaderColumns(reader);

            // Create property and Attribute dictionaries
            var _attributes = new Dictionary<string, DBFieldAttribute>();
            var _properties = new Dictionary<string, PropertyInfo>();

            foreach (PropertyInfo property in typeof(TRESULTOBJECT).GetProperties())
            {
                DBFieldAttribute _attribute = property.GetCustomAttribute<DBFieldAttribute>(false);
                if (_attribute != null)
                {
                    string _key = _attribute.Field.ToLower();
                    _attributes.Add(_key, _attribute);
                    _properties.Add(_key, property);
                }
            }

            // Validate fieldsnames
            foreach (string column in _readerFieldNames)
            {
                if (!_properties.ContainsKey(column))
                {
                    string message = string.Format("Column '{0}' does not exist in the result DataSet.", column);
                    throw new DataException(message);
                }
            }

            // Convert Property / Attribute Dictionaries to Ordinal Array.
            _Attributes = new DBFieldAttribute[_readerFieldNames.Length];
            _Properties = new PropertyInfo[_readerFieldNames.Length];

            for (int index = 0; index < _readerFieldNames.Length; index++)
            {
                string _name = _readerFieldNames[index];
                _Attributes[index] = _attributes[_name];
                _Properties[index] = _properties[_name];
            }

            _Initialized = true;
        }


        private void SetPropertyValue(PropertyInfo propertyInfo, TRESULTOBJECT dataobject, object value, Type valueType, object defaultvalue, StrictMapping strictField)
        {
            if (value == DBNull.Value || value == null)
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
                propertyInfo.SetValue(dataobject, Enum.Parse(propertyInfo.PropertyType, System.Convert.ToString(value)), null);
                return;
            }

            if ((strictField == StrictMapping.ByConfigSetting && Config.StrictMapping) || strictField == StrictMapping.True)
            {
                if (propertyInfo.PropertyType == valueType)
                {
                    propertyInfo.SetMethod.Invoke(dataobject, new object[] { value });  
                    return;
                }

                throw new DataException("Property '{0}': Type cannot be mapped from '{1}' to '{2}' ", propertyInfo.Name, valueType, propertyInfo.PropertyType);
            }

            if (propertyInfo.PropertyType == valueType)
                propertyInfo.SetMethod.Invoke(dataobject, new object[] { value });
            else
                propertyInfo.SetMethod.Invoke(dataobject, new object[] { Convert.ChangeType(value, propertyInfo.PropertyType, Config.CultureInfo) });
        }
    }
}