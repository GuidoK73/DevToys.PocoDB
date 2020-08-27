using DevToys.PocoDB.Encryption;
using System;
using System.Data;
using System.Reflection;
using System.Security;

namespace DevToys.PocoDB.Attributes
{ 
    /// <summary>
    /// Translates to: System.Data.IDbDataParameter
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class DBParameterAttribute : BaseDBParameterAttribute
    {
        public ParameterDirection Direction { get; set; } = ParameterDirection.Input;

        public DBParameterAttribute(string Name) : base(Name)
        {
        }

        public virtual void InitParameter<TCOMMAND>(TCOMMAND commandObject, PropertyInfo property, IDbDataParameter parameter, SecureString password)
        {
            object value = property.GetValue(commandObject);
            parameter.ParameterName = Name;
            parameter.Value = FieldEncryption.Encrypt(value, password, Encrypt);
        }
    }
}