using Noty.Interfaces;
using Noty.SqlServer;
using Polly.Wrap;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;


namespace Noty
{
    public class DataContextCore<TCommand, TConnection, TDataReader> where TCommand : DbCommand
                                                                    where TConnection : DbConnection
                                                                    where TDataReader : DbDataReader
    {
        protected readonly string _connectionString;

        protected readonly IContextConfiguration _config;

        public DataContextCore(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataContextCore(IContextConfiguration config)
        {
            _connectionString = config.GetConnectionString();
            _config = config;
        }

        private async Task<bool> ExecuteReader(string query, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters, Func<TDataReader, Task<bool>> func)
        {
            using (var sqlConnection = await CreateAndOpenSqlConnection())
            {
                using (var sqlCommand = Command.CreateCommand<TConnection, TCommand>(sqlConnection, query, commandType, parameters))
                {
                    using (var sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                    {
                        return await func((TDataReader)sqlDataReader);
                    }
                }
            }
        }
        private async Task<bool> ExecuteReaderWrapper(string query, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters, Func<TDataReader, Task<bool>> func)
        {
            var policy = _config?.GetPolicy<bool>();

            if (policy != null)
                return await policy.ExecuteAsync(() => ExecuteReader(query, commandType, parameters, func));
            else
                return await ExecuteReader(query, commandType, parameters, func);
        }

        private async Task<IEnumerable<T>> ExecuteCollectionReader<T>(string query, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            using (var sqlConnection = await CreateAndOpenSqlConnection())
            {
                using (var sqlCommand = Command.CreateCommand<TConnection, TCommand>(sqlConnection, query, commandType, parameters))
                {
                    using (var sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                    {
                        return await DataReader.MapDataToObjectCollection<T, DbDataReader>(sqlDataReader);
                    }
                }
            }
        }

        private async Task<IEnumerable<T>> ExecuteCollectionReaderWrapper<T>(string query, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            var policy = _config?.GetPolicy<IEnumerable<T>>();

            if (policy != null)
                return await policy.ExecuteAsync(() => ExecuteCollectionReader<T>(query, commandType, parameters));
            else
                return await ExecuteCollectionReader<T>(query, commandType, parameters);
        }

        private async Task<T> ExecuteReader<T>(string query, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            using (var sqlConnection = await CreateAndOpenSqlConnection())
            {
                using (var sqlCommand = Command.CreateCommand<TConnection, TCommand>(sqlConnection, query, commandType, parameters))
                {
                    using (var sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                    {
                        return await DataReader.MapDataToObject<T, DbDataReader>(sqlDataReader);
                    }
                }
            }
        }
        private async Task<T> ExecuteReaderWrapper<T>(string query, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            var policy = _config?.GetPolicy<T>();

            if (policy != null)
                return await policy.ExecuteAsync(() => ExecuteReader<T>(query, commandType, parameters));
            else
                return await ExecuteReader<T>(query, commandType, parameters);
        }

        private async Task<T> ExecuteScalar<T>(string query, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            using (var sqlConnection = await CreateAndOpenSqlConnection())
            {
                using (var sqlCommand = Command.CreateCommand<TConnection, TCommand>(sqlConnection, query, commandType, parameters))
                {
                    var result = await sqlCommand.ExecuteScalarAsync();
                    return result.MapDataToObject<T>();
                }
            }
        }
        private async Task<T> ExecuteScalarWrapper<T>(string query, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            var policy = _config?.GetPolicy<T>();

            if (policy != null)
                return await policy.ExecuteAsync(() => ExecuteScalar<T>(query, commandType, parameters));
            else
                return await ExecuteScalar<T>(query, commandType, parameters);
        }

        private async Task<int> ExecuteNonQuery(string query, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            using (var sqlConnection = await CreateAndOpenSqlConnection())
            {
                using (var sqlCommand = Command.CreateCommand<TConnection, TCommand>(sqlConnection, query, commandType, parameters))
                {
                    return await sqlCommand.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task<int> ExecuteNonQueryWrapper(string query, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            var policy = _config?.GetPolicy<int>();

            if (policy != null)
                return await policy.ExecuteAsync(() => ExecuteNonQuery(query, commandType, parameters));
            else
                return await ExecuteNonQuery(query, commandType, parameters);
        }


        #region Overrides

        public async Task<bool> ExecuteStoredProcedureReader(string query, IEnumerable<KeyValuePair<string, object>> parameters, Func<TDataReader, Task<bool>> func)
        {
            return await ExecuteReaderWrapper(query, CommandType.StoredProcedure, parameters, func);
        }

        public async Task<bool> ExecuteStoredProcedureReader(string query, Func<TDataReader, Task<bool>> func, params KeyValuePair<string, object>[] parameters)
        {
            return await ExecuteReaderWrapper(query, CommandType.StoredProcedure, parameters, func);
        }

        public async Task<bool> ExecuteStoredProcedureReader(string query, Func<TDataReader, Task<bool>> func)
        {
            return await ExecuteReaderWrapper(query, CommandType.StoredProcedure, null, func);
        }

        public async Task<bool> ExecuteReader(string query, Func<TDataReader, Task<bool>> func)
        {
            return await ExecuteReaderWrapper(query, CommandType.Text, null, func);
        }


        public async Task<int> ExecuteNonQuery(string query)
        {
            return await ExecuteNonQueryWrapper(query, CommandType.Text);
        }

        public async Task<int> ExecuteStoredProcedureNonQuery(string query, params KeyValuePair<string, object>[] parameters)
        {
            return await ExecuteNonQueryWrapper(query, CommandType.StoredProcedure, parameters);
        }

        public async Task<T> ExecuteStoredProcedureScalar<T>(string query, params KeyValuePair<string, object>[] parameters)
        {
            return await ExecuteScalarWrapper<T>(query, CommandType.StoredProcedure, parameters);
        }

        public async Task<T> ExecuteScalar<T>(string query)
        {
            return await ExecuteScalarWrapper<T>(query, CommandType.Text);
        }

        public async Task<T> ExecuteReader<T>(string query)
        {
            return await ExecuteReaderWrapper<T>(query, CommandType.Text);
        }

        public async Task<T> ExecuteStoredProcedureReader<T>(string query, params KeyValuePair<string, object>[] parameters)
        {
            return await ExecuteReaderWrapper<T>(query, CommandType.StoredProcedure, parameters);
        }

        public async Task<IEnumerable<T>> ExecuteCollectionReader<T>(string query)
        {
            return await ExecuteCollectionReaderWrapper<T>(query, CommandType.Text);
        }

        public async Task<IEnumerable<T>> ExecuteStoredProcedureCollectionReader<T>(string query, params KeyValuePair<string, object>[] parameters)
        {
            return await ExecuteCollectionReaderWrapper<T>(query, CommandType.StoredProcedure, parameters);
        }

        #endregion

        protected async Task<TConnection> CreateAndOpenSqlConnection()
        {
            var connection = (TConnection)Activator.CreateInstance(typeof(TConnection), _connectionString);

            await connection.OpenAsync();

            return connection;

        }
    }
}
