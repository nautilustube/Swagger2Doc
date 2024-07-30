using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Transactions;

namespace Swagger2Doc.Helpers
{
    public interface IDapperHelper
    {
        IDbConnection CreateConnection(string connectStringKey);
    }

    public class DapperHelper
    {
        private readonly IConfiguration config;

        public DapperHelper(IConfiguration config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public string GetConnectionString(string key)
        {
            return config.GetConnectionString(key) ?? throw new ArgumentNullException(nameof(key));
        }

        public IDbConnection CreateConnection(string connectStringKey)
        {
            string connectString = GetConnectionString(connectStringKey);
            return new SqlConnection(connectString);
        }

        public IEnumerable<T> Query<T>(string connectStringKey, string sql, object? param = null, CommandType commandType = CommandType.StoredProcedure)
        {
            using (IDbConnection conn = this.CreateConnection(connectStringKey))
            {
                return conn.Query<T>(sql, param, commandType: commandType);
            }
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string connectStringKey, string sql, object? param = null, CommandType commandType = CommandType.StoredProcedure)
        {
            using (IDbConnection conn = this.CreateConnection(connectStringKey))
            {
                var result = await conn.QueryAsync<T>(sql, param, commandType: commandType);
                return result;
            }
        }

        /// <summary>
        /// 執行單筆交易
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectStringKey"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int Execute(string connectStringKey, string sql, object? param = null, CommandType commandType = CommandType.StoredProcedure)
        {
            int rowCount = 0;
            using (IDbConnection conn = this.CreateConnection(connectStringKey))
            {
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    rowCount = conn.Execute(sql, param, commandType: commandType);
                    transactionScope.Complete();
                }
            }
            return rowCount;
        }
        public async Task<int> ExecuteAsync(string connectStringKey, string sql, object? param = null, CommandType commandType = CommandType.StoredProcedure)
        {
            int rowCount = 0;
            using (IDbConnection conn = this.CreateConnection(connectStringKey))
            {
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    rowCount = await conn.ExecuteAsync(sql, param, commandType: commandType);
                    transactionScope.Complete();
                }
            }
            return rowCount;
        }

        public int? ExecuteMulti<T>(string connectStringKey, IEnumerable<ExecuteActionDTO<T>> paramList)
        {
            int rowCount = 0;
            using (IDbConnection conn = this.CreateConnection(connectStringKey))
            {
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (ExecuteActionDTO<T> item in paramList)
                    {
                        rowCount += conn.Execute(item.sql, item.param, commandType: item.CommandType);
                    }
                    transactionScope.Complete();
                }
            }
            return rowCount;
        }

        public async Task<int?> ExecuteMultiAsync<T>(string connectStringKey, IEnumerable<ExecuteActionDTO<T>> paramList)
        {
            int rowCount = 0;
            using (IDbConnection conn = this.CreateConnection(connectStringKey))
            {
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (ExecuteActionDTO<T> item in paramList)
                    {
                        rowCount += await conn.ExecuteAsync(item.sql, item.param, commandType: item.CommandType);
                    }
                    transactionScope.Complete();
                }
            }
            return rowCount;
        }

        #region Read Data AS DataTable
        public DataTable? ExecuteReader(string connectStringKey, string sql, object? param = null, CommandType commandType = CommandType.StoredProcedure)
        {
            DataTable? result = null;
            using (IDbConnection conn = this.CreateConnection(connectStringKey))
            {
                IDataReader reader = conn.ExecuteReader(sql, param, commandType: commandType);
                if (((DbDataReader)reader).HasRows)
                {
                    result = new DataTable();
                    result.Load(reader);
                }
            }
            return result;
        }

        public async Task<DataTable?> ExecuteReaderAsync(string connectStringKey, string sql, object? param = null, CommandType commandType = CommandType.StoredProcedure)
        {
            DataTable? result = null;
            using (IDbConnection conn = this.CreateConnection(connectStringKey))
            {
                IDataReader reader = await conn.ExecuteReaderAsync(sql, param, commandType: commandType);
                if (((DbDataReader)reader).HasRows)
                {
                    result = new DataTable();
                    result.Load(reader);
                }
            }
            return result;
        }
        #endregion

    }

    #region Transaction Each Action DTO
    public class ExecuteActionDTO<T>
    {
        public T? param { get; set; }

        public string? sql { get; set; }

        public CommandType CommandType { get; set; } = CommandType.StoredProcedure;
    }
    #endregion
}
