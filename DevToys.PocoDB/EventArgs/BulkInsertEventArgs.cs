using System;

namespace DevToys.PocoDB
{
    public sealed class BulkInsertEventArgs : EventArgs
    {
        public int RowsProcessed { get; internal set; }
    }
}