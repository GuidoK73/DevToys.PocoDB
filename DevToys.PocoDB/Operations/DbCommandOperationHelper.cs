using DevToys.PocoDB.Attributes;
using DevToys.PocoDB.Encryption;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Security;

namespace DevToys.PocoDB
{ 
    internal sealed class DbCommandOperationHelper<TCOMMAND>
    {
        private ConnectionConfig _Config;
        private bool _Initialized = false;
        private List<string> _OutputParameters;
        private SecureString _Password;

        public DbCommandOperationHelper(ConnectionConfig config)
        {
            _Config = config;
            _Password = _Config.FieldEncryptionPasswordEncrypted;
        }

        public Dictionary<string, DBParameterAttribute> Attributes { get; private set; }

        public DBCommandAttribute CommandAttribute { get; private set; }
        public Dictionary<string, PropertyInfo> Properties { get; private set; }

        public void GetOutputParameters(DbCommand command, TCOMMAND commandObject)
        {
            foreach (string name in _OutputParameters)
            {
                DBParameterAttribute attribute = Attributes[name];
                PropertyInfo property = Properties[name];

                if (!DataUtils.IsSimpleType(property.PropertyType) || property.PropertyType.IsEnum)
                    throw new DataException("Output parameter property {0} must be a simple type", property.Name);

                object val = command.Parameters[string.Format("{0}", attribute.Name)].Value;

                val = FieldEncryption.Decrypt(val, _Password, attribute.Encrypt);

                if (val != DBNull.Value)
                {
                    if (property.PropertyType.IsEnum)
                        property.SetValue(commandObject, Enum.Parse(property.PropertyType, val.ToString()), null);
                    else
                        property.SetValue(commandObject, val);
                }
                else
                {
                    if (property.PropertyType != typeof(string))
                        if (Nullable.GetUnderlyingType(property.PropertyType) == null)
                            throw new DataException("Output parameter property {0} cannot contain null value", property.Name);

                    if (property.PropertyType.IsEnum)
                        property.SetValue(commandObject, Enum.Parse(property.PropertyType, val.ToString()), null);
                    else
                        property.SetValue(commandObject, null);
                }
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
                attribute.InitParameter<TCOMMAND>(commandObject, property, parameter, _Password);
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