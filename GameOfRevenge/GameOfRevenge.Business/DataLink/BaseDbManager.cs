using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models.Table;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Services;

namespace GameOfRevenge.Business.DataLink
{
    internal class BaseDbManager : IBaseDbManager
    {
        private static BaseDbManager instance = null;
        private string connectionString;

        public static IBaseDbManager Instance
        {
            get
            {
                if (instance == null) instance = new BaseDbManager();
                return instance;
            }
        }

        public string ConnectionString { get => string.IsNullOrWhiteSpace(connectionString) ? Config.ConnectionString : connectionString; set => connectionString = value; }

        string IBaseDbManager.ConnectionString => throw new NotImplementedException();

        public async Task<Response> ExecuteSPNoData(string spName) => await ExecuteSPNoData(spName, null, false);
        public async Task<Response> ExecuteSPNoData(string spName, IDictionary<string, object> spParams) => await ExecuteSPNoData(spName, spParams, false);
        public async Task<Response> ExecuteSPNoData(string spName, IDictionary<string, object> spParams, bool throwError)
        {
            var response = new Response();

            try
            {
                using (SqlConnection Connection = new SqlConnection(ConnectionString))
                {
                    await Connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(spName, Connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        AddParamToSqlCmd(spParams, command);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync()) response.LoadFromDataReader(reader);
                            reader.Close();
                        }
                        command.Dispose();
                    }
                    Connection.Close();
                }
            }
            catch (Exception ex)
            {
                if (throwError) throw ex;
                else
                {
                    response.Message = ErrorManager.ShowError(ex);
                    response.Case = 0;
                }
            }

