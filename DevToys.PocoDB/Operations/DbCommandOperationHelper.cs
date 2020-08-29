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
        private ConnectionConfig _Config;
        private bool _Initialized = false;
        private List<string> _OutputParameters;

        public DbCommandOperationHelper(ConnectionConfig config)
        {
            _Config = config;
        }

        public Dictionary<string, DBParameterAttribute> Attributes { get; private set; }

        public DBCommandAttribute CommandAttribute { get; private set; }
        public Dictionary<string, PropertyInfo> Properties { get; private set; }

        public void GetParameters(DbCommand command, TCOMMAND commandObject)
        {
            foreach (string name in _OutputParameters)
            {
                DBParameterAttribute _attribute = Attributes[name];
                PropertyInfo _property = Properties[name];
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
            foreach (string name in Properties.Keys)
            {
                var attribute = Attributes[name];
                var property = Properties[name];

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
            Attributes = typeof(TCOMMAND).GetProperties().Where(p => p.GetCustomAttribute(typeof(DBParameterAttribute)) != null)
                .Select(p => new { Name = p.GetCustomAttribute<DBParameterAttribute>(true).Name, Parameter = p.GetCustomAttribute<DBParameterAttribute>() })
                .ToDictionary(p => p.Name, p => p.Parameter);

            Properties = typeof(TCOMMAND).GetProperties().Where(p => p.GetCustomAttribute(typeof(DBParameterAttribute)) != null)
                .Select(p => new { Name = p.GetCustomAttribute<DBParameterAttribute>(true).Name, Property = p })
                .ToDictionary(p => p.Name, p => p.Property);

            _OutputParameters = typeof(TCOMMAND).GetProperties().Where(p => p.GetCustomAttribute(typeof(DBParameterAttribute)) != null)
                .Select(p => p.GetCustomAttribute<DBParameterAttribute>(true)).Where(p => p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.ReturnValue)
                .Select(p => p.Name)
                .ToList();

            CommandAttribute = GetDBCommandAttribute();
        }
    }
}