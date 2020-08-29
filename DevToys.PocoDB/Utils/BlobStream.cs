using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

// https://stackoverflow.com/questions/2101149/how-to-i-serialize-a-large-graph-of-net-object-into-a-sql-server-blob-without-c

// https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sqlclient-streaming-support

namespace DevToys.PocoDB.Utils
{
    public class BlobStream : Stream
    {
        private SqlCommand _CommandAppendChunk;
        private SqlCommand _CommandFirstChunk;
        private SqlConnection _Connection;
        private SqlTransaction _Transaction;

        private SqlParameter _ParameterChunk;
        private SqlParameter _ParameterLength;

        private long offset;

        public BlobStream(SqlConnection connection, SqlTransaction transaction, string schemaName, string tableName, string blobColumn, string keyColumn, object keyValue)
        {
            this._Transaction = transaction;
            this._Connection = connection;

            _CommandFirstChunk = new SqlCommand($"UPDATE [{schemaName}].[{tableName}] SET [{blobColumn}] = @firstChunk  WHERE [{keyColumn}] = @key");
            _CommandFirstChunk.Connection = connection;
            _CommandFirstChunk.Transaction = transaction;
            _CommandFirstChunk.Parameters.AddWithValue("@key", keyValue);

            _CommandAppendChunk = new SqlCommand($"UPDATE [{schemaName}].[{tableName}]  SET [{blobColumn}].WRITE(@chunk, NULL, NULL) WHERE [{keyColumn}] = @key");
            _CommandAppendChunk.Connection = connection;
            _CommandAppendChunk.Transaction = transaction;
            _CommandAppendChunk.Parameters.AddWithValue("@key", keyValue);
            _ParameterChunk = new SqlParameter("@chunk", SqlDbType.VarBinary, -1);
            _CommandAppendChunk.Parameters.Add(_ParameterChunk);
        }

        public override bool CanRead => throw new NotImplementedException();

        public override bool CanSeek => throw new NotImplementedException();

        public override bool CanWrite => throw new NotImplementedException();

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int index, int count)
        {
            byte[] bytesToWrite = buffer;
            if (index != 0 || count != buffer.Length)
            {
                bytesToWrite = new MemoryStream(buffer, index, count).ToArray();
            }
            if (offset == 0)
            {
                _CommandFirstChunk.Parameters.AddWithValue("@firstChunk", bytesToWrite);
                _CommandFirstChunk.ExecuteNonQuery();
                offset = count;
            }
            else
            {
                _ParameterChunk.Value = bytesToWrite;
                _CommandAppendChunk.ExecuteNonQuery();
                offset += count;
            }
        }

        // Rest of the abstract Stream implementation
    }
}