            return response;
        }

        public async Task<Response<T>> ExecuteSPSingleRow<T>(string spName) where T : IBaseTable, new() => await ExecuteSPSingleRow<T>(spName, false);
        public async Task<Response<T>> ExecuteSPSingleRow<T>(string spName, bool throwError) where T : IBaseTable, new() => await ExecuteSPSingleRow<T>(spName, null, throwError);
        public async Task<Response<T>> ExecuteSPSingleRow<T>(string spName, IDictionary<string, object> spParams) where T : IBaseTable, new() => await ExecuteSPSingleRow<T>(spName, spParams, false);
        public async Task<Response<T>> ExecuteSPSingleRow<T>(string spName, IDictionary<string, object> spParams, bool throwError) where T : IBaseTable, new()
        {
            var response = new Response<T>();

            try
            {
                using (SqlConnection Connection = new SqlConnection(ConnectionString))
                {
                    await Connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(spName, Connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        AddParamToSqlCmd(spParams, command);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                response.Data = new T();
                                response.Data.LoadFromDataReader(reader);
                            }

                            await reader.NextResultAsync();

                            if (await reader.ReadAsync())
                            {
                                response.LoadFromDataReader(reader);
                            }
                            reader.Close();
                        }

                        command.Dispose();
                    }

                    Connection.Close();
                }

            }
            catch (Exception ex)
            {
                if (throwError) throw ex;
                else
                {
                    response.Message = ErrorManager.ShowError(ex);
                    response.Case = 0;
                }
            }

            return response;
        }

        public async Task<Response<List<T>>> ExecuteSPMultipleRow<T>(string spName) where T : IBaseTable, new() => await ExecuteSPMultipleRow<T>(spName, false);
        public async Task<Response<List<T>>> ExecuteSPMultipleRow<T>(string spName, bool throwError) where T : IBaseTable, new() => await ExecuteSPMultipleRow<T>(spName, null, throwError);
        public async Task<Response<List<T>>> ExecuteSPMultipleRow<T>(string spName, IDictionary<string, object> spParams) where T : IBaseTable, new() => await ExecuteSPMultipleRow<T>(spName, spParams, false);
        public async Task<Response<List<T>>> ExecuteSPMultipleRow<T>(string spName, IDictionary<string, object> spParams, bool throwError) where T : IBaseTable, new()
        {
            var response = new Response<List<T>>();
            try
            {
                using (SqlConnection Connection = new SqlConnection(ConnectionString))
                {
                    await Connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand(spName, Connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        AddParamToSqlCmd(spParams, command);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            response.Data = new List<T>();
                            while (await reader.ReadAsync())
                            {
                                var data = new T();
                                data.LoadFromDataReader(reader);
                                response.Data.Add(data);
                            }
                            await reader.NextResultAsync();
                            if (await reader.ReadAsync()) response.LoadFromDataReader(reader);
                            reader.Close();
                        }
                        command.Dispose();
                    }
                    Connection.Close();
                }
            }
            catch (Exception ex)
            {
                if (throwError) throw ex;
                else
                {
                    response.Message = ErrorManager.ShowError(ex);
                    response.Case = 0;
                }
            }

            return response;
        }

        public async Task<Response<List<T>>> ExecuteSPMultipleRowPaged<T>(string spName) where T : IBaseTable, new() => await ExecuteSPMultipleRowPaged<T>(spName, 1);
        public async Task<Response<List<T>>> ExecuteSPMultipleRowPaged<T>(string spName, int page) where T : IBaseTable, new() => await ExecuteSPMultipleRowPaged<T>(spName, page, 10);
        public async Task<Response<List<T>>> ExecuteSPMultipleRowPaged<T>(string spName, int page, int count) where T : IBaseTable, new() => await ExecuteSPMultipleRowPaged<T>(spName, page, count, false);
        public async Task<Response<List<T>>> ExecuteSPMultipleRowPaged<T>(string spName, int page, int count, bool throwError) where T : IBaseTable, new() => await ExecuteSPMultipleRowPaged<T>(spName, page, count, null, throwError);
        public async Task<Response<List<T>>> ExecuteSPMultipleRowPaged<T>(string spName, int page, int count, IDictionary<string, object> spParams) where T : IBaseTable, new() => await ExecuteSPMultipleRowPaged<T>(spName, page, count, spParams, false);
        public async Task<Response<List<T>>> ExecuteSPMultipleRowPaged<T>(string spName, int page, int count, IDictionary<string, object> spParams, bool throwError) where T : IBaseTable, new()
        {
            var response = new Response<List<T>>();

            try
            {
                using (SqlConnection Connection = new SqlConnection(ConnectionString))
                {
                    await Connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand(spName, Connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        AddParamToSqlCmd(spParams, command);
                        AddParamToSqlCmd(new Dictionary<string, object>() { { "Page", page }, { "Count", count } }, command);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            response.Data = new List<T>();
                            while (await reader.ReadAsync())
                            {
                                var data = new T();
                                data.LoadFromDataReader(reader);
                                response.Data.Add(data);
                            }
                            await reader.NextResultAsync();
                            if (await reader.ReadAsync()) response.LoadFromDataReader(reader);
                            await reader.NextResultAsync();
                            if (await reader.ReadAsync())
                            {
                                response.PageDetails = new PagedView();
                                response.PageDetails.LoadFromDataReader(reader);
                            }

                            reader.Close();
                        }
                        command.Dispose();
                    }
                    Connection.Close();
                }
            }
            catch (Exception ex)
            {
                if (throwError) throw ex;
                else
                {
                    response.Message = ErrorManager.ShowError(ex);
                    response.Case = 0;
                }
            }

            return response;
        }

        private void AddParamToSqlCmd(IDictionary<string, object> spParams, SqlCommand command)
        {
            if (spParams != null && spParams.Any())
            {
                foreach (var param in spParams)
                {
                    string key = param.Key;
                    object value = param.Value;

                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        const char dbParamPre = '@';
                        key = key.FirstOrDefault() == dbParamPre ? key : dbParamPre + key;
                        value = value ?? DBNull.Value;
                    }
                    else throw new Exception("Invalid paramater name was provided");

                    command.Parameters.AddWithValue(key, param.Value);
                }
            }
        }
    }
}
