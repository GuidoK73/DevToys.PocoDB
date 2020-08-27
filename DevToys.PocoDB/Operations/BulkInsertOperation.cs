using DevToys.PocoDB.Attributes;
using DevToys.PocoDB.Factory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace DevToys.PocoDB.Operations
{ 
    /// <summary>
    /// SQLCLIENT CONNECTION ONLY
    ///
    /// Class for executing bulk insert operations. Wrapper arround System.Data.SqlClient.SqlBulkCopy.
    /// </summary>
    /// <typeparam name="TINSERTOBJECT">The input object</typeparam>
    public sealed class BulkInsertOperation<TINSERTOBJECT> : BaseDataOperation
        where TINSERTOBJECT : class, new()
    {
        private int _Batchsize = 0;

        private Dictionary<string, DBParameterAttribute> _DbParameters = new Dictionary<string, DBParameterAttribute>();

        private Dictionary<string, PropertyInfo> _DBProperties = new Dictionary<string, PropertyInfo>();

        private bool _Initialized = false;

        private string _TableName;

        /// <summary>
        ///
        /// </summary>
        /// <param name="configConnectionName">Reference to connection in DevToys.PocoDB config section</param>
        /// <param name="batchsize"></param>
        public BulkInsertOperation(string configConnectionName, int batchsize) : base(configConnectionName)
        {
            if (!Config.ConnectionType.Equals(ConnectionFactoryDefaultTypes.SqlClient))
                throw new DataException("BulkInsertOperation only available for SqlClient ConnectionType");

            var bulkinsertattr = typeof(TINSERTOBJECT).GetCustomAttribute<DBBulkInsertAttribute>();
            if (bulkinsertattr == null)
                throw new DataException(string.Format("{0} not defined on {1}.", typeof(DBBulkInsertAttribute).Name, typeof(TINSERTOBJECT).Name));

            if (_Batchsize < 25)
                _Batchsize = 25;

            if (_Batchsize > 100000)
                _Batchsize = 100000;

            _Batchsize = batchsize;
            _TableName = bulkinsertattr.TableName;
        }

        public event EventHandler<BulkInsertEventArgs> Progress = delegate { };

        /// <summary>
        /// Insert repetative Data. this is usefull when using the DBRandomParameter.
        /// </summary>
        /// <param name="repeat"></param>
        public void Insert(int repeat) => Insert(Repeat(repeat));

        /// <summary>
        /// Bulk insert data
        /// </summary>
        /// <param name="data"></param>
        public void Insert(IEnumerable<TINSERTOBJECT> data)
        {
            Init();

            var _table = new DataTable(_TableName);

            foreach (string key in _DBProperties.Keys)
            {
                var property = _DBProperties[key];
                var column = new DataColumn(key, property.PropertyType);
                _table.Columns.Add(column);
            }

            int ii = 0;

            foreach (TINSERTOBJECT item in data)
            {
                var row = _table.NewRow();
                foreach (string key in _DBProperties.Keys)
                {
                    var property = _DBProperties[key];
                    var parameter = GetParameter(item, _DbParameters[key], property);
                    //if (property.PropertyType.IsEnum)
                    //    row[key] = (int)parameter.Value;
                    //else
                    row[key] = Convert.ChangeType(parameter.Value, property.PropertyType);
                }
                _table.Rows.Add(row);

                if (ii % _Batchsize == 0 && ii > 0)
                {
                    Insert(_table);
                    _table.Rows.Clear();
                    Progress(this, new BulkInsertEventArgs() { RowsProcessed = ii });
                }
                ii++;
            }
            if (_table.Rows.Count > 0)
                Insert(_table);
        }

        private IDbDataParameter GetParameter(TINSERTOBJECT commandObject, DBParameterAttribute attribute, PropertyInfo property)
        {
            IDbDataParameter parameter = new SqlParameter();
            parameter.Direction = attribute.Direction;
            attribute.InitParameter(commandObject, property, parameter, Config.FieldEncryptionPasswordEncrypted);
            return parameter;
        }

        private void Init()
        {
            if (_Initialized)
                return;

            _DBProperties = typeof(TINSERTOBJECT).GetProperties().Where(p => p.GetCustomAttribute(typeof(DBParameterAttribute)) != null)
                .Select(p => new { Name = p.GetCustomAttribute<DBParameterAttribute>().Name, Parameter = p })
                .ToDictionary(p => p.Name, p => p.Parameter);

            _DbParameters = typeof(TINSERTOBJECT).GetProperties().Where(p => p.GetCustomAttribute(typeof(DBParameterAttribute)) != null)
                .Select(p => new { Name = p.GetCustomAttribute<DBParameterAttribute>().Name, Parameter = p.GetCustomAttribute<DBParameterAttribute>() })
                .ToDictionary(p => p.Name, p => p.Parameter);

            _Initialized = true;
        }

        private void Insert(DataTable table)
        {
            using (SqlConnection connection = ConnectionFactory.Instance.Create(Config.ConnectionType, Config.ConnectionString) as SqlConnection)
            {
                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(connection))
                {
                    connection.Open();
                    bulkcopy.DestinationTableName = _TableName;

                    foreach (string key in _DBProperties.Keys)
                        bulkcopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(key, key));

                    bulkcopy.WriteToServer(table);
                }
            }
        }

        private IEnumerable<TINSERTOBJECT> Repeat(int repeat)
        {
            for (int ii = 0; ii < repeat; ii++)
                yield return new TINSERTOBJECT();
        }
    }
}