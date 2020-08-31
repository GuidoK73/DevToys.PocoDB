using DevToys.PocoDB.Attributes;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace DevToys.PocoDB
{
    internal sealed class DbCommandOperationHelper<TCOMMAND>
    {
        private readonly ConnectionConfig _Config;
        private bool _Initialized = false;

        DBParameterAttribute[] _Attributes;
        PropertyInfo[] _Properties;
        List<int> _OutputParameters = new List<int>();

        public DbCommandOperationHelper(ConnectionConfig config)
        {
            _Config = config;
        }

        public DBCommandAttribute CommandAttribute { get; set; }

        public void GetParameters(DbCommand command, TCOMMAND commandObject)
        {
            foreach (int index in _OutputParameters)
            {
                var _attribute = _Attributes[index];
                var _property = _Properties[index];
                var _parameter = command.Parameters[_attribute.Name];

                if (!DataUtils.IsSimpleType(_property.PropertyType) || _property.PropertyType.IsEnum)
                    throw new DataException("Output parameter property {0} must be a simple type", _property.Name);

                _attribute.GetParameterValue<TCOMMAND>(commandObject, _property, _parameter);
            }
        }

        public void Initialize()
        {
            if (_Initialized)
                return;

            Dictionary<string, DBParameterAttribute> _attributes = new Dictionary<string, DBParameterAttribute>();
            Dictionary<string, PropertyInfo> _properties = new Dictionary<string, PropertyInfo>();

            foreach (PropertyInfo property in typeof(TCOMMAND).GetProperties())
            {
                DBParameterAttribute _attribute = property.GetCustomAttribute<DBParameterAttribute>(true);
                string _key = _attribute.Name;
                _attributes.Add(_key, _attribute);
                _properties.Add(_key, property);
            }

            _Attributes = new DBParameterAttribute[_attributes.Keys.Count];
            _Properties = new PropertyInfo[_attributes.Keys.Count];

            int _index = 0;

            foreach (string key in _attributes.Keys)
            {
                var _attribute = _attributes[key];
                _Attributes[_index] = _attribute;
                _Properties[_index] = _properties[key];

                if (_attribute.Direction == ParameterDirection.InputOutput || _attribute.Direction == ParameterDirection.Output || _attribute.Direction == ParameterDirection.ReturnValue)
                    _OutputParameters.Add(_index);

                _index++;
            }

            CommandAttribute = GetDBCommandAttribute();

            _Initialized = true;
        }

        public void SetParameters(DbCommand command, TCOMMAND commandObject)
        {
            for (int index = 0; index < _Properties.Count(); index++)
            {
                var _attribute = _Attributes[index];
                var _property = _Properties[index];
                var _parameter = command.CreateParameter();
                _parameter.Direction = _attribute.Direction;
                _attribute.SetParameterValue<TCOMMAND>(commandObject, _property, _parameter);
                command.Parameters.Add(_parameter);
            }
        }

        private DBCommandAttribute GetDBCommandAttribute()
        {
            var _commandAttribute = typeof(TCOMMAND).GetCustomAttribute<DBCommandAttribute>();

            if (_commandAttribute == null)
                throw new CustomAttributeFormatException(string.Format("DBCommandAttribute is missing on {0}", typeof(TCOMMAND).Name));

            if (!string.IsNullOrEmpty(_commandAttribute.RequiredConnectionType))
                if (!_Config.ConnectionType.Equals(_commandAttribute.RequiredConnectionType))
                    throw new DataException("Required connection type: {0}, not supplied for connectionname: {1} ", _commandAttribute.RequiredConnectionType, _Config.Name);

            return _commandAttribute;
        }
    }
}