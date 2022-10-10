using System;
using System.Runtime.Serialization;

namespace GameOfRevenge.Common
{
    public class RequirementExecption : Exception
    {
        public RequirementExecption() { }
        public RequirementExecption(string message) : base(message) { }
        public RequirementExecption(string message, Exception innerException) : base(message, innerException) { }
        protected RequirementExecption(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
