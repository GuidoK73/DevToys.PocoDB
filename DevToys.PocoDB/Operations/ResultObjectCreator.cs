using DevToys.PocoDB.Factory;
using System.Data;
using System.Data.Common;
using System.Text;
 
namespace DevToys.PocoDB.Operations
{ 
    /// <summary>
    /// Generate TResultObject script for DBCommandOperation or SqlOperation.
    /// </summary>
    public sealed class ResultObjectCreator : BaseDataOperation
    {
        /// <param name="configConnectionName">Points to ConnectionString Configuration in section DevToys.PocoDB in App.Config</param>
        public ResultObjectCreator(string configConnectionName) : base(configConnectionName) { }

        public ResultObjectCreator(ConnectionConfig config) : base(config)  {  }

        /// <param name="configConnectionName">Points to ConnectionString Configuration in section DevToys.PocoDB in App.Config</param>
        public ResultObjectCreator(DbConnectionStringBuilder connectionString, string configConnectionName) : base(connectionString, configConnectionName)  { }


        public string PropertyTemplate { get; set; } = "[DBField(\"{0}\")]\r\npublic {1} {2} {{ get; set; }}\r\n\r\n";

        public string ExecuteSingleReader(string sql) => ExecuteSingleReader(sql, null);
        
        /// <summary>
        /// Returns the first Row for a query converted to T.
        /// Mapping will be done by DBField attributes or by overriding the ReadDataRow method.
        /// </summary>
        /// <param name="sql">script to execute</param>
        public string ExecuteSingleReader(string sql, params IDbDataParameter[] parameters)
        {
            string result = string.Empty;

            using (DbConnection connection = ConnectionFactory.Instance.Create(Config.ConnectionType, Config.ConnectionString))
            {
                connection.Open();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;

                    if (parameters != null)
                        command.Parameters.AddRange(parameters);

                    RaisePreExecute(connection, command);

                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        bool b = reader.Read();
                        if (b)
                            result = ReadDataRow(reader, PropertyTemplate);
                    }
                }
                connection.Close();
                return result;
            }
        }

        /// <summary>
        /// Each data table requires at least one record.
        /// </summary>
        public string FromDataSet(DataSet dataSet)
        {
            string _template = PropertyTemplate;
            StringBuilder _code = new StringBuilder();

            foreach (DataTable table in dataSet.Tables)
            {
                _code.Append($"\tpublic class {DataUtils.CleanString(table.TableName)}\r\n");
                _code.Append("\t{\r\n\r\n");
                foreach (DataColumn column in table.Columns)
                {
                    string _type = table.Rows[0][column].GetType().Name;
                    string _name = column.ColumnName;
                    _code.AppendFormat(_template, _name, _type, DataUtils.CamelCaseUpper(_name));
                }
                _code.Append("\t}\r\n");
            }

            return _code.ToString();
        }

        /// <summary>
        /// Data table requires at least one record.
        /// </summary>
        public string FromDataTable(DataTable dataTable)
        {
            string _template = PropertyTemplate;
            StringBuilder _properties = new StringBuilder();

            foreach (DataColumn column in dataTable.Columns)
            {
                string _type = dataTable.Rows[0][column].GetType().Name;
                string _name = column.ColumnName;
                _properties.AppendFormat(_template, _name, _type, DataUtils.CamelCaseUpper(_name));
            }

            return _properties.ToString();
        }

        internal static string ReadDataRow(IDataReader reader, string template)
        {
            StringBuilder properties = new StringBuilder();

            foreach (DataRow row in reader.GetSchemaTable().Rows)
            {
                properties.Append(string.Format(template,
                    row["ColumnName"].ToString(),
                    row["DataType"].ToString(),
                    DataUtils.CamelCaseUpper(row["ColumnName"].ToString())));
            }
            return properties.ToString();
        }
    }

    /// <summary>
    /// Generate TResultObject script for DBCommandOperation or SqlOperation.
    /// </summary>
    public sealed class ResultObjectCreator<TCOMMAND> : BaseDataOperation
        where TCOMMAND : class
    {
        private DbCommandOperationHelper<TCOMMAND> _Helper;

        /// <param name="connectionTypeName">Points to ConnectionString Configuration in section DevToys.PocoDB in App.Config</param>
        public ResultObjectCreator(string connectionTypeName) : base(connectionTypeName) { }

        /// <param name="connectionname">Reference to connection in DevToys.PocoDB config section</param>
        public ResultObjectCreator(ConnectionConfig config) : base(config) { }

        /// <param name="connectionname">Reference to connection in DevToys.PocoDB config section</param>
        public ResultObjectCreator(string connectionname, string commandName) : base(connectionname) => CommandName = commandName;

        /// <param name="connectionname">Reference to connection in DevToys.PocoDB config section</param>
        public ResultObjectCreator(ConnectionConfig config, string commandName) : base(config) => CommandName = commandName;

        /// <param name="connectionTypeName">Points to ConnectionString Configuration in section DevToys.PocoDB in App.Config</param>
        public ResultObjectCreator(DbConnectionStringBuilder connectionString, string connectionTypeName) : base(connectionString, connectionTypeName) { }

        /// <summary>Reference to DbCommand's name attribute, pointer to query command to use. </summary>
        public string CommandName { get; set; } = string.Empty;

        public string PropertyTemplate { get; set; } = "\t\t[DBField(\"{0}\")]\r\n\t\tpublic {1} {2} { get; set; }";

        /// <summary>
        /// Exexutes a procedure, parameters are specified by TProcedure object marked with DBProcedureAttribute and DBParameterAttributes
        /// returns TObject, TObject must be marked with DBFieldAttributes and must match the procedure result.
        /// </summary>
        public string ExecuteSingleReader(TCOMMAND commandObject)
        {
            Init(); 

            string _result = string.Empty;

            using (DbConnection connection = ConnectionFactory.Instance.Create(Config.ConnectionType, Config.ConnectionString))
            {
                connection.Open();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = _Helper.CommandAttribute.CommandText;
                    command.CommandType = _Helper.CommandAttribute.CommandType;
                    command.CommandTimeout = _Helper.CommandAttribute.CommandTimeout;

                    _Helper.SetParameters(command, commandObject);

                    RaisePreExecute(connection, command);

                    IDataReader reader = command.ExecuteReader();

                    _Helper.GetParameters(command, commandObject);

                    bool b = reader.Read();
                    if (b)
                        _result = ResultObjectCreator.ReadDataRow(reader, PropertyTemplate);
                }
                connection.Close();
            }
            return _result;
        }

        private void Init()
        {
            if (_Helper != null)
                return;

            _Helper = new DbCommandOperationHelper<TCOMMAND>(Config);
            _Helper.Initialize();
        }

    }
}