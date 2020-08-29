using System.Configuration;
using System.Globalization;

namespace DevToys.PocoDB
{
    public class ConnectionConfig : ConfigurationElement
    {
        public ConnectionConfig() { }

        [ConfigurationProperty("Name", DefaultValue = "", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// DataProvider to use, three types are registered SqlClient(SqlConnection), Odbc(OdbcConnection), OleDb(OleDbConnection).
        /// Other dataproviders need to be registered using the ConnectionFactory.
        /// </summary>
        [ConfigurationProperty("ConnectionType", DefaultValue = Factory.ConnectionFactoryDefaultTypes.SqlClient, IsRequired = true, IsKey = false)]
        public string ConnectionType
        {
            get { return (string)this["ConnectionType"]; }
            set { this["ConnectionType"] = value; }
        }

        [ConfigurationProperty("ConnectionString", DefaultValue = "", IsRequired = true, IsKey = false)]
        public string ConnectionString
        {
            get { return (string)this["ConnectionString"]; }
            set { this["ConnectionString"] = value; }
        }

        /// <summary>
        /// Determines whether the property type should match the Reader's field type (C# type), when false properties will be converted to the target type.
        /// </summary>
        [ConfigurationProperty("StrictMapping", DefaultValue = true, IsRequired = true, IsKey = false)]
        public bool StrictMapping
        {
            get { return (bool)this["StrictMapping"]; }
            set { this["StrictMapping"] = value; }
        }


        /// <summary>
        /// Used for non strict mapping, leave empty for current culture.
        /// </summary>
        [ConfigurationProperty("Culture", DefaultValue = "", IsRequired = false, IsKey = false)]
        public string Culture
        {
            get { return (string)this["Culture"]; }
            set { this["Culture"] = value; }
        }

        private CultureInfo _cultureInfo;

        public CultureInfo CultureInfo
        {
            get
            {
                if (_cultureInfo != null)
                    return _cultureInfo;

                if (string.IsNullOrEmpty(Culture))
                {
                    _cultureInfo = CultureInfo.CurrentCulture;
                    return _cultureInfo;
                }

                _cultureInfo = CultureInfo.GetCultureInfo(Culture);
                return _cultureInfo;
            }
        }
    }
}