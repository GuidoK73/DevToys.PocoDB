﻿using DevToys.PocoDB.Attributes;
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
                DBParameterAttribute _attribute = _Attributes[index];
                PropertyInfo _property = _Properties[index];
                DbParameter _parameter = command.Parameters[_attribute.Name];

                if (!DataUtils.IsSimpleType(_property.PropertyType) || _property.PropertyType.IsEnum)
                    throw new DataException("Output parameter property {0} must be a simple type", _property.Name);

                _attribute.GetParameterValue<TCOMMAND>(commandObject, _property, _parameter);
            }
        }

        public void Initialize()
        {
            if (_Initialized)
                return;

            Init();

            _Initialized = true;
        }

        public void SetParameters(DbCommand command, TCOMMAND commandObject)
        {
            for (int index = 0; index < _Properties.Count(); index++)
            {
                var attribute = _Attributes[index];
                var property = _Properties[index];

                IDbDataParameter parameter = command.CreateParameter();
                parameter.Direction = attribute.Direction;
                attribute.SetParameterValue<TCOMMAND>(commandObject, property, parameter);
                command.Parameters.Add(parameter);
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

        private void Init()
        {
            Dictionary<string, DBParameterAttribute> _attributes = typeof(TCOMMAND).GetProperties().Where(p => p.GetCustomAttribute(typeof(DBParameterAttribute)) != null)
                .Select(p => new { Name = p.GetCustomAttribute<DBParameterAttribute>(true).Name, Parameter = p.GetCustomAttribute<DBParameterAttribute>() })
                .ToDictionary(p => p.Name, p => p.Parameter);

            Dictionary<string, PropertyInfo> _properties = typeof(TCOMMAND).GetProperties().Where(p => p.GetCustomAttribute(typeof(DBParameterAttribute)) != null)
                .Select(p => new { Name = p.GetCustomAttribute<DBParameterAttribute>(true).Name, Property = p })
                .ToDictionary(p => p.Name, p => p.Property);

            _Attributes = new DBParameterAttribute[_attributes.Keys.Count];
            _Properties = new PropertyInfo[_attributes.Keys.Count];

            int _index = 0;
           
            foreach (string key in _attributes.Keys)
            {
                var _attribute = _attributes[key];
                _Attributes[_index] = _attribute;
                _Properties[_index] = _properties[key];

                if (_attribute.Direction == ParameterDirection.InputOutput ||  _attribute.Direction == ParameterDirection.Output || _attribute.Direction == ParameterDirection.ReturnValue)
                    _OutputParameters.Add(_index);

                _index++;
            }

            CommandAttribute = GetDBCommandAttribute();
        }
    }
}