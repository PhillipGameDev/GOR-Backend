using GameOfRevenge.Common.Models.Table;
using GameOfRevenge.Common.Services;
using System;
using System.Data;

namespace GameOfRevenge.Common.Net
{
    public class Response : IResponse, IBaseTable
    {
        public Response()
        {
            Message = null;
            Case = 0;
        }

        public Response(int caseNum, string message)
        {
            Message = message;
            Case = caseNum;
        }

        public Response(CaseType caseType, string message)
        {
            Message = message;
            Case = caseType == CaseType.Error ? 1 : caseType == CaseType.Success ? 100 : 200;
        }

        /// <summary>
        /// 0-99 - Error
        /// 100-199 - Success
        /// 200-999 - Other
        /// </summary>
        public int Case { get; set; }
        /// <summary>
        /// Additional message to display in frontend
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// IsSuccess = true the executed successfully
        /// </summary>
        public bool IsSuccess => GetCaseType() == CaseType.Success;

        public CaseType GetCaseType()
        {
            if (Case <= 0 || Case <= 99) return CaseType.Error;
            else if (Case <= 199) return CaseType.Success;
            else return CaseType.Invalid;
        }

        public static Response GetErrorResponse(Exception ex)
        {
            return new Response(CaseType.Error, ErrorManager.ShowError(ex));
        }

        public virtual void LoadFromDataReader(IDataReader reader)
        {
            Message = reader.GetValue(0) == DBNull.Value ? string.Empty : reader.GetString(0);
            Case = reader.GetValue(1) == DBNull.Value ? 0 : reader.GetInt32(1);
        }
    }

    public class Response<T> : Response, IResponse, IResponse<T>, IPagedResponse, IBaseTable
    {
        public Response()
        {
            Data = default;
        }

        public Response(int casee, string message) : base(casee, message)
        {
            Data = default;
        }

        public Response(CaseType caseType, string message) : base(caseType, message)
        {
            Data = default;
        }

        public Response(T data)
        {
            Data = data;
        }

        public Response(T data, int casee, string message) : base(casee, message)
        {
            Data = data;
        }

        public Response(T data, CaseType caseType, string message) : base(caseType, message)
        {
            Data = data;
        }

        /// <summary>
        /// Response data returned form execution
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// Check if request has returen any data
        /// </summary>
        public bool HasData => Data != null;

        public static Response<T> GetErrorResponse(T data, Exception ex)
        {
            return new Response<T>(data, CaseType.Error, ErrorManager.ShowError(ex));
        }

        public PagedView PageDetails { get; set; }

        public override void LoadFromDataReader(IDataReader reader)
        {
            Message = reader.GetValue(0) == DBNull.Value ? string.Empty : reader.GetString(0);
            Case = reader.GetValue(1) == DBNull.Value ? 0 : reader.GetInt32(1);
        }
    }
}
