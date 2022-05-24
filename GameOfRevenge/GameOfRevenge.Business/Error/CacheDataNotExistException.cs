using GameOfRevenge.Common.Net;
using System;

namespace GameOfRevenge.Business
{
    public class CacheDataNotExistExecption : Exception
    {
        private int caseNumber;
        public int Case { get => caseNumber; private set { caseNumber = value < 0 ? 0 : value; } }

        public CacheDataNotExistExecption() { }
        public CacheDataNotExistExecption(int caseNumber) { Case = caseNumber; }
        public CacheDataNotExistExecption(CaseType caseType) { Case = caseType == CaseType.Error ? 1 : caseType == CaseType.Success ? 100 : 200; }
        public CacheDataNotExistExecption(string message) : base(message) { }
        public CacheDataNotExistExecption(int caseNumber, string message) : base(message) { Case = caseNumber; }
        public CacheDataNotExistExecption(CaseType caseType, string message) : base(message) { Case = caseType == CaseType.Error ? 1 : caseType == CaseType.Success ? 100 : 200; }
        public CacheDataNotExistExecption(string message, Exception innerException) : base(message, innerException) { }
        public CacheDataNotExistExecption(int caseNumber, string message, Exception innerException) : base(message, innerException) { Case = caseNumber; }
        public CacheDataNotExistExecption(CaseType caseType, string message, Exception innerException) : base(message, innerException) { Case = caseType == CaseType.Error ? 1 : caseType == CaseType.Success ? 100 : 200; }

        public CaseType GetCaseType()
        {
            if (Case <= 0 || Case <= 99) return CaseType.Error;
            else if (Case <= 199) return CaseType.Success;
            else return CaseType.Invalid;
        }
    }
}
