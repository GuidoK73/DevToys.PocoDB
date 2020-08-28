using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevToys.PocoDB.Enums;

namespace DevToys.PocoDB.Attributes
{ 
    public enum ReaderDefaults
    { 
        UseDefaultValueProperty = 0,

        /// <summary>DateTime</summary>
        DateTimeNow = 3,

        /// <summary>DateTime</summary>
        DateTimeToday = 4,

        /// <summary>TimeSpan</summary>
        TimeNow = 5,

        /// <summary>TimeSpan: cast null to Now time.</summary>
        DateTimeUTCNow = 6,

        /// <summary>Int, Double, Float, DateTime etc</summary>
        MinValue = 7,

        /// <summary>Int, Double, Float, DateTime etc</summary>
        MaxValue = 8,

        /// <summary> Guid: Cast null to Empty Guid</summary>
        GuidEmpty = 9,

        /// <summary>Guid: Cast null to new Guid</summary>
        GuidNew = 10,

        /// <summary> String: Cast null to empty string</summary>
        StringEmpty = 11
    }


    /// <summary>
    /// Relates a property to a DB field.
    ///
    /// this Attribute is Used by DataOperation<TObject> Read methods.
    /// it will map the datatable fields to the corresponding values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class DBFieldAttribute : Attribute
    { 
        public DBFieldAttribute(string field)
        {
            Field = field;
        }

        public string Field { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether this data must be Encrypted. (Only works for String and Byte[] properties). 
        /// WARNING: this can quickly become a performance drain.
        /// </summary>
        public bool Decrypt { get; set; } = false;

        /// <summary>
        /// Determines default value when object is read from DB and the specific value property is DBNull. (it's not an object creation default!)
        /// </summary>
        public ReaderDefaults ReaderCustomDefault { get; set; } = ReaderDefaults.UseDefaultValueProperty;

        /// <summary>
        /// Determines how field is mapped.
        /// </summary>
        public StrictMapping StrictMapping { get; set; } = StrictMapping.ByConfigSetting;

        /// <summary>
        /// Determines default value when object is read from DB and the specific value property is DBNull. (it's not an object creation default!)
        /// </summary>
        public object ReaderDefaultValue { get; set; }

        /// <summary>
        /// Retrieve properties marked with DBField, as dictionary where key is specified Field.
        /// </summary>
        internal static Dictionary<string, object> GetDBFieldPropertiesDefaults(Type type)
        {
            Dictionary<string, object> _result = new Dictionary<string, object>();
            IEnumerable<PropertyInfo> _properties = type.GetProperties().Where(p => p.GetCustomAttributes(typeof(DBFieldAttribute), true).Count() > 0);

            foreach (PropertyInfo property in _properties)
            {
                DBFieldAttribute attribute = property.GetCustomAttributes(typeof(DBFieldAttribute), true).FirstOrDefault() as DBFieldAttribute;

                object _defaultval = null;
                switch (attribute.ReaderCustomDefault)
                {
                    case ReaderDefaults.DateTimeNow:
                        _defaultval = DateTime.Now;
                        break;

                    case ReaderDefaults.DateTimeUTCNow:
                        _defaultval = DateTime.UtcNow;
                        break;

                    case ReaderDefaults.DateTimeToday:
                        _defaultval = DateTime.Today;
                        break;

                    case ReaderDefaults.GuidEmpty:
                        _defaultval = Guid.Empty;
                        break;

                    case ReaderDefaults.GuidNew:
                        _defaultval = Guid.NewGuid();
                        break;

                    case ReaderDefaults.MaxValue:
                        _defaultval = DataUtils.GetMaxValue(property.DeclaringType);
                        break;

                    case ReaderDefaults.MinValue:
                        _defaultval = DataUtils.GetMinValue(property.DeclaringType);
                        break;

                    case ReaderDefaults.TimeNow:
                        _defaultval = DateTime.Now;
                        break;

                    case ReaderDefaults.UseDefaultValueProperty:
                        _defaultval = attribute.ReaderDefaultValue;
                        break;

                    case ReaderDefaults.StringEmpty:
                        _defaultval = string.Empty;
                        break;
                }
                _result.Add(attribute.Field, _defaultval);
            }
            return _result;
        }
    }
}