using System.Collections.Generic;
using System.Data;

namespace DevToys.PocoDB.Operations
{
    /// <summary>
    /// this Operation can be used to extract data from DataSet object.
    ///
    /// Map datatable rows to object TARGET based on property binding. ByAttribute binding binds to properties with DBFieldAttribute.
    /// </summary>
    /// <typeparam name="TARGET"></typeparam>
    public sealed class DataTableMapperOperation<TARGET> : BaseDataOperation<TARGET> where TARGET : class, new()
    { 
        public DataTableMapperOperation() : base(new ConnectionConfig() { StrictMapping = true })
        { }

        /// <param name="strictMapping">Determines whether the property type should match the Reader's field type (C# type), when false properties will be converted to the target type.</param>
        public DataTableMapperOperation(bool strictMapping) : base(new ConnectionConfig() { StrictMapping = strictMapping }) { }

        /// <summary>Map rows to TARGET objects</summary>
        /// <param name="dataTable">source datatable.</param>
        public IEnumerable<TARGET> Map(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
                yield return ReadDataRow(dataTable, row);
        }
    }
}