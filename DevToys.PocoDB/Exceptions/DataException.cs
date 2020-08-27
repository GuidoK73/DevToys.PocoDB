using System;
using System.Runtime.Serialization;

namespace DevToys.PocoDB
{
    [Serializable]
    public class DataException : Exception
    { 
        public DataException() { }

        public DataException(string message, params object[] values) : base(string.Format(message, values)) { }

        public DataException(string message, Exception innerException) : base(message, innerException) { }

        protected DataException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) => base.GetObjectData(info, context);
    }
}