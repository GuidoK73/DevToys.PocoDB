using DevToys.PocoDB.Enums;
using System;

namespace DevToys.PocoDB.Attributes
{
    /// <summary>
    /// Relates a property to a DB field.
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
        /// Determines how field is mapped.
        /// </summary>
        public StrictMapping StrictMapping { get; set; } = StrictMapping.ByConfigSetting;

        /// <summary>
        /// Determines default value when object is read from DB and the specific value property is DBNull. (it's not an object creation default!)
        /// </summary>
        public object ReaderDefaultValue { get; set; }
    }
}