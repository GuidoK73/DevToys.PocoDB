using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Text;

namespace DevToys.PocoDB.Attributes
{
    /// <summary>
    /// Translates to: System.Data.IDbDataParameter.
    /// Use this attribute if you want to map an array/collection property to a procedure parameter accepting a comma seperated string.
    /// this attribute can be placed on List<T> or T[] where T is a simple type like int, Guid, DateTime, string, etc.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class DBStringArrayParameterAttribute : DBParameterAttribute
    {
        /// <param name="name">DbParameter Name</param>
        public DBStringArrayParameterAttribute(string name) : base(name) { }

        public override void GetParameterValue<TCOMMAND>(TCOMMAND commandObject, PropertyInfo property, IDbDataParameter parameter)
        {
            throw new DataException($"Output parameters not supported for '{nameof(DBStringArrayParameterAttribute)}'.");
        }

        public override void SetParameterValue<TCOMMAND>(TCOMMAND commandObject, PropertyInfo property, IDbDataParameter parameter)
        {
            object _value = property.GetValue(commandObject);
            string _valuestring = null;
            bool _isenumerable = (_value.GetType().GetInterface(nameof(IEnumerable)) != null);
            if (_isenumerable)
            {
                IEnumerable _values = _value as IEnumerable;

                StringBuilder _sb = new StringBuilder();
                foreach (var val in _values)
                {
                    _sb.Append(val);
                    _sb.Append(",");
                }
                if (_sb.Length > 0)
                    _sb.Length--;

                _valuestring = DataUtils.NormalizeQuotes(_sb.ToString());
            }
            parameter.ParameterName = Name;

            if (_valuestring == null)
                parameter.Value = DBNull.Value;
            else
                parameter.Value = _valuestring;
        }
    }
}