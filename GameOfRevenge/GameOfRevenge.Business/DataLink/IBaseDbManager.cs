using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models.Table;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Business.DataLink
{
    internal interface IBaseDbManager
    {
        string ConnectionString { get; }
        Task<Response<List<T>>> ExecuteSPMultipleRow<T>(string spName) where T : IBaseTable, new();
        Task<Response<List<T>>> ExecuteSPMultipleRow<T>(string spName, bool throwError) where T : IBaseTable, new();
        Task<Response<List<T>>> ExecuteSPMultipleRow<T>(string spName, IDictionary<string, object> spParams) where T : IBaseTable, new();
        Task<Response<List<T>>> ExecuteSPMultipleRow<T>(string spName, IDictionary<string, object> spParams, bool throwError) where T : IBaseTable, new();
        Task<Response<List<T>>> ExecuteSPMultipleRowPaged<T>(string spName) where T : IBaseTable, new();
        Task<Response<List<T>>> ExecuteSPMultipleRowPaged<T>(string spName, int page) where T : IBaseTable, new();
        Task<Response<List<T>>> ExecuteSPMultipleRowPaged<T>(string spName, int page, int count) where T : IBaseTable, new();
        Task<Response<List<T>>> ExecuteSPMultipleRowPaged<T>(string spName, int page, int count, bool throwError) where T : IBaseTable, new();
        Task<Response<List<T>>> ExecuteSPMultipleRowPaged<T>(string spName, int page, int count, IDictionary<string, object> spParams) where T : IBaseTable, new();
        Task<Response<List<T>>> ExecuteSPMultipleRowPaged<T>(string spName, int page, int count, IDictionary<string, object> spParams, bool throwError) where T : IBaseTable, new();
        Task<Response> ExecuteSPNoData(string spName);
        Task<Response> ExecuteSPNoData(string spName, IDictionary<string, object> spParams);
        Task<Response> ExecuteSPNoData(string spName, IDictionary<string, object> spParams, bool throwError);
        Task<Response<T>> ExecuteSPSingleRow<T>(string spName) where T : IBaseTable, new();
        Task<Response<T>> ExecuteSPSingleRow<T>(string spName, bool throwError) where T : IBaseTable, new();
        Task<Response<T>> ExecuteSPSingleRow<T>(string spName, IDictionary<string, object> spParams) where T : IBaseTable, new();
        Task<Response<T>> ExecuteSPSingleRow<T>(string spName, IDictionary<string, object> spParams, bool throwError) where T : IBaseTable, new();
    }
}