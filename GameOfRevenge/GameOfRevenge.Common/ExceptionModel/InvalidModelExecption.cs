using GameOfRevenge.Common.Net;
using System;
using System.Runtime.Serialization;

namespace GameOfRevenge.Common
{
    public class InvalidModelExecption : Exception
    {
        //public readonly int Case;

        public InvalidModelExecption() { }
        //public InvalidModelExecption(int caseType) { Case = caseType; }
        //public InvalidModelExecption(CaseType caseType) { Case = caseType == CaseType.Error ? 1 : caseType == CaseType.Success ? 100 : 200; }
        public InvalidModelExecption(string message) : base(message) { }
        //public InvalidModelExecption(int caseType, string message) : base(message) { }
        public InvalidModelExecption(string message, Exception innerException) : base(message, innerException) { }
        protected InvalidModelExecption(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
