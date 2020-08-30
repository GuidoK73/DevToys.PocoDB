using System;
using System.Data;
using System.Reflection;

namespace DevToys.PocoDB.Attributes
{
    /// <summary>
    /// Translates to: System.Data.IDbDataParameter
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class DBParameterAttribute : BaseDBParameterAttribute
    {
        public DBParameterAttribute(string Name) : base(Name)
        {
        }

        public ParameterDirection Direction { get; set; } = ParameterDirection.Input;

        /// <summary>
        /// Occurs on Output parameters.
        /// </summary>
        public virtual void GetParameterValue<TCOMMAND>(TCOMMAND commandObject, PropertyInfo property, IDbDataParameter parameter)
        {
            object _val = parameter.Value;

            if (_val != DBNull.Value)
            {
                if (property.PropertyType.IsEnum)
                    property.SetValue(commandObject, Enum.Parse(property.PropertyType, _val.ToString()), null);
                else
                    property.SetValue(commandObject, _val);
            }
            else
            {
                if (property.PropertyType != typeof(string))
                    if (Nullable.GetUnderlyingType(property.PropertyType) == null)
                        throw new DataException("Output parameter property {0} cannot contain null value", property.Name);

                if (property.PropertyType.IsEnum)
                    property.SetValue(commandObject, Enum.Parse(property.PropertyType, _val.ToString()), null);
                else
                    property.SetValue(commandObject, null);
            }
        }

        /// <summary>
        /// Occurs on Input and Output parameters.
        /// </summary>
        public virtual void SetParameterValue<TCOMMAND>(TCOMMAND commandObject, PropertyInfo property, IDbDataParameter parameter)
        {
            object value = property.GetValue(commandObject);
            parameter.ParameterName = Name;
            parameter.Value = value;
        }
    }
}