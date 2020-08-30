using System.Configuration;
using System.Globalization;

namespace DevToys.PocoDB
{
    public class ConnectionConfig : ConfigurationElement
    {
        public ConnectionConfig() { }


        private string _Name = null;

        [ConfigurationProperty("Name", DefaultValue = "", IsRequired = true, IsKey = true)]
        public string Name
        {
            get 
            { 
                if (string.IsNullOrEmpty(_Name))
                    _Name = (string)this["Name"];

                return _Name;
            }
            set 
            {
                _Name = value;
                this["Name"] = value; 
            }
        }


        private string _ConnectionType = null;

        /// <summary>
        /// DataProvider to use, three types are registered SqlClient(SqlConnection), Odbc(OdbcConnection), OleDb(OleDbConnection).
        /// Other dataproviders need to be registered using the ConnectionFactory.
        /// </summary>
        [ConfigurationProperty("ConnectionType", DefaultValue = Factory.ConnectionFactoryDefaultTypes.SqlClient, IsRequired = true, IsKey = false)]
        public string ConnectionType
        {
            get 
            { 
                if (string.IsNullOrEmpty(_ConnectionType))
                    _ConnectionType = (string)this["ConnectionType"];

                return _ConnectionType;
            }
            set 
            {
                _ConnectionType = value;
                this["ConnectionType"] = value; 
            }
        }

        private string _ConnectionString = null;


        [ConfigurationProperty("ConnectionString", DefaultValue = "", IsRequired = true, IsKey = false)]
        public string ConnectionString
        {
            get 
            { 
                if (string.IsNullOrEmpty(_ConnectionString))
                    _ConnectionString = (string)this["ConnectionString"];

                return _ConnectionString;
            }
            set 
            {
                _ConnectionString = value;
                this["ConnectionString"] = value; 
            }
        }

        private bool? _StrictMapping = null;

        /// <summary>
        /// Determines whether the property type should match the Reader's field type (C# type), when false properties will be converted to the target type.
        /// </summary>
        [ConfigurationProperty("StrictMapping", DefaultValue = true, IsRequired = true, IsKey = false)]
        public bool StrictMapping
        {
            get 
            {
                if (!_StrictMapping.HasValue)
                    _StrictMapping = (bool)this["StrictMapping"];

                return _StrictMapping.Value; 
            
            }
            set 
            {
                _StrictMapping = value;
                this["StrictMapping"] = value; 
            }
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