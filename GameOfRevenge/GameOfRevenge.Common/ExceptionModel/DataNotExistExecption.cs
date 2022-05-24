using System;
using System.Runtime.Serialization;

namespace GameOfRevenge.Common
{
    public class DataNotExistExecption : Exception
    {
        public DataNotExistExecption() { }
        public DataNotExistExecption(string message) : base(message) { }
        public DataNotExistExecption(string message, Exception innerException) : base(message, innerException) { }
        protected DataNotExistExecption(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
